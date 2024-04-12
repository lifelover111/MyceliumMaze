using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

using System.Reflection;

public class PurchaseUI : MonoBehaviour
{


    public TextMeshProUGUI priceText1;
    public TextMeshProUGUI priceText2;
    public TextMeshProUGUI priceText3;
    public void SetItemPrice(int index, int price)
    {
        switch (index)
        {
            case 0:
                priceText1.text = price.ToString();
                break;
            case 1:
                priceText2.text = price.ToString();
                break;
            case 2:
                priceText3.text = price.ToString();
                break;
            default:
                Debug.LogWarning("Invalid index for setting item price!");
                break;
        }
    }
    public Image imageItem1;
    public Image imageItem2;
    public Image imageItem3;
    public void SetItemImage(int index, Sprite sprite)
    {
        switch (index)
        {
            case 0:
                imageItem1.sprite = sprite;
                break;
            case 1:
                imageItem2.sprite = sprite;
                break;
            case 2:
                imageItem3.sprite = sprite;
                break;
            default:
                Debug.LogWarning("Invalid index for setting item image!");
                break;
        }
    }
    public Button buyButton1;
    public Button buyButton2;
    public Button buyButton3;
    private ItemDealer itemDealer;

    private void Start()
    {
        itemDealer = FindObjectOfType<ItemDealer>(); // Поиск экземпляра ItemDealer в сцене

        // Добавляем обработчики событий для кнопок покупки
        buyButton1.onClick.AddListener(() => PurchaseItem(0));
        buyButton2.onClick.AddListener(() => PurchaseItem(1));
        buyButton3.onClick.AddListener(() => PurchaseItem(2));
    }
    //private void Update()
    //{
    //    // Проверяем нажатие клавиш клавиатуры 1, 2 и 3
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        PurchaseItem(0); // Вызываем метод покупки предмета с индексом 0
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Alpha2))
    //    {
    //        PurchaseItem(1); // Вызываем метод покупки предмета с индексом 1
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Alpha3))
    //    {
    //        PurchaseItem(2); // Вызываем метод покупки предмета с индексом 2
    //    }
    //}
    // Метод для покупки предмета по индексу
    public void PurchaseItem(int index)
    {
        if (itemDealer != null)
        {
            // Вызываем метод покупки предмета у экземпляра ItemDealer
            Debug.LogWarning("ItemDound!");
            itemDealer.PurchaseItem(index);
            
        }
        else
        {
            Debug.LogWarning("ItemDealer not found!");
        }
    }

    //public Color highlightColor = Color.yellow;
    //public void OnButtonClick(Image imageToHighlight)
    //{
    //    imageToHighlight.color = highlightColor;
    //}


}
