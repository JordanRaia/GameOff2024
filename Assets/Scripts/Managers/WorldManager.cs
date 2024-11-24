using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }
    [SerializeField] private Player player;
    [SerializeField] private GameObject playerGameObject; // Reference to the Player GameObject

    [Header("UI Elements")]
    private static VisualElement savePanel;
    private Button saveButton;
    private Button cancelButton;

    // Add a list to track picked up world items
    private List<string> pickedUpItems = new List<string>();

    // Properties to access variables if needed
    public List<string> PickedUpItems => pickedUpItems;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadData();

        var root = GetComponent<UIDocument>().rootVisualElement;

        savePanel = root.Q<VisualElement>("SavePanel");
        saveButton = root.Q<Button>("SaveButton");
        cancelButton = root.Q<Button>("CancelButton");

        saveButton.clicked += SaveData;
        cancelButton.clicked += () => savePanel.style.display = DisplayStyle.None;
    }

    public void SaveData()
    {
        // Create a save data object
        SaveDataObject saveData = new SaveDataObject
        {
            playerData = JsonUtility.ToJson(player),
            playerPosition = playerGameObject.transform.position,
            currentScene = SceneManager.GetActiveScene().name,
            pickedUpItems = pickedUpItems
        };

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/playerData.json", json);

        Debug.Log(Application.persistentDataPath + "/playerData.json");

        savePanel.style.display = DisplayStyle.None;
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/playerData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveDataObject saveData = JsonUtility.FromJson<SaveDataObject>(json);
            JsonUtility.FromJsonOverwrite(saveData.playerData, player);
            // Set player position and load scene
            //SceneManager.LoadScene(saveData.currentScene);
            // Delay setting position until after scene loads
            StartCoroutine(SetPlayerPosition(saveData.playerPosition));
            pickedUpItems = saveData.pickedUpItems;
        }
    }

    private System.Collections.IEnumerator SetPlayerPosition(Vector3 position)
    {
        // Wait for the scene to load
        yield return null;
        if (playerGameObject != null)
        {
            playerGameObject.transform.position = position;
        }
    }

    public void AddItemToInventory(Item item, string id)
    {
        player.AddItem(item);
        pickedUpItems.Add(id); // Use the provided ID
    }

    public static void EnableUIDocument()
    {
        savePanel.style.display = DisplayStyle.Flex;
    }

    public static void DisableUIDocument()
    {
        savePanel.style.display = DisplayStyle.None;
    }

    public static bool IsUIDocumentActive()
    {
        return savePanel.style.display == DisplayStyle.Flex;
    }
}

// Serializable class to hold all save data
[System.Serializable]
public class SaveDataObject
{
    public string playerData;
    public Vector3 playerPosition;
    public string currentScene;
    public List<string> pickedUpItems;
}