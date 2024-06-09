using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private Transform root;
    [SerializeField] private Transform inventory;
    [SerializeField] private Transform controls;

    private void OnEnable()
    {
        inventory.gameObject.SetActive(false);
        controls.gameObject.SetActive(false);
        root.gameObject.SetActive(true);
        PlayerInputManager.instance.uiStack.Add(transform);
    }

    private void OnDisable()
    {
        inventory.gameObject.SetActive(false);
        controls.gameObject.SetActive(false);
        root.gameObject.SetActive(false);
        PlayerInputManager.instance.uiStack.Remove(transform);
    }

    public void Continue()
    {
        Time.timeScale = 1.0f;
        gameObject.SetActive(false);
    }

    public void OpenInventoryMenu()
    {
        root.gameObject.SetActive(false);
        inventory.gameObject.SetActive(true);
    }

    public void ShowControls()
    {
        root.gameObject.SetActive(false);
        controls.gameObject.SetActive(true);
    }

    public void Restart()
    {
        foreach (var player in PlayersInGameManager.instance.playerList)
            player.PrepareToRestart();

        Time.timeScale = 1.0f;
        SceneManager.LoadScene(2);
    }

    public void Exit()
    {
        Application.Quit();
    }

}
