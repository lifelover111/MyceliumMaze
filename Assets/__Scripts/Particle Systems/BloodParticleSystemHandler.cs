using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticleSystemHandler : MonoBehaviour
{

    public static BloodParticleSystemHandler Instance { get; private set; }

    private MeshParticleSystem meshParticleSystem;
    private List<Single> singleList;

    private void Awake()
    {
        Instance = this;
        meshParticleSystem = GetComponent<MeshParticleSystem>();
        singleList = new List<Single>();
    }

    private void Update()
    {
        for (int i = 0; i < singleList.Count; i++)
        {
            Single single = singleList[i];
            single.Update();
            if (single.IsParticleComplete())
            {
                singleList.RemoveAt(i);
                i--;
            }
        }
    }

    public void SpawnBlood(Vector3 position, Vector3 direction)
    {
        //число частиц крови
        float bloodParticleCount = Random.Range(5, 15);
        for (int i = 0; i < bloodParticleCount; i++)
        {
            singleList.Add(new Single(position, ApplyRotationToVector(direction, Random.Range(-15f, 15f)), meshParticleSystem));
        }
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public static Vector3 ApplyRotationToVector(Vector3 vec, Vector3 vecRotation)
    {
        return ApplyRotationToVector(vec, GetAngleFromVectorFloat(vecRotation));
    }

    public static Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, angle, 0) * vec;
    }



    private class Single
    {

        private MeshParticleSystem meshParticleSystem;
        private Vector3 position;
        private Vector3 direction;
        private int quadIndex;
        private Vector3 quadSize;
        private float moveSpeed;
        private float rotation;
        private int uvIndex;

        private float particleSize = .5f;

        public Single(Vector3 position, Vector3 direction, MeshParticleSystem meshParticleSystem)
        {
            this.position = position;
            this.direction = direction.normalized;
            this.meshParticleSystem = meshParticleSystem;

            quadSize = Random.Range(1f, 1.8f) * new Vector3(particleSize, particleSize); //размер частицы 
            rotation = Random.Range(0, 360f);
            moveSpeed = Random.Range(25, 40);
            uvIndex = Random.Range(0, 8);

            quadIndex = meshParticleSystem.AddQuad(position, rotation, quadSize, false, uvIndex);
        }

        public void Update()
        {
            position += direction * moveSpeed * Time.deltaTime;
            rotation += 360f * (moveSpeed / 10f) * Time.deltaTime;

            meshParticleSystem.UpdateQuad(quadIndex, position, rotation, quadSize, false, uvIndex);

            float slowDownFactor = 5f;
            moveSpeed -= moveSpeed * slowDownFactor * Time.deltaTime;
        }

        public bool IsParticleComplete()
        {
            return moveSpeed < 15f;
        }

    }

}