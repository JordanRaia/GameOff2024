using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public BattleEnemy enemyData { get; private set; }  // Public getter with a private setter

    private void Start()
    {
        if (enemyData != null)
        {
            // Initialize properties from the scriptable object
            GetComponent<SpriteRenderer>().sprite = enemyData.EnemySprite;
            enemyData.CurrentHealth = enemyData.MaxHealth;  // Initialize the enemy's health
        }
    }

    public void SetEnemyData(BattleEnemy data)
    {
        enemyData = Instantiate(data);  // Create a unique instance to prevent shared state
        InitializeEnemy();
    }

    private void InitializeEnemy()
    {
        // Set the enemy sprite
        GetComponent<SpriteRenderer>().sprite = enemyData.EnemySprite;
        enemyData.CurrentHealth = enemyData.MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        enemyData.CurrentHealth -= damage - enemyData.Defense;
        if (enemyData.CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"Enemy {enemyData.EnemyName} defeated!");
        // Reward player with experience, gold, and possibly drop an item
        // Additional actions such as destroying the GameObject or playing animations
    }
}
