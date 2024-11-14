using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleActEnemyUI : MonoBehaviour
{
    public List<BattleEnemy> enemies;
    private VisualElement container; // The UI container for buttons
    private Button nextPageButton;
    private Button previousPageButton;

    private int itemsPerPage = 6;
    private int currentPage = 0;

    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        container = root.Q<VisualElement>("EnemyActContainer"); // Container for the item buttons
        nextPageButton = root.Q<Button>("EnemyActNextPageButton");
        previousPageButton = root.Q<Button>("EnemyActLastPageButton");

        nextPageButton.clicked += NextPage;
        previousPageButton.clicked += PreviousPage;

        UpdateUI();
    }

    void UpdateUI()
    {
        container.Clear();

        int start = currentPage * itemsPerPage;
        int end = Mathf.Min(start + itemsPerPage, enemies.Count);

        for (int i = start; i < end; i++)
        {
            Button itemButton = new Button();
            itemButton.text = enemies[i].EnemyName;
            itemButton.AddToClassList("itemButton");

            int index = i;
            itemButton.clicked += () => OnItemClicked(index);
            container.Add(itemButton);
        }

        // Enable/disable pagination buttons based on page limits
        previousPageButton.SetEnabled(currentPage > 0);
        nextPageButton.SetEnabled(end < enemies.Count);
    }

    public void UpdateItems(List<BattleEnemy> Items)
    {
        enemies = Items;
        UpdateUI();
    }

    void OnItemClicked(int index)
    {
        BattleManager.Instance.OnEnemyActSelected(index); // Notify the BattleManager of enemy selection
    }

    void NextPage()
    {
        if ((currentPage + 1) * itemsPerPage < enemies.Count)
        {
            currentPage++;
            UpdateUI();
        }
    }

    void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateUI();
        }
    }
}