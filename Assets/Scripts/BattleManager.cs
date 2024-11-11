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
    private int selectedEnemyIndex = -1; // Index of the selected enemy

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

    IEnumerator TypeText(string text, float speed)
    {
        boxText.text = ""; // Clear the text initially
        boxText.style.display = DisplayStyle.Flex;

        foreach (char letter in text.ToCharArray())
        {
            boxText.text += letter; // Add one character at a time
            yield return new WaitForSeconds(speed); // Wait for the specified speed interval
        }

        yield return new WaitForSeconds(1.0f); // Wait for 1 second after the full text is displayed
    }

    IEnumerator TypeEnemyText(TextMeshPro textComponent, string text, float speed)
    {
        textComponent.text = ""; // Clear the enemy bubble text
        textComponent.gameObject.SetActive(true); // Make sure the text bubble is visible

        foreach (char letter in text.ToCharArray())
        {
            textComponent.text += letter; // Add one character at a time to the enemy's text bubble
            yield return new WaitForSeconds(speed); // Wait for the specified speed interval
        }

        yield return new WaitForSeconds(1.0f); // Wait for 1 second after the full text is displayed
    }

    //narrator coroutine
    IEnumerator ShowNarratorLines(List<string> lines)
    {
        foreach (var line in lines)
        {
            // start coroutine to type out the line
            yield return StartCoroutine(TypeText(line, 0.05f));
            // yield return new WaitForSeconds(2.0f); //TODO change back to 2.0f
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
                // start coroutine to type out the line
                yield return StartCoroutine(TypeEnemyText(textComponent, line, 0.05f));
                // yield return new WaitForSeconds(2.0f); //TODO Change back to 2.0f
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
        DefaultLine();
        PlayerTurn();
    }

    void DefaultLine()
    {
        boxText.style.display = DisplayStyle.Flex;
        boxText.text = enemies[0].DefaultLine;
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
        // set box sprite to none
        boxScaler.SetSprite(null);

        // TODO resize box, make it change gradually
        boxScaler.ResizeBox(0.25f, 0.25f);

        // TODO Put Player prefab in middle of box


        // start bullet hell sequence        
        StartCoroutine(EnemyAttack());

    }

    IEnumerator EnemyAttack()
    {
        // Implement enemy attack logic
        // TODO Bullet Hell Sequence
        yield return new WaitForSeconds(2.0f);


        // For now, just switch back to player turn
        state = BattleState.PlayerTurn;
        PlayerTurn();
    }


    public void OnEnemySelected(int index)
    {
        selectedEnemyIndex = index;
        ClearBox(); // Clear box content

        //change box sprite to radiant
        boxScaler.SetSprite(radiantSprite); // Set the new sprite

        // Spawn the Line from the prefab
        GameObject lineObject = Instantiate(linePrefab);
        LinePositionTracker lineTracker = lineObject.GetComponent<LinePositionTracker>();

        lineTracker.OnLineCoroutineComplete += SwitchToEnemyTurn; // Listen for coroutine completion
    }

    public void OnAttackCompleted(float percentage)
    {
        if (state != BattleState.PlayerTurn) return;

        if (selectedEnemyIndex >= 0 && selectedEnemyIndex < instantiatedEnemies.Count)
        {
            EnemyController enemyController = instantiatedEnemies[selectedEnemyIndex].GetComponent<EnemyController>();
            int damage = CalculateDamage(percentage);
            enemyController.TakeDamage(damage);
            selectedEnemyIndex = -1;

            // TODO start coroutine to show enemy health and damage animation
        }
        else
        {
            Debug.LogError("No enemy selected or invalid enemy index.");
        }
    }

    private int CalculateDamage(float percentage)
    {
        int minDamage = 1;
        // set maxDamage to Player equipped weapon damage
        int maxDamage = player.EquippedWeapon.Damage;

        int damage = Mathf.RoundToInt(Mathf.Lerp(minDamage, maxDamage, percentage / 100f));
        return damage;
    }

    private void SwitchToEnemyTurn()
    {
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
