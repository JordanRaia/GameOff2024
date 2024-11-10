using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

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

    //--- UI Management Scripts ---
    [Header("UI Management Scripts")]
    private BattleItemUI battleItemUI;  // UI component for displaying items
    private BattleActUI battleActUI;    // UI component for displaying act options
    private BattleEnemiesUI battleEnemyUI; // UI component for displaying enemies

    //--- Game Data (Player and Enemies) ---
    [Header("Game Data")]
    public Player player;              // Reference to the player object
    public List<BattleEnemy> enemies;  // List of enemies in the battle

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
        SetupBattle();
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
    }
}
