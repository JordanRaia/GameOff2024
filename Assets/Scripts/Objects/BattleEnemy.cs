using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Battle System/Enemy")]
public class BattleEnemy : ScriptableObject
{
    // Basic Information
    [Header("Basic Information")]
    [SerializeField] private string enemyName = "";               // Name of the enemy
    [SerializeField] private Sprite enemySprite;                  // Sprite representing the enemy

    // Dialogue and Interactions
    [Header("Dialogue and Interaction")]
    [SerializeField] private List<string> narratorDialogue = new List<string>();  // Narrator dialogue lines for the enemy
    [SerializeField] private List<string> startingDialogue = new List<string>();  // Initial dialogue lines the enemy might say
    [SerializeField] private string defaultLine = "";       // Dialogue lines for the "Act" phase of battle
    [SerializeField] private float dialogueSpeed = 0.05f;         // Speed at which dialogue is displayed
    [SerializeField] private List<ActOption> actOptions = new List<ActOption>();  // List of options available in "Act" phase of battle

    // Enemy Stats
    [Header("Stats")]
    [SerializeField] private int maxHealth = 20;                       // Maximum health of the enemy
    [SerializeField] private int currentHealth = 20;                   // Current health of the enemy
    [SerializeField] private int attack = 3;                          // Attack strength of the enemy
    [SerializeField] private int defense = 3;                         // Defense strength of the enemy

    // Rewards
    [Header("Rewards")]
    [SerializeField] private int experience = 10;                      // Experience points awarded for defeating the enemy
    [SerializeField] private int gold = 5;                            // Gold rewarded for defeating the enemy
    [SerializeField] private Item itemDrop;                       // Item dropped by the enemy upon defeat

    // Properties for accessing private variables
    public string EnemyName => enemyName;
    public Sprite EnemySprite => enemySprite;
    public List<string> StartingDialogue => startingDialogue;
    public float DialogueSpeed => dialogueSpeed;
    public List<string> NarratorDialogue => narratorDialogue;
    public string DefaultLine => defaultLine;
    public List<ActOption> ActOptions => actOptions;
    public int MaxHealth => maxHealth;
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int Attack => attack;
    public int Defense => defense;
    public int Experience => experience;
    public int Gold => gold;
    public Item ItemDrop => itemDrop;

    // Method to take damage and update health
    public void TakeDamage(int damage)
    {
        currentHealth -= damage - defense;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to handle enemy defeat
    private void Die()
    {
        Debug.Log($"Enemy {enemyName} defeated!");
        // Reward player with experience, gold, and possibly drop an item
        // Additional actions such as playing animations or destroying the GameObject
    }

    // Method to heal the enemy
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
