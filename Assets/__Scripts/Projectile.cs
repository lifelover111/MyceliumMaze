using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool isHoming = true;
    public float homingAcceleration = 1f;
    public float speed;
    public float lifetime;
    public bool shootable = false;
    public bool manuallySetRotation = false;

    public CharacterManager target;
    public CharacterManager weaponOwner;

    private string ownerTag;
    private float spawnTime;
    private bool lifetimeEnded = false;
    private bool shooted = false;


    public Vector3 baseRotation = Vector3.zero;

    private void Start()
    {
        var damageCollider = GetComponent<ProjectileDamageCollider>();
        damageCollider.weaponOwner = weaponOwner;
        spawnTime = Time.time;

        if (!manuallySetRotation)
        {
            var direction = target.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        ownerTag = weaponOwner.gameObject.tag;
    }

    private void Update()
    {
        if (shootable && !shooted)
            return;
        if (lifetimeEnded)
            return;
        if(Time.time - spawnTime >= lifetime)
        {
            lifetimeEnded = true;
            StartCoroutine(DestroyCoroutine());
        }
        var moveDirection = Quaternion.Euler(baseRotation) * transform.forward;
        moveDirection.y = 0;
        moveDirection.Normalize();

        transform.position += moveDirection * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, 2, transform.position.z);

        if (isHoming)
        {
            var direction = target.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), homingAcceleration*Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (shootable && !shooted)
            return;

        if (weaponOwner != null && other.transform.IsChildOf(weaponOwner.transform))
            return;

        StartCoroutine(DestroyCoroutine());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (shootable && !shooted)
            return;

        if (collision.transform.IsChildOf(weaponOwner.transform))
            return;
        Destroy(gameObject);
    }

    public void Shoot()
    {
        shooted = true;
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Destroy(gameObject);
    }
}
