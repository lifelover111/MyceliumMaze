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
    public Room room { get { return currentRoom; } }
    [SerializeField] public DoorSelector doorSelector;
    //
    //public delegate void DoorAction();
    //public event DoorAction OnDoorOpen;
    //public event DoorAction OnDoorClose;

    //public void OpenDoor()
    //{
    //    Debug.Log("Door is opening...");
    //    OnDoorOpen?.Invoke();
    //}

    //public void CloseDoor()
    //{
    //    Debug.Log("Door is closing...");
    //    OnDoorClose?.Invoke();
    //}
    //

    //
    //

    public void OpenDoor()
    {
        doorSelector.OpenDoors();
    }

    public void CloseDoor()
    {
        doorSelector.CloseDoors();
    }

    private void Awake()
    {
        OnTransition = () => { };
        coll = GetComponent<Collider>();
        currentRoom = GetComponentInParent<Room>();
    }
    private void Start()
    {
        if (transitionTo == null)
            coll.enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;
        
        OnTransition?.Invoke();
        player = other.transform;
        StartCoroutine(TranslatePlayer());
    }



    IEnumerator TranslatePlayer()
    {

        transitionTo.room.gameObject.SetActive(true);
        float time = Time.time;
        while (Mathf.Sin(Time.time - time) < 0.99)
        {
            yield return null;
        }
        currentRoom.gameObject.SetActive(false);
        player.position = transitionTo.transform.position + transitionTo.enterPositionShift + 0.5f * Vector3.up;


        transitionTo.currentRoom.WakeEnemies();
        
    }

    public void ConnectWith(Door toConnect)
    {
        transitionTo = toConnect;
        toConnect.transitionTo = this;

        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);

        transitionTo.transform.GetChild(0).gameObject.SetActive(true);
        transitionTo.transform.GetChild(1).gameObject.SetActive(false);

    }

    public bool IsEmptyTransition()
    {
        return transitionTo == null;
    }
}
