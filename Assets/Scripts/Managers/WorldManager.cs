using UnityEngine;
using System.IO;
using UnityEngine.UIElements;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }
    [SerializeField] private Player player;

    [Header("UI Elements")]
    private static VisualElement savePanel;
    private Button saveButton;
    private Button cancelButton;

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
        string json = JsonUtility.ToJson(player);
        File.WriteAllText(Application.persistentDataPath + "/playerData.json", json);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/playerData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, player);
        }
    }

    public void AddItemToInventory(Item item)
    {
        player.AddItem(item);
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