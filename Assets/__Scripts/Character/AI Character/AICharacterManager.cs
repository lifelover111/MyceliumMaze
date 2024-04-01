using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{
    [HideInInspector] public AICharacterCombatManager aiCombatManager;
    [HideInInspector] public AICharacterLocomotionManager aiLocomotionManager;
    [HideInInspector] public AISteeringManager aiSteering;

    [Header("Spore Count Properties")]
    public int minSporeCount = 10;
    public int maxSporeCount = 60;

    [Header("Current State")]
    [SerializeField] AIState currentState;

    [Header("NavMesh Agent")]
    public NavMeshAgent navMeshAgent;

    [Header("States")]
    public IdleState idleState;
    public PursueTargetState pursueTargetState;
    public SurroundState surroundState;
    public CombatStanceState combatStanceState;
    public AttackState attackState;
    public BlockState blockState;

    [Header("AI flags")]
    public bool isMoving = false;


    [Header("GUI")]
    public GameObject guiPrefab;


    protected override void Awake()
    {
        base.Awake();
        aiCombatManager = GetComponent<AICharacterCombatManager>();
        aiLocomotionManager = GetComponent<AICharacterLocomotionManager>();
        aiSteering = GetComponent<AISteeringManager>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();

        idleState = Instantiate(idleState);
        pursueTargetState = Instantiate(pursueTargetState);
        surroundState = Instantiate(surroundState);
        attackState = Instantiate(attackState);
        combatStanceState = Instantiate(combatStanceState);
        blockState = Instantiate(blockState);

        currentState = idleState;
        OnDead += () => { PlayersInGameManager.instance.playerList.ForEach(p => p.GiveSpores(Random.Range(minSporeCount, maxSporeCount))); };
    }

    protected override void Start()
    {
        base.Start();
        EnableGUI();
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
        navMeshAgent.transform.localRotation = Quaternion.FromToRotation(transform.forward, aiLocomotionManager.GetForward());

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

    public void EnableGUI()
    {
        Instantiate(guiPrefab).GetComponent<EnemyUIManager>().Enable(statsManager);
    }

}
