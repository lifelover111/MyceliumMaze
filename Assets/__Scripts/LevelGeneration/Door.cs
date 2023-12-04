using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Door : MonoBehaviour
{
    public Vector3 enterPositionShift = Vector3.zero;
    Door transitionTo;
    Collider coll;
    public static event System.Action OnTransition;
    static Transform player;
    Room currentRoom;

    private void Awake()
    {
        OnTransition = () => { };
        coll = GetComponent<Collider>();
        currentRoom = GetComponentInParent<Room>();
    }
    private void Start()
    {
        if(transitionTo == null)
            coll.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;
        OnTransition?.Invoke();
        player = other.transform;
        StartCoroutine(nameof(TranslatePlayer));
    }

    IEnumerator TranslatePlayer()
    {
        float time = Time.time;
        while (Mathf.Sin(Time.time - time) < 0.99)
        {
            yield return null;
        }
        player.position = transitionTo.transform.position + transitionTo.enterPositionShift + 0.5f * Vector3.up;
        transitionTo.currentRoom.WakeEnemies();
    }

    public void ConnectWith(Door toConnect)
    {
        transitionTo = toConnect;
        toConnect.transitionTo = this;

        //Скрытие объектов дверей
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < transitionTo.transform.childCount; i++)
        {
            transitionTo.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public bool IsEmptyTransition()
    {
        return transitionTo == null;
    }
}
