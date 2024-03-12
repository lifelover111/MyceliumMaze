//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MenuMashroom : MonoBehaviour
//{
//    [SerializeField] Player player;
//    //public string canvasTag = "Bonfire"; // Тег объекта Canvas
//    //private GameObject canvas;

//    //void Start()
//    //{
//    //    canvas = GameObject.FindWithTag(canvasTag);
//    //    if (canvas != null)
//    //    {
//    //        canvas.SetActive(false);
//    //    }
//    //    else
//    //    {
//    //        Debug.LogError("Canvas with tag " + canvasTag + " not found!");
//    //    }
//    //}

//    //void OnTriggerEnter(Collider other)
//    //{
//    //    if (other.CompareTag("Interactable"))
//    //    {
//    //        canvas.SetActive(true);
//    //    }
//    //}

//    //void Update()
//    //{
//    //    if (Input.GetKeyDown(KeyCode.E))
//    //    {
//    //        if (canvas != null && canvas.activeSelf)
//    //        {
//    //            canvas.SetActive(false);
//    //        }
//    //    }
//    //}
//    Transform bonfireMenu;

//    public void ShowMenu()
//    {
//        Cursor.visible = true;
//        bonfireMenu.gameObject.SetActive(true);
//    }

//    public void HideMenu()
//    {
//        Cursor.visible = false;
//        bonfireMenu.gameObject.SetActive(false);
//    }
//}
