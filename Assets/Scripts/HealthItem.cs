using UnityEngine;

[CreateAssetMenu(fileName = "New Health Item", menuName = "Inventory/Health Item")]
public class HealthItem : ScriptableObject
{
    public string itemName;
    public int healAmount;
}