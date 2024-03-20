using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISteeringManager : MonoBehaviour
{
    private AIData aiData;
    private MovingAI movingAI;
    [SerializeField] private Transform steering;

    private void Awake()
    {
        aiData = steering.GetComponent<AIData>();
        movingAI = steering.GetComponent<MovingAI>();
    }

    public Vector3 GetDirection(Transform target)
    {
        aiData.SetTarget(target);
        return movingAI.movementDirectionSolver.GetDirectionToMove(movingAI.steeringBehaviours, aiData);
    }

}
