using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //[SerializeField] GameObject pauseMenu;
    public void Load()
    {
        //Debug.LogError("Hello World!");
        //SaveSystem.SavePlayer(player);
        SceneManager.LoadScene("PauseSave Menu");
        //SaveSystem.LoadPlayer();
        //Debug.LogError("Hello World!");
    }
}
