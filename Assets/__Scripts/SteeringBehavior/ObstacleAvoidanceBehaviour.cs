using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidanceBehaviour : SteeringBehaviour
{
    [SerializeField]
    private float radius = 2f, agentColliderSize = 0.6f;

    [SerializeField]
    private bool showGizmo = true;

    float[] dangersResultTemp = null;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData, float zRot = 0)
    {
        foreach (Collider obstacleCollider in aiData.obstacles)
        {
            if(obstacleCollider != null)
            {
                Vector3 directionToObstacle
                    = obstacleCollider.ClosestPoint(transform.position) - transform.position;
                float distanceToObstacle = directionToObstacle.magnitude;

                float weight
                    = distanceToObstacle <= agentColliderSize
                    ? 1
                    : (radius - distanceToObstacle) / radius;

                Vector3 directionToObstacleNormalized = directionToObstacle.normalized;

                for (int i = 0; i < Directions.eightDirections.Count; i++)
                {
                    float result = Vector3.Dot(directionToObstacleNormalized, Directions.eightDirections[i]);

                    float valueToPutIn = result * weight;

                    //override value only if it is higher than the current one stored in the danger array
                    if (valueToPutIn > danger[i])
                    {
                        danger[i] = valueToPutIn;
                    }
                }
            }
        }
        dangersResultTemp = danger;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {
        if (showGizmo == false)
            return;

        if (Application.isPlaying && dangersResultTemp != null)
        {
            if (dangersResultTemp != null)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < dangersResultTemp.Length; i++)
                {
                    Gizmos.DrawRay(
                        transform.position,
                        Directions.eightDirections[i] * dangersResultTemp[i]*2
                        );
                }
            }
        }

    }
}

public static class Directions
{
    public static List<Vector3> eightDirections = new List<Vector3>{
            Vector3.forward,
            new Vector3(1, 0, 1).normalized,
            Vector3.right,
            new Vector3(1, 0, -1).normalized,
            Vector3.back,
            new Vector3(-1, 0, -1).normalized,
            Vector3.left,
            new Vector3(-1, 0, 1).normalized
        };
}
