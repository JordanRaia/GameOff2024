using UnityEngine;

public enum ItemType
{
    Health,
    Weapon,
    PowerUp,
    // Add more as needed
}

public abstract class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] protected ItemType itemType;

    // Public properties for access
    public string ItemName => itemName;
    public ItemType ItemType => itemType;

    // Initialization of item type
    protected virtual void OnEnable()
    {
        // Override this method in subclasses to set the item type
    }
}
