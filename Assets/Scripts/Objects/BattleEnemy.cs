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
    [SerializeField] private List<string> startingDialogue = new List<string>();  // Initial dialogue lines the enemy might say
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
    public List<ActOption> ActOptions => actOptions;
    public int MaxHealth => maxHealth;
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int Attack => attack;
    public int Defense => defense;
    public int Experience => experience;
    public int Gold => gold;
    public Item ItemDrop => itemDrop;
}
