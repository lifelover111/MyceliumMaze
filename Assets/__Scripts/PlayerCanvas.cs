using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    public static PlayerCanvas instance;

    public Transform HPBar;
    public Transform SanityBar;
    public Transform ConcentrationBar;
    public Transform SporeCounter;
    public Transform FlaskCounter;
    public Transform ActiveItemCooldownIndicator;
    public Transform ActiveItemCooldownIndicatorFrame;
    public Transform CooldownShadow;
    public Transform InteractButton;
    public Transform PauseUI;

    public ItemNotification itemNotification;

    public PurchaseUI purchaseWindow;
    public InventoryUI inventoryWindow;
    public Image activeItemIconRenderer;
    public GameObject menuCanvas;

    private void Awake()
    {
        instance = this;
    }
}
