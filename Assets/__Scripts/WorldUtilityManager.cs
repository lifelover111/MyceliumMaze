using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldUtilityManager : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
    public static float GetAngleOfTarget(Vector3 characterForward, Vector3 targetsDirection)
    {
        targetsDirection.y = 0;
        float viewableAngle = Vector3.Angle(characterForward, targetsDirection);
        Vector3 cross = Vector3.Cross(characterForward, targetsDirection);
        if (cross.y < 0) viewableAngle *= -1;

        return viewableAngle;
    }

    public static bool RollForOutcomeChance(int outcomeChance)
    {
        bool outcomeWillBePerformed = false;

        int randomPercentage = Random.Range(0, 100);

        if (randomPercentage <= outcomeChance)
            outcomeWillBePerformed = true;

        return outcomeWillBePerformed;
    }
}
