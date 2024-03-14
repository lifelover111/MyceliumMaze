using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Vector3 smoothingDelta;
    Vector3 delta;
    [SerializeField] Transform player;
    [SerializeField] int pixelHeight;
    private Camera cameraComponent;
    private void Awake()
    {
        delta = transform.position;
        cameraComponent = GetComponent<Camera>();
    }

    private void Update()
    {
        if (player is null)
            return;
        // Calculate the size of a pixel in world-space
        float pixelSize = 2f * cameraComponent.orthographicSize / pixelHeight;

        // Calculate target position in world-space
        Vector3 targetPosition = player.position + delta;

        // Calculate delta in both world-space and local-space
        Vector3 worldDelta = targetPosition - transform.position;
        Vector3 localDelta = transform.InverseTransformDirection(worldDelta) / pixelSize;

        // Round localDelta to the nearest pixel increment
        localDelta.x = Mathf.RoundToInt(localDelta.x);
        localDelta.y = Mathf.RoundToInt(localDelta.y);
        localDelta.z = Mathf.RoundToInt(localDelta.z);

        // Move the camera to the nearest point that is an increment of pixelSize
        transform.position +=
            transform.right * localDelta.x * pixelSize +
            transform.up * localDelta.y * pixelSize +
            transform.forward * localDelta.z * pixelSize;

        smoothingDelta = cameraComponent.WorldToScreenPoint(targetPosition)-cameraComponent.WorldToScreenPoint(transform.position);
        //Debug.Log(smoothingDelta);
    }
}
