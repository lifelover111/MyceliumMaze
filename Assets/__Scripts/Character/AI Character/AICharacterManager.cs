using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{
    [HideInInspector] public AICharacterCombatManager aiCombatManager;
    [HideInInspector] public AICharacterLocomotionManager aiLocomotionManager;

    [Header("Current State")]
    [SerializeField] AIState currentState;

    [Header("NavMesh Agent")]
    public NavMeshAgent navMeshAgent;

    [Header("States")]
    public IdleState idleState;
    public PursueTargetState pursueTargetState;
    public SurroundState surroundState;

    [Header("AI flags")]
    public bool isMoving = false;

    [Header("Model Forward")]
    public Vector3 forward;


    protected override void Awake()
    {
        base.Awake();
        aiCombatManager = GetComponent<AICharacterCombatManager>();
        aiLocomotionManager = GetComponent<AICharacterLocomotionManager>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();

        idleState = Instantiate(idleState);
        pursueTargetState = Instantiate(pursueTargetState);
        surroundState = Instantiate(surroundState);
        currentState = idleState;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        ProcessStateMachine();
    }

    private void ProcessStateMachine()
    {
        AIState nextState = currentState?.Tick(this);

        if(nextState is not null)
        {
            currentState = nextState;
        }

        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.FromToRotation(Vector3.forward, aiLocomotionManager.GetForward());  //Quaternion.Euler(0, -90, 0);

        if (navMeshAgent.enabled)
        {
            Vector3 agentDestination = navMeshAgent.destination;
            float remainingDistance = Vector3.Distance(agentDestination, transform.position);

            if(remainingDistance > navMeshAgent.stoppingDistance)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }
        else
        {
            isMoving = false;
        }
    }

}
