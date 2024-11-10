using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MangerMenu : MonoBehaviour
{
    //public pauseMenu optionsMenu;

    [SerializeField] GameObject pauseMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
        }
    }
}






