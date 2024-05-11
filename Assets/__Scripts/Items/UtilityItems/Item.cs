using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public abstract class Item : ScriptableObject
{
    [HideInInspector] public int id;

    [Header("Item properties")]
    [SerializeField] public string itemName;
    [SerializeField] public string descriptionShort;
    [SerializeField] public string description;
    [SerializeField] public Sprite icon;
    [SerializeField] public int meanPrice;

    public virtual void PickUp(PlayerManager player)
    {
    }
    public virtual void Remove(PlayerManager player)
    {
    }

    public virtual void Reset()
    {

    }
}

public abstract class ActiveItem : Item
{
    [Header("Active Item Properties")]
    public int cooldownUnits = 100;

    [HideInInspector] public int currentCooldown;
    
    public event System.Func<PlayerManager, bool> OnTryUse;

    public static int cooldownTickRegeneration = 5;

    public ActiveItem()
    {
        Reset();
    }

    public void Use(PlayerManager player)
    {
        if (currentCooldown < cooldownUnits)
            return;

        if(OnTryUse.Invoke(player))
            currentCooldown = 0;
    }

    public override void PickUp(PlayerManager player)
    {
        
        base.PickUp(player);
        player.playerCombatManager.SubscribeToHitEnemy(() => HandleCooldownTick(cooldownTickRegeneration));
    }

    public void HandleCooldownTick(int value)
    {
        currentCooldown += value;
        currentCooldown = currentCooldown > cooldownUnits ? cooldownUnits : currentCooldown;
    }

    /// <summary>
    /// В этом методе необходимо добавить к событию OnTryUse обработчик с логикой предмета. Обработчик возвращает true, если эффект был успешно прменен.
    /// </summary>
    public override void Reset()
    {
        base.Reset();
        OnTryUse = null;
        currentCooldown = cooldownUnits;
    }
}

public abstract class PassiveItem : Item
{
    public virtual void ProcessPassiveEffect(PlayerManager player)
    {

    }
}
