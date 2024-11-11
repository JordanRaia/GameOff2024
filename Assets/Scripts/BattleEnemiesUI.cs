using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleEnemiesUI : MonoBehaviour
{
    public List<BattleEnemy> battleEnemies;
    private VisualElement container; // The UI container for buttons
    private Button nextPageButton;
    private Button previousPageButton;

    private int itemsPerPage = 6;
    private int currentPage = 0;

    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        container = root.Q<VisualElement>("EnemyContainer"); // Container for the item buttons
        nextPageButton = root.Q<Button>("EnemyNextPageButton");
        previousPageButton = root.Q<Button>("EnemyLastPageButton");

        nextPageButton.clicked += NextPage;
        previousPageButton.clicked += PreviousPage;

        UpdateUI();
    }

    void UpdateUI()
    {
        container.Clear();

        int start = currentPage * itemsPerPage;
        int end = Mathf.Min(start + itemsPerPage, battleEnemies.Count);

        for (int i = start; i < end; i++)
        {
            Button itemButton = new Button();
            itemButton.text = battleEnemies[i].EnemyName;
            itemButton.AddToClassList("itemButton");

            int index = i;
            itemButton.clicked += () => OnItemClicked(index);
            container.Add(itemButton);
        }

        // Enable/disable pagination buttons based on page limits
        previousPageButton.SetEnabled(currentPage > 0);
        nextPageButton.SetEnabled(end < battleEnemies.Count);
    }

    public void UpdateItems(List<BattleEnemy> Items)
    {
        battleEnemies = Items;
        UpdateUI();
    }

    void OnItemClicked(int index)
    {
        BattleManager.Instance.OnEnemySelected(index); // Notify the BattleManager of enemy selection
    }

    void NextPage()
    {
        if ((currentPage + 1) * itemsPerPage < battleEnemies.Count)
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