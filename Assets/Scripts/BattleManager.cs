using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    End
}

public class BattleManager : MonoBehaviour
{
    //--- Serialized Fields (UI and Prefabs) ---
    [Header("UI and Prefabs")]
    [SerializeField] private GameObject linePrefab; // Reference to the line prefab
    [SerializeField] private BoxScaler boxScaler;   // Reference to the BoxScaler component
    [SerializeField] private Sprite radiantSprite;  // Sprite to display in the box for a radiant effect
    [SerializeField] private GameObject enemyPrefab; // Reference to the enemy prefab

    //--- UI Elements (Buttons and Pages) ---
    [Header("UI Elements")]
    private Button fightButton;     // Button for Fight action
    private Button actButton;       // Button for Act action
    private Button itemButton;      // Button for Item action
    private Button mercyButton;     // Button for Mercy action
    private Label boxText;          // Label for dialogue or text display in the box
    private VisualElement dialogueBox; // Main dialogue box
    private VisualElement itemsPage;   // Visual element for item display page
    private VisualElement actPage;     // Visual element for act options page
    private VisualElement enemyPage;   // Visual element for enemy selection page

    [Header("Enemy Spawn Settings")]
    [SerializeField] private float spawnAreaWidth = 10.0f; // Total width for enemy spawn area
    //[SerializeField] private float spawnAreaHeight = 2.0f; // Optional: height, if needed for vertical spacing
    [SerializeField] private float yOffset = 2.0f; // Vertical offset from the center for positioning

    [SerializeField] private float enemyScale = 6.0f;   // Desired scale for enemies
    //[SerializeField] private float enemySpacing = 2.0f; // Desired spacing between enemies


    //--- UI Management Scripts ---
    [Header("UI Management Scripts")]
    private BattleItemUI battleItemUI;  // UI component for displaying items
    private BattleActUI battleActUI;    // UI component for displaying act options
    private BattleEnemiesUI battleEnemyUI; // UI component for displaying enemies

    //--- Game Data (Player and Enemies) ---
    [Header("Game Data")]
    public Player player;              // Reference to the player object
    public List<BattleEnemy> enemies;  // List of enemies in the battle
    private List<GameObject> instantiatedEnemies = new List<GameObject>(); // List to store instantiated enemy objects

    //--- Battle State Management ---
    [Header("Battle State Management")]
    private BattleState state;         // Current state of the battle
    public static BattleManager Instance; // Singleton instance for easy access

    //--- Unity Lifecycle Methods ---
    void Awake()
    {
        Instance = this; // Initialize singleton instance
    }

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        fightButton = root.Q<Button>("ButtonFight");
        actButton = root.Q<Button>("ButtonAct");
        itemButton = root.Q<Button>("ButtonItem");
        mercyButton = root.Q<Button>("ButtonMercy");
        boxText = root.Q<Label>("BoxText");
        itemsPage = root.Q<VisualElement>("ItemPage");
        actPage = root.Q<VisualElement>("ActPage");
        enemyPage = root.Q<VisualElement>("EnemyPage");

        fightButton.clicked += OnFightButton;
        actButton.clicked += OnActButton;
        itemButton.clicked += OnItemButton;
        mercyButton.clicked += OnMercyButton;

        // Spawn enemies
        LayoutEnemies(); // Layout the enemies in the battle scene

        // Initialize BattleItemUI and update items
        battleItemUI = GetComponent<BattleItemUI>();
        battleItemUI.UpdateItems(player.GetItemsByType(ItemType.Health));

        List<ActOption> actOptions = new List<ActOption>();
        foreach (var enemy in enemies)
        {
            actOptions.AddRange(enemy.ActOptions);
        }

        battleActUI = GetComponent<BattleActUI>();
        battleActUI.UpdateItems(actOptions);

        battleEnemyUI = GetComponent<BattleEnemiesUI>();
        battleEnemyUI.UpdateItems(enemies);

        state = BattleState.Start;

        // if the enemy has narrator lines
        if (enemies[0].NarratorDialogue.Count > 0)
        {
            boxText.style.display = DisplayStyle.Flex;
            // start coroutine to display the narrator lines
            StartCoroutine(ShowNarratorLines(enemies[0].NarratorDialogue));
        }
        else
        {
            SetupBattle();
        }
    }

    //narrator coroutine
    IEnumerator ShowNarratorLines(List<string> lines)
    {
        foreach (var line in lines)
        {
            boxText.text = line;
            yield return new WaitForSeconds(2.0f);
        }

        boxText.style.display = DisplayStyle.None;

        if (enemies[0].StartingDialogue.Count > 0)
        {
            GameObject enemyObject = instantiatedEnemies[0]; // Get the first enemy instance
            Transform bubbleTransform = enemyObject.transform.Find("Bubble");
            Transform textTransform = bubbleTransform.Find("Text");
            TextMeshPro textComponent = textTransform.GetComponent<TextMeshPro>();

            // show enemy prefab text bubble
            bubbleTransform.gameObject.SetActive(true);

            foreach (var line in enemies[0].StartingDialogue)
            {
                //change text bubble text to line
                textComponent.text = line;
                yield return new WaitForSeconds(2.0f);
            }

            bubbleTransform.gameObject.SetActive(false);
            textComponent.text = ""; // Clear the text
        }

        ClearBox();
        SetupBattle();
    }

    void LayoutEnemies()
    {
        int enemyCount = enemies.Count;
        if (enemyCount == 0) return;

        // Clear the list in case LayoutEnemies() is called again
        instantiatedEnemies.Clear();

        // Calculate spacing similar to justify-content-around
        float totalSpacing = spawnAreaWidth - (enemyCount * enemyScale);
        float spacingBetweenEnemies = totalSpacing / (enemyCount + 1);

        // Start at the leftmost point of the spawn area
        float startX = -(spawnAreaWidth / 2) + spacingBetweenEnemies + (enemyScale / 2);

        for (int i = 0; i < enemyCount; i++)
        {
            // Instantiate each enemy
            GameObject enemyObject = Instantiate(enemyPrefab);
            enemyObject.transform.localScale = Vector3.one * enemyScale; // Scale each enemy

            // Position each enemy with equal spacing across the spawn area
            float positionX = startX + i * (enemyScale + spacingBetweenEnemies);
            enemyObject.transform.position = new Vector3(positionX, yOffset, 0);

            // Assign data to the enemy
            EnemyController enemyController = enemyObject.GetComponent<EnemyController>();
            enemyController.SetEnemyData(enemies[i]);

            // Add the instantiated enemy to the list
            instantiatedEnemies.Add(enemyObject);
        }
    }

    void SetupBattle()
    {
        // Start the battle
        state = BattleState.PlayerTurn;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        // Enable buttons for player actions
    }

    void OnFightButton()
    {
        if (state != BattleState.PlayerTurn) return;
        // Implement Fight logic
        ClearBox();
        enemyPage.style.display = DisplayStyle.Flex;
    }

    void OnActButton()
    {
        if (state != BattleState.PlayerTurn) return;

        ClearBox();
        actPage.style.display = DisplayStyle.Flex;
    }

    void OnItemButton()
    {
        if (state != BattleState.PlayerTurn) return;

        ClearBox();
        itemsPage.style.display = DisplayStyle.Flex;
    }

    void OnMercyButton()
    {
        if (state != BattleState.PlayerTurn) return;
        state = BattleState.EnemyTurn;
        // Implement Mercy logic
    }

    void EnemyTurn()
    {
        // Implement Enemy turn logic
    }

    public void OnEnemySelected()
    {
        ClearBox(); // Clear box content

        //change box sprite to radiant
        boxScaler.SetSprite(radiantSprite); // Set the new sprite

        // Spawn the Line from the prefab
        GameObject lineObject = Instantiate(linePrefab);
        LinePositionTracker lineTracker = lineObject.GetComponent<LinePositionTracker>();
        lineTracker.OnLineCoroutineComplete += SwitchToEnemyTurn; // Listen for coroutine completion
    }

    private void SwitchToEnemyTurn()
    {
        // set box sprite to none
        boxScaler.SetSprite(null);

        state = BattleState.EnemyTurn;
        EnemyTurn();
    }

    void ClearBox()
    {
        itemsPage.style.display = DisplayStyle.None;
        actPage.style.display = DisplayStyle.None;
        enemyPage.style.display = DisplayStyle.None;
        boxText.style.display = DisplayStyle.None;
    }
}
