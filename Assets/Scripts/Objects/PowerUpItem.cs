using UnityEngine;

// Enum to define possible power-up effects
public enum PowerUpEffect
{
    SpeedBoost,
    StrengthIncrease,
    Invincibility,
    // Add other effects as needed
}

[CreateAssetMenu(fileName = "New PowerUp Item", menuName = "Player/Inventory/PowerUp Item")]
public class PowerUpItem : Item
{
    public float duration;  // Duration of the power-up effect
    public PowerUpEffect effect;  // Type of effect

    protected override void OnEnable()
    {
        base.OnEnable();
        itemType = ItemType.PowerUp;
    }
}
