using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool isHoming = true;
    public float homingAcceleration = 1f;
    public float speed;
    public float lifetime;

    public CharacterManager target;
    public CharacterManager weaponOwner;

    private string ownerTag;
    private float spawnTime;
    private bool lifetimeEnded = false;


    private void Start()
    {
        var damageCollider = GetComponent<ProjectileDamageCollider>();
        damageCollider.weaponOwner = weaponOwner;
        spawnTime = Time.time;
        var direction = target.transform.position - transform.position;
        direction.y = 0;
        direction.Normalize();
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        ownerTag = weaponOwner.gameObject.tag;
    }

    private void Update()
    {
        if (lifetimeEnded)
            return;
        if(Time.time - spawnTime >= lifetime)
        {
            lifetimeEnded = true;
            StartCoroutine(DestroyCoroutine());
        }

        transform.position += transform.forward * speed * Time.deltaTime;

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
        if (other.tag.Equals(ownerTag))
            return;

        StartCoroutine(DestroyCoroutine());
    }
    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Destroy(gameObject);
    }
}
