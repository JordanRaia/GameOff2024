using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleActUI : MonoBehaviour
{
    public List<ActOption> actOptions;
    private VisualElement container; // The UI container for buttons
    private Button nextPageButton;
    private Button previousPageButton;

    private int itemsPerPage = 6;
    private int currentPage = 0;
    private int enemyIndex = 0;

    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        container = root.Q<VisualElement>("OptionContainer"); // Container for the item buttons
        nextPageButton = root.Q<Button>("ActNextPageButton");
        previousPageButton = root.Q<Button>("ActLastPageButton");

        nextPageButton.clicked += NextPage;
        previousPageButton.clicked += PreviousPage;

        UpdateUI();
    }

    void UpdateUI()
    {
        container.Clear();

        int start = currentPage * itemsPerPage;
        int end = Mathf.Min(start + itemsPerPage, actOptions.Count);

        for (int i = start; i < end; i++)
        {
            Button itemButton = new Button();
            itemButton.text = actOptions[i].Act;
            itemButton.AddToClassList("itemButton");

            int index = i;
            itemButton.clicked += () => OnItemClicked(index);
            container.Add(itemButton);
        }

        // Enable/disable pagination buttons based on page limits
        previousPageButton.SetEnabled(currentPage > 0);
        nextPageButton.SetEnabled(end < actOptions.Count);
    }

    public void UpdateItems(List<ActOption> Items, int enemyIndex)
    {
        actOptions = Items;
        this.enemyIndex = enemyIndex;
        UpdateUI();
    }

    void OnItemClicked(int index)
    {
        StartCoroutine(BattleManager.Instance.OnActSelected(index, enemyIndex));
    }

    void NextPage()
    {
        if ((currentPage + 1) * itemsPerPage < actOptions.Count)
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