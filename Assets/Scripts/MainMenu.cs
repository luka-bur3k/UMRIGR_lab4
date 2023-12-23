using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static PlayerType playerType;

    public void Create()
    {
        Debug.Log("Create");
        playerType = PlayerType.HOST;
        SceneManager.LoadScene(1);
    }

    public void Join()
    {
        Debug.Log("Join");
        playerType = PlayerType.CLIENT;
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
