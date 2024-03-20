using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextSolver : MonoBehaviour
{
    [SerializeField]
    private bool showGizmos = true;

    float[] interestGizmo = new float[0];
    [SerializeField] Vector3 resultDirection = Vector3.zero;
    private float rayLength = 2;
    [SerializeField] float getDirectionDelay = 0.1f;
    float timeDirectionGot;

    private void Start()
    {
        interestGizmo = new float[8];
    }

    public Vector3 GetDirectionToMove(List<SteeringBehaviour> behaviours, AIData aiData, float zRot = 0)
    {
        if (Time.time - timeDirectionGot > getDirectionDelay)
        {
            float[] danger = new float[8];
            float[] interest = new float[8];

            foreach (SteeringBehaviour behaviour in behaviours)
            {
                (danger, interest) = behaviour.GetSteering(danger, interest, aiData, zRot);
            }

            for (int i = 0; i < 8; i++)
            {
                interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
            }

            interestGizmo = interest;

            Vector3 outputDirection = Vector3.zero;
            for (int i = 0; i < 8; i++)
            {
                outputDirection += Directions.eightDirections[i] * interest[i];
            }

            outputDirection.Normalize();

            resultDirection = outputDirection;
            timeDirectionGot = Time.time;
        }

        return resultDirection;
    }


    private void OnDrawGizmos()
    {
        if (Application.isPlaying && showGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, resultDirection * rayLength);
        }
    }
}
