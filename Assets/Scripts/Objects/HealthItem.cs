using UnityEngine;

[CreateAssetMenu(fileName = "New Health Item", menuName = "Player/Inventory/Health Item")]
public class HealthItem : Item
{
    [SerializeField] private int healAmount;
    [SerializeField] private Sprite sprite;

    // Public properties to access private fields
    public int HealAmount => healAmount;
    public Sprite Sprite => sprite;

    protected override void OnEnable()
    {
        base.OnEnable();
        itemType = ItemType.Health;
    }
}
