using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleItemUI : MonoBehaviour
{
    public List<Item> healthItems;
    private VisualElement container; // The UI container for buttons
    private Button nextPageButton;
    private Button previousPageButton;

    private int itemsPerPage = 6;
    private int currentPage = 0;

    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        container = root.Q<VisualElement>("ItemContainer"); // Container for the item buttons
        nextPageButton = root.Q<Button>("ItemNextPageButton");
        previousPageButton = root.Q<Button>("ItemLastPageButton");

        nextPageButton.clicked += NextPage;
        previousPageButton.clicked += PreviousPage;

        UpdateUI();
    }

    void UpdateUI()
    {
        container.Clear();

        int start = currentPage * itemsPerPage;
        int end = Mathf.Min(start + itemsPerPage, healthItems.Count);

        for (int i = start; i < end; i++)
        {
            Button itemButton = new Button();
            itemButton.text = healthItems[i].ItemName;
            itemButton.AddToClassList("itemButton");

            int index = i;
            itemButton.clicked += () => OnItemClicked(index);
            container.Add(itemButton);
        }

        // Enable/disable pagination buttons based on page limits
        previousPageButton.SetEnabled(currentPage > 0);
        nextPageButton.SetEnabled(end < healthItems.Count);
    }

    public void UpdateItems(List<Item> Items)
    {
        healthItems = Items;
        UpdateUI();
    }

    void OnItemClicked(int index)
    {
        Debug.Log("Clicked on " + healthItems[index].ItemName);
    }

    void NextPage()
    {
        if ((currentPage + 1) * itemsPerPage < healthItems.Count)
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