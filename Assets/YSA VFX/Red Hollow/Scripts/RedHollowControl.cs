using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedHollowControl : MonoBehaviour
{
    [SerializeField] new Collider collider;
    public CharacterManager owner;

    [Range(0.0f, 1.0f)]
    public float hue = 0;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        GetComponentInChildren<ConcentrationDamageCollider>().characterCausingDamage = owner;
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetChild(0).GetComponent<HueControl>().hue = hue;
    }

    public void Play_Charging() {
        animator.Play("Red Hollow - Charging");
    }

    public void Finish_Charging() {
        animator.Play("Red Hollow - Charged");
    }

    public void Burst_Beam() {
        animator.Play("Red Hollow - Burst");
	if(Camera.main.transform.GetComponent<CameraShake>() != null){
		Camera.main.transform.GetComponent<CameraShake>().Shake(0.5f, 1f);
	}
        collider.enabled = true;
    }

    public void Dead()
    {
        collider.enabled = false;
        animator.Play("Red Hollow - Dead");
        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
