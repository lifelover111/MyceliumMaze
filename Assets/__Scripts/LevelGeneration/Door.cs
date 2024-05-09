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
    [SerializeField] Light[] lights;

    public static float transitionSpeed = 1;

    public void OpenDoor()
    {
        doorSelector.OpenDoor();
    }

    public void CloseDoor()
    {
        doorSelector.CloseDoor();
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

    public void EnableLights()
    {
        foreach (var light in lights)
        {
            StartCoroutine(EnableLightCoroutine(light));
        }
    }

    IEnumerator EnableLightCoroutine(Light light)
    {
        while (light.intensity < 15) 
        {
            light.intensity += 5*Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator TranslatePlayer()
    {
        transitionTo.room.gameObject.SetActive(true);
        EnableLights();
        transitionTo.EnableLights();

        float time = Time.time;
        var playerManager = player.GetComponent<PlayerManager>();
        playerManager.playerLocomotionManager.externallyControlled = true;
        playerManager.playerLocomotionManager.GoTowards(Vector3.Project((transitionTo.transform.position - transform.position).normalized, Vector3.forward));
        playerManager.CurrentRoom = transitionTo.room;

        while (transitionSpeed * Mathf.Sin(Time.time - time) < 0.99)
        {
            yield return null;
        }
        playerManager.characterController.enabled = false;
        player.position = transitionTo.transform.position + transitionTo.enterPositionShift;
        playerManager.characterController.enabled = true;

        currentRoom.DestroyEnemies();
        currentRoom.gameObject.SetActive(false);

        playerManager.isPerformingAction = false;
        playerManager.canMove = true;
        playerManager.canRotate = true;
        playerManager.playerLocomotionManager.externallyControlled = false;
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
