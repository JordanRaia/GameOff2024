using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System; // Ensure this line is present and outside any namespace or class

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
    [SerializeField] private GameObject playerPrefab; // Reference to the player prefab
    [SerializeField] private GameObject slashEffect; // Reference to the slash effect prefab

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
    private VisualElement enemyActPage; // Visual element for enemy act options page

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
    private BattleActEnemyUI battleActEnemyUI; // UI component for displaying enemies before act options

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

    private Coroutine bulletHellCoroutine; // Reference to the bullet hell coroutine
    private GameObject playerInstance;     // Reference to the instantiated player
    private BulletHellPattern currentBulletHellPattern; // Add this field

    // Store original box size
    private float originalWidthPercentage = 0.99f;
    private float originalHeightPercentage = 0.35f;

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
        enemyActPage = root.Q<VisualElement>("EnemyActPage");

        fightButton.clicked += OnFightButton;
        actButton.clicked += OnActButton;
        itemButton.clicked += OnItemButton;
        mercyButton.clicked += OnMercyButton;

        // Spawn enemies
        LayoutEnemies(); // Layout the enemies in the battle scene

        // Initialize BattleItemUI and update items
        UpdateUI();

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

    private void UpdateUI()
    {
        battleItemUI = GetComponent<BattleItemUI>();
        battleItemUI.UpdateItems(player.GetItemsByType(ItemType.Health));

        battleEnemyUI = GetComponent<BattleEnemiesUI>();
        battleEnemyUI.UpdateItems(enemies);

        battleActEnemyUI = GetComponent<BattleActEnemyUI>();
        battleActEnemyUI.UpdateItems(enemies);
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

    IEnumerator TypeEnemyText(TextMeshPro textComponent, string text, float speed, Action onComplete)
    {
        textComponent.text = ""; // Clear the enemy bubble text
        textComponent.gameObject.SetActive(true); // Make sure the text bubble is visible

        foreach (char letter in text.ToCharArray())
        {
            textComponent.text += letter; // Add one character at a time to the enemy's text bubble
            yield return new WaitForSeconds(speed); // Wait for the specified speed interval
        }

        yield return new WaitForSeconds(1.0f); // Wait for 1 second after the full text is displayed

        onComplete?.Invoke();
    }

    IEnumerator TypePlayerText(string text, float speed)
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

    void EnemyActDialogue(int index)
    {

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

            // Subscribe to the OnEnemyDeath event
            //enemyController.OnEnemyDeath += HandleEnemyDeath;

            // Set Health Bar reference
            HealthBar hb = enemyObject.GetComponentInChildren<HealthBar>();
            if (hb != null)
            {
                enemies[i].SetHealthBar(hb);
            }

            // Add the instantiated enemy to the list
            instantiatedEnemies.Add(enemyObject);
        }
    }

    private IEnumerator HandleEnemyDeath(EnemyController deadEnemy, int index)
    {
        ClearBox();

        yield return StartCoroutine(deadEnemy.Die());

        // Remove the enemy from the list
        BattleEnemy enemyData = deadEnemy.enemyData;
        enemies.Remove(enemies[index]);

        // Remove from instantiatedEnemies list
        instantiatedEnemies.Remove(instantiatedEnemies[index]);

        // Remove from UI Components
        UpdateUI();

        // Check if all enemies are defeated
        if (enemies.Count == 0)
        {
            EndBattle();
        }
        else
        {
            // Handle the next enemy's turn
            SwitchToEnemyTurn();
        }
    }

    private void EndBattle()
    {
        Debug.Log("All enemies defeated! Battle ended.");
        // Implement battle end logic, such as rewarding the player, transitioning scenes, etc.
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
        selectedEnemyIndex = -1; // Reset the selected enemy index
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
        enemyActPage.style.display = DisplayStyle.Flex;
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

        //check each enemy's mercy chance for lowest chance
        float lowestMercyChance = 1.0f;

        foreach (var enemy in enemies)
        {
            if (enemy.MercyChance < lowestMercyChance)
            {
                lowestMercyChance = enemy.MercyChance;
            }
        }

        //get random number to see if mercy is successful
        float randomValue = UnityEngine.Random.value;
        if (randomValue <= lowestMercyChance)
        {
            //TODO implement mercy logic
            Debug.Log("Mercy successful!");
        }
        else
        {
            StartCoroutine(OnMercySelected());
        }
    }

    public IEnumerator OnMercySelected()
    {
        // Implement Act logic
        ClearBox(); // Clear box content

        int remainingEnemies = instantiatedEnemies.Count;

        for (int i = 0; i < instantiatedEnemies.Count; i++)
        {
            GameObject enemyObject = instantiatedEnemies[i];
            Transform bubbleTransform = enemyObject.transform.Find("Bubble");
            Transform textTransform = bubbleTransform.Find("Text");
            TextMeshPro textComponent = textTransform.GetComponent<TextMeshPro>();

            // Show enemy prefab text bubble
            bubbleTransform.gameObject.SetActive(true);

            // Start the dialogue coroutine for each enemy and decrement the counter upon completion
            StartCoroutine(TypeEnemyText(textComponent, enemies[i].MercyDialogue, 0.05f, () =>
            {
                remainingEnemies--;

                bubbleTransform.gameObject.SetActive(false);
                textComponent.text = ""; // Clear the text
            }));
        }

        // Wait until all enemy dialogues have completed
        while (remainingEnemies > 0)
        {
            yield return null;
        }

        SwitchToEnemyTurn();
        yield break;
    }

    void EnemyTurn()
    {
        // set box sprite to none
        boxScaler.SetSprite(null);

        // TODO resize box, make it change gradually
        boxScaler.ResizeBox(0.25f, 0.25f, true, OnBoxResizeComplete);

        // Remove StartCoroutine(EnemyAttack()); from here
    }

    private void OnBoxResizeComplete()
    {
        Vector3 spawnPosition = boxScaler.transform.position;
        playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        // Start the bullet hell sequence
        StartCoroutine(EnemyAttack());
    }

    IEnumerator EnemyAttack()
    {
        if (enemies.Count == 0) yield break;

        var enemy = enemies[0];
        currentBulletHellPattern = enemy.bulletHellPatterns[UnityEngine.Random.Range(0, enemy.bulletHellPatterns.Count)];

        SpiralPattern spiralPattern = currentBulletHellPattern as SpiralPattern;
        if (spiralPattern != null && spiralPattern.IsStatic)
        {
            spiralPattern.Initialize(boxScaler.transform.position);
        }

        Func<Vector2> getTargetPosition = () =>
        {
            if (currentBulletHellPattern.IsStatic)
            {
                return boxScaler.transform.position;
            }
            else
            {
                return playerInstance.transform.position;
            }
        };

        bulletHellCoroutine = StartCoroutine(currentBulletHellPattern.ExecutePattern(getTargetPosition));

        // Start monitoring bullets
        StartCoroutine(currentBulletHellPattern.MonitorBullets());

        yield return new WaitForSeconds(1f);

        if (bulletHellCoroutine != null)
        {
            StopCoroutine(bulletHellCoroutine);
            bulletHellCoroutine = null;
        }

        if (currentBulletHellPattern != null)
        {
            currentBulletHellPattern.StopPattern();
            currentBulletHellPattern = null;

            Destroy(playerInstance);
            playerInstance = null;
        }

        boxScaler.ResizeBox(originalWidthPercentage, originalHeightPercentage, false, OnBoxResizeBackComplete);
    }

    private void OnBoxResizeBackComplete()
    {
        // Proceed to the player's turn
        state = BattleState.PlayerTurn;
        PlayerTurn();
        DefaultLine(); // Display the enemy's default line
    }

    public void OnEnemyActSelected(int index)
    {
        if (state != BattleState.PlayerTurn) return;

        ClearBox();

        battleActUI = GetComponent<BattleActUI>();
        battleActUI.UpdateItems(enemies[index].ActOptions, index);

        actPage.style.display = DisplayStyle.Flex;
    }

    public IEnumerator OnActSelected(int index, int enemyIndex)
    {
        // Implement Act logic
        ClearBox(); // Clear box content

        //Player text
        foreach (var line in enemies[enemyIndex].ActOptions[index].playerDialogue)
        {
            // start coroutine to type out the line
            yield return StartCoroutine(TypePlayerText(line, 0.05f));
            // yield return new WaitForSeconds(2.0f); //TODO change back to 2.0f
        }

        ClearBox(); // Clear box content

        GameObject enemyObject = instantiatedEnemies[enemyIndex]; // Get the first enemy instance
        Transform bubbleTransform = enemyObject.transform.Find("Bubble");
        Transform textTransform = bubbleTransform.Find("Text");
        TextMeshPro textComponent = textTransform.GetComponent<TextMeshPro>();

        // show enemy prefab text bubble
        bubbleTransform.gameObject.SetActive(true);

        foreach (var line in enemies[enemyIndex].ActOptions[index].responses)
        {
            //change text bubble text to line
            // start coroutine to type out the line
            yield return StartCoroutine(TypeEnemyText(textComponent, line, 0.05f));
            // yield return new WaitForSeconds(2.0f); //TODO Change back to 2.0f
        }

        bubbleTransform.gameObject.SetActive(false);
        textComponent.text = ""; // Clear the text

        SwitchToEnemyTurn();
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
    }

    public void OnAttackCompleted(float percentage)
    {
        if (state != BattleState.PlayerTurn) return;

        if (selectedEnemyIndex >= 0 && selectedEnemyIndex < instantiatedEnemies.Count)
        {
            EnemyController enemyController = instantiatedEnemies[selectedEnemyIndex].GetComponent<EnemyController>();
            int damage = CalculateDamage(percentage);
            enemyController.TakeDamage(damage);

            if (enemyController.enemyData.CurrentHealth <= 0)
            {
                // Handle enemy defeat
                StartCoroutine(HandleEnemyDeath(enemyController, selectedEnemyIndex));
            }
            else
            {
                // Handle enemy attack
                SwitchToEnemyTurn();
            }
        }
        else
        {
            Debug.LogError("No enemy selected or invalid enemy index.");
        }
    }

    public int CalculateDamage(float percentage)
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
        enemyActPage.style.display = DisplayStyle.None;
        boxScaler.SetSprite(null);
    }

    // Method to spawn the slash effect at a specified position
    public void SpawnSlash()
    {
        Vector3 position = instantiatedEnemies[selectedEnemyIndex].transform.position;
        Instantiate(slashEffect, position, Quaternion.identity);
    }

    // Add this method to get the currently selected enemy
    public GameObject GetSelectedEnemy()
    {
        if (selectedEnemyIndex >= 0 && selectedEnemyIndex < instantiatedEnemies.Count)
        {
            return instantiatedEnemies[selectedEnemyIndex];
        }
        return null;
    }
}
