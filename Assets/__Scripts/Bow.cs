using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : RangeWeapon
{
    [SerializeField] Transform rightHand;
    
    private SoundManager soundManager;
    private Animator animator;
    private Projectile currentArrow;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        soundManager = GetComponent<SoundManager>();
    }

    public override void Attack()
    {
        currentArrow.transform.SetParent(null, true);
        currentArrow.Shoot();        
    }

    public void AddArrow()
    {
        currentArrow = Instantiate(projectilePrefab).GetComponent<Projectile>();
        var pos = currentArrow.transform.position;
        var rot = currentArrow.transform.rotation;
        var scale = currentArrow.transform.localScale;
        currentArrow.target = weaponOwner.combatManager.currentTarget;
        currentArrow.weaponOwner = weaponOwner;
        currentArrow.transform.SetParent(rightHand, false);
        currentArrow.transform.position = rightHand.position;
        currentArrow.transform.localPosition += pos;
        currentArrow.transform.localRotation = rot;
        currentArrow.transform.localScale = scale;
    }

    public void LoadArrow()
    {
        animator.CrossFade("Load", Time.deltaTime);
    }

    public void PlayShootSound()
    {
        soundManager.PlaySound(SoundBank.instance.bowShootSound);
    }

}
