//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseMenuSCPT : MonoBehaviour
{
    //[SerializeField] GameObject pauseMenu;
    public void Save(TopDownMovement player)
    {
        //Debug.LogError("Hello World!");
        SaveSystem.SavePlayer(player);
        SceneManager.LoadScene("main menu");
        //Debug.LogError("Hello World!");
    }

    public void Load()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        transform.position = position;
    }
}
