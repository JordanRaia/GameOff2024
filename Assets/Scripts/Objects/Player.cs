using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Player", menuName = "Player/Player")]
[System.Serializable]
public class Player : ScriptableObject
{
    // Health
    [Header("Health")]
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int currentHealth = 20;

    // Experience
    [Header("Experience")]
    [SerializeField] private int experience = 0;
    [SerializeField] private int level = 1;
    private int requiredExperience = 100;

    // Inventory
    [Header("Inventory")]
    [SerializeField] private int gold = 0;
    [SerializeField] private WeaponItem equippedWeapon;
    [SerializeField] private List<Item> inventory = new List<Item>();

    // Properties to access variables if needed
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int Experience => experience;
    public int Level => level;
    public int Gold => gold;
    public WeaponItem EquippedWeapon => equippedWeapon;
    public List<Item> Inventory => inventory;

    private void OnEnable()
    {
        // Initialize equippedWeapon using CreateInstance to avoid the constructor issue
        if (equippedWeapon == null)
        {
            equippedWeapon = ScriptableObject.CreateInstance<WeaponItem>();
        }
    }

    //methods
    public void AddExperience(int amount)
    {
        experience += amount;
        if (experience >= requiredExperience)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        experience -= 100;
        level++;
        requiredExperience += 50;
        maxHealth += 10;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle player death
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void AddItem(Item item)
    {
        inventory.Add(item);
    }

    public void AddItems(List<Item> items)
    {
        inventory.AddRange(items);
    }

    public void RemoveItem(Item item)
    {
        inventory.Remove(item);
    }

    public void AddGold(int amount)
    {
        gold += amount;
    }

    public void RemoveGold(int amount)
    {
        gold -= amount;
    }

    public List<Item> GetItemsByType(ItemType type)
    {
        List<Item> itemsByType = new List<Item>();

        foreach (Item item in inventory)
        {
            if (item.ItemType == type)
            {
                itemsByType.Add(item);
            }
        }

        return itemsByType;
    }
}