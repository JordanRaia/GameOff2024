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
    private Button fightButton;
    private Button actButton;
    private Button itemButton;
    private Button mercyButton;
    private Label boxText;
    private VisualElement dialogueBox;
    private VisualElement itemsPage;
    private VisualElement actPage;
    private VisualElement enemyPage;

    public BattleItemUI battleItemUI;
    public BattleActUI battleActUI;
    public BattleEnemiesUI battleEnemyUI;

    public PlayerInventory playerInventory;
    public List<BattleEnemy> enemies;

    private BattleState state;

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

        //temp
        playerInventory = ScriptableObject.CreateInstance<PlayerInventory>();
        AddInventoryItems();

        //temp
        enemies = new List<BattleEnemy>();
        SetupTestEnemy();

        // Initialize BattleItemUI and update items
        battleItemUI = GetComponent<BattleItemUI>();
        battleItemUI.UpdateItems(playerInventory.healthItems);

        List<ActOption> actOptions = new List<ActOption>();
        foreach (var enemy in enemies)
        {
            actOptions.AddRange(enemy.actOptions);
        }

        battleActUI = GetComponent<BattleActUI>();
        battleActUI.UpdateItems(actOptions);

        battleEnemyUI = GetComponent<BattleEnemiesUI>();
        battleEnemyUI.UpdateItems(enemies);

        state = BattleState.Start;
        SetupBattle();
    }

    private void SetupTestEnemy()
    {
        BattleEnemy enemy = ScriptableObject.CreateInstance<BattleEnemy>();

        enemy.enemyName = "Test Enemy";
        enemy.startingDialogue.Add("Test Enemy: I am a test enemy.");
        enemy.startingDialogue.Add("Test Enemy: I am here to test your skills.");
        enemy.startingDialogue.Add("Test Enemy: Prepare yourself!");

        ActOption actOption1 = ScriptableObject.CreateInstance<ActOption>();
        actOption1.Act = "Act 1";
        actOption1.responses.Add("Response 1");
        actOption1.responses.Add("Response 2");
        actOption1.responses.Add("Response 3");

        ActOption actOption2 = ScriptableObject.CreateInstance<ActOption>();
        actOption2.Act = "Act 2";
        actOption2.responses.Add("Response 1");
        actOption2.responses.Add("Response 2");
        actOption2.responses.Add("Response 3");

        enemy.actOptions.Add(actOption1);
        enemy.actOptions.Add(actOption2);

        enemies.Add(enemy);
    }

    //temp
    private void AddInventoryItems()
    {
        // Create and add 10 unique health items
        HealthItem[] healthItems = new HealthItem[10];

        healthItems[0] = CreateHealthItem("Small Health Potion", 10);
        healthItems[1] = CreateHealthItem("Medium Health Potion", 20);
        healthItems[2] = CreateHealthItem("Large Health Potion", 30);
        healthItems[3] = CreateHealthItem("Super Health Potion", 40);
        healthItems[4] = CreateHealthItem("Mega Health Potion", 50);
        healthItems[5] = CreateHealthItem("Ultra Health Potion", 60);
        healthItems[6] = CreateHealthItem("Hyper Health Potion", 70);
        healthItems[7] = CreateHealthItem("Ultimate Health Potion", 80);
        healthItems[8] = CreateHealthItem("Legendary Health Potion", 90);
        healthItems[9] = CreateHealthItem("Mythical Health Potion", 100);

        foreach (var item in healthItems)
        {
            playerInventory.healthItems.Add(item);
        }
    }

    //temp
    HealthItem CreateHealthItem(string name, int healAmount)
    {
        HealthItem healthItem = ScriptableObject.CreateInstance<HealthItem>();
        healthItem.itemName = name;
        healthItem.healAmount = healAmount;
        return healthItem;
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

    void ClearBox()
    {
        itemsPage.style.display = DisplayStyle.None;
        actPage.style.display = DisplayStyle.None;
        enemyPage.style.display = DisplayStyle.None;
    }
}
