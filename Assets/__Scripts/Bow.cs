using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    [SerializeField] GameObject arrowPrefab;
    public Vector2 direction;
    public float speed = 5f;
    DamageEffect bowDmgEf;
    private void Awake()
    {
        bowDmgEf = GetComponent<DamageEffect>();
    }
    private void OnEnable()
    {
        GameObject arrow = Instantiate(arrowPrefab);
        arrow.transform.position = transform.position;
        arrow.transform.rotation = Quaternion.FromToRotation(Vector2.right, direction);
        arrow.GetComponent<Rigidbody>().velocity = direction.normalized*speed;
        DamageEffect dmgEf = arrow.GetComponent<DamageEffect>();
        dmgEf.damage = bowDmgEf.damage;
        dmgEf.concentrationDamage = bowDmgEf.concentrationDamage;
        dmgEf.knockback = bowDmgEf.knockback;
    }
}
