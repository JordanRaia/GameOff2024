using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Player/Inventory/Weapon Item")]
public class WeaponItem : Item
{
    [SerializeField] private int damage;
    [SerializeField] private Sprite sprite;

    public int Damage => damage;
    public Sprite Sprite => sprite;

    protected override void OnEnable()
    {
        itemType = ItemType.Weapon;
    }
}
