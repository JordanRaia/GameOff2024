using UnityEngine;
using System;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public event Action<EnemyController> OnEnemyDeath; // Event to notify when the enemy dies

    public BattleEnemy enemyData { get; private set; }  // Public getter with a private setter

    [SerializeField] private HealthBar healthBar; // Reference to the Health Bar

    // Public property to access the HealthBar
    public HealthBar HealthBar => healthBar;

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

        Animator animator = GetComponent<Animator>();
        if (animator != null && enemyData.AnimatorController != null)
        {
            // Assign the Animator Controller from BattleEnemy
            animator.runtimeAnimatorController = enemyData.AnimatorController;
        }
    }

    public void TakeDamage(int damage)
    {
        bool isDead = enemyData.TakeDamage(damage);
        // if (isDead)
        // {
        //     StartCoroutine(Die());
        // }
    }

    public IEnumerator Die()
    {
        // Trigger death animation
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Death"); // Assumes a "Death" trigger is set up in the Animator Controller
        }

        OnEnemyDeath?.Invoke(this); // Invoke the death event

        yield return new WaitForSeconds(3f);

        Destroy(gameObject); // Destroy after 3 seconds to allow animation to play
    }
}
