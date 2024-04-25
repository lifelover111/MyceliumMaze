using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemNotification : MonoBehaviour
{
    private Animation appearanceAnimation;
    [SerializeField] Image icon;
    [SerializeField] TMP_Text caption;
    [SerializeField] TMP_Text description;

    private void Awake()
    {
        appearanceAnimation = GetComponent<Animation>();
    }

    public void SetItem(Item item)
    {
        icon.sprite = item.icon;
        caption.text = item.itemName;
        description.text = item.descriptionShort;
    }

    private void OnEnable()
    {
        appearanceAnimation.Play();
    }


}
