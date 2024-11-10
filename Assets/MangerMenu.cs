using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MangerMenu : MonoBehaviour
{
    //public pauseMenu optionsMenu;

    int tm = 1;
    [SerializeField] GameObject pauseMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //PlayerData data = SaveSystem.LoadPlayer();

        //Vector3 position;
        //position.x = data.position[0];
        //position.y = data.position[1];
        //position.z = data.position[2];
        //transform.position = position;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (tm == 1)
                tm--;
            else
                tm++;

            Time.timeScale = tm;
            pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
        }
    }
}






