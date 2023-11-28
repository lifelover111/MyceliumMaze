using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    public static string seed;
    public static Texture2D cursor;
    public static void PrepareSceneBeforeLoad()
    {
        OldProject.Door.enemiesInCurrentRoom?.Clear();
        Object.Destroy(MusicSource.instance?.gameObject);
    }

    public static IEnumerator LoadSceneAfterTime(string name, float time)
    {
        PrepareSceneBeforeLoad();

        yield return new WaitForSeconds(time);

        SceneManager.LoadScene(name);
    }

    public static IEnumerator WaitForTime(float time)
    {
        yield return new WaitForSeconds(time);
    }
}
