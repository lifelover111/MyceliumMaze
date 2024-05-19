using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{
    [HideInInspector] public AICharacterCombatManager aiCombatManager;
    [HideInInspector] public AICharacterLocomotionManager aiLocomotionManager;

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
    public KeepDistanceState keepDistanceState;

    [Header("AI flags")]
    public bool isMoving = false;
    public bool isSleeping = false;


    [Header("GUI")]
    public GameObject guiPrefab;
    private EnemyUIManager gui;

    [Header("Animator options")]
    public bool noRootMotion = false;


    [Header("Level generation")]
    [SerializeField] private int spawnCost = 0;

    [Header("Options")]
    public bool movable = true;

    public int SpawnCost => spawnCost;

    protected override void Awake()
    {
        base.Awake();
        aiCombatManager = GetComponent<AICharacterCombatManager>();
        aiLocomotionManager = GetComponent<AICharacterLocomotionManager>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();

        idleState = Instantiate(idleState);
        pursueTargetState = Instantiate(pursueTargetState);
        surroundState = Instantiate(surroundState);
        attackState = Instantiate(attackState);
        combatStanceState = Instantiate(combatStanceState);
        if(blockState != null)
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

    protected override void Update()
    {
        base.Update();
        HandleSleep();
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
                isMoving = movable;
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

    private void HandleSleep()
    {
        if (isDead)
            return;

        characterController.enabled = !isSleeping;
        isInvulnerable = isSleeping;
                
        if(gui != null)
            gui.gameObject.SetActive(!isSleeping);
    }

    public void EnableGUI()
    {
        if (guiPrefab == null)
            return;

        gui = Instantiate(guiPrefab).GetComponent<EnemyUIManager>();
        gui.Enable(statsManager);
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        //Попытки пофиксить баг с проигрыванием анимации смерти. 
        navMeshAgent.enabled = false;
        animatorManager.UpdateAnimatorBlockParameters(false);
        //
        yield return base.ProcessDeathEvent(manuallySelectDeathAnimation);
    }

}
