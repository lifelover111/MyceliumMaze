using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonfireMenu : MonoBehaviour
{
    [SerializeField] Player player;
    Transform background;
    Transform rootMenu;
    Transform levelUpMenu;
    Transform restorationMenu;

    private void Awake()
    {
        background = transform.GetChild(0);
        rootMenu = transform.GetChild(1);
        levelUpMenu = transform.GetChild(2);
        restorationMenu = transform.GetChild(3);


    }

    private void OnEnable()
    {
        background.gameObject.SetActive(true);
        rootMenu.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        background.gameObject.SetActive(false);
        rootMenu.gameObject.SetActive(false);
        levelUpMenu.gameObject.SetActive(false);
        restorationMenu.gameObject.SetActive(false);
    }


    public void Leave()
    {
        /*
        System.Action leave;
        leave = delegate () { player.EndRest(); };
        Invoke(leave.Method.Name, 0.4f);
        */
    }
    public void RestoreFlasks()
    {
        rootMenu.gameObject.SetActive(false);
        restorationMenu.gameObject.SetActive(true);
        Transform message = restorationMenu.Find("Message");
        string text;
        if(player.spores >= 300)
        {
            text = "You will restore "+(player.maxNumFlasks- player.numFlasks)+" flasks. \n It will cost "+300+" spores.\n Do you wish to continue?";
            restorationMenu.Find("Ok").gameObject.SetActive(true);
        }
        else
        {
            text = "You don't have enough spores.\n Spores required: " + 300 + '.';
            restorationMenu.Find("Ok").gameObject.SetActive(false);
        }
        message.gameObject.GetComponent<Text>().text = text;
    }

    public void ConfirmFlasksRestoration()
    {
        player.spores -= 300;
        player.numFlasks = player.maxNumFlasks;
        restorationMenu.gameObject.SetActive(false);
        rootMenu.gameObject.SetActive(true);
    }
    public void CancelFlasksRestoration()
    {
        restorationMenu.gameObject.SetActive(false);
        rootMenu.gameObject.SetActive(true);
    }


}