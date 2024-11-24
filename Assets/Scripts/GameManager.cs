using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void LoadGame()
    {

    }

    public void NewGame()
    {
        SceneManager.LoadScene((int)SceneIndexes.OpeningScene);
    }
}
