using UnityEngine;
using System.Collections;

public class LinePositionTracker : MonoBehaviour
{
    [SerializeField] private Transform boxTransform; // Reference to the BoxScaler's Transform
    [SerializeField] private SpriteRenderer lineSpriteRenderer; // Reference to the line's SpriteRenderer component
    [SerializeField] private float timeToFlash = 3f; // Time in seconds to flash the line
    private MovingLine movingLine; // Reference to the MovingLine component
    private float boxWidth; // Width of the box in world units
    private bool isFlashing = false;
    private Coroutine enemyMovementCoroutine; // Reference to the enemy movement coroutine
    private HealthBar currentHealthBar; // Reference to the existing Health Bar

    public delegate void LineCoroutineComplete();
    public event LineCoroutineComplete OnLineCoroutineComplete;

    void Start()
    {
        if (boxTransform == null)
        {
            // Attempt to find the Box object in the scene by name
            GameObject boxObject = GameObject.Find("Box/Box");
            if (boxObject != null)
            {
                boxTransform = boxObject.transform;
            }
            else
            {
                Debug.LogError("Box Transform not found in the scene.");
                return;
            }
        }

        // Calculate the box width by getting its X scale (assuming the BoxScaler script sets the scale appropriately)
        boxWidth = boxTransform.localScale.x;
        movingLine = GetComponent<MovingLine>(); // Get the MovingLine component
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isFlashing) // Check if the left mouse button is clicked and not already flashing
        {
            float clickPercentage = CalculateClickPercentage();
            // Start the flashing coroutine and pass the clickPercentage
            StartCoroutine(FlashLineForThreeSeconds(clickPercentage));
        }
    }

    private float CalculateClickPercentage()
    {
        // Get the current X position of the line relative to the box
        float linePosition = transform.position.x;

        // Calculate the bounds of the box in world coordinates
        float boxLeftEdge = boxTransform.position.x - (boxWidth / 2);
        float boxRightEdge = boxTransform.position.x + (boxWidth / 2);
        float boxCenter = boxTransform.position.x;

        // Calculate the relative distance from the center
        float distanceFromCenter = Mathf.Abs(linePosition - boxCenter);

        // Normalize this distance relative to half the box width to get a range from 0 (center) to 1 (edges)
        float normalizedDistance = Mathf.InverseLerp(0, boxWidth / 2, distanceFromCenter);

        // Invert to make the center 100% and edges 0%
        float percentage = (1 - normalizedDistance) * 100f;

        return percentage;
    }

    private IEnumerator FlashLineForThreeSeconds(float clickPercentage)
    {
        isFlashing = true;


        // Get the selected enemy GameObject
        GameObject selectedEnemy = BattleManager.Instance.GetSelectedEnemy();
        if (selectedEnemy == null)
        {
            Debug.LogError("No enemy selected.");
            yield break;
        }

        // Get the EnemyController from the selected enemy
        EnemyController enemyController = selectedEnemy.GetComponent<EnemyController>();
        if (enemyController == null)
        {
            Debug.LogError("EnemyController component not found on selected enemy.");
            yield break;
        }

        int damage = BattleManager.Instance.CalculateDamage(clickPercentage); // Calculate the damage based on the click percentage
        int newHealth = enemyController.enemyData.CurrentHealth - damage; // Calculate the new health after taking damage

        // Reference the Health Bar via EnemyController
        currentHealthBar = enemyController.HealthBar;
        if (currentHealthBar != null)
        {
            currentHealthBar.gameObject.SetActive(true); // Enable the HealthBar
            currentHealthBar.SetHealth(newHealth, enemyController.enemyData.CurrentHealth, enemyController.enemyData.MaxHealth);
        }
        else
        {
            Debug.LogError("HealthBar not found on the selected enemy.");
        }

        // Call BattleManager to spawn the slash effect on the enemy
        BattleManager.Instance.SpawnSlash();

        // Stop movement by setting CanMove to false
        if (movingLine != null)
        {
            movingLine.CanMove = false;
        }

        // Start enemy movement
        enemyMovementCoroutine = StartCoroutine(MoveEnemyRapidly());

        float duration = timeToFlash; // Duration of the flashing effect
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Toggle between black and white
            lineSpriteRenderer.color = lineSpriteRenderer.color == Color.black ? Color.white : Color.black;

            // Wait for a short time before toggling again
            yield return new WaitForSeconds(0.1f); // Adjust the time to control the flashing speed

            elapsedTime += 0.1f;
        }

        // Update Health Bar gradually
        StartCoroutine(currentHealthBar.GetComponent<HealthBar>().UpdateHealthBar(enemyController.enemyData.CurrentHealth, enemyController.enemyData.MaxHealth));

        // Stop enemy movement
        if (enemyMovementCoroutine != null)
        {
            StopCoroutine(enemyMovementCoroutine);
            enemyMovementCoroutine = null;
        }

        // Disable the HealthBar
        if (currentHealthBar != null)
        {
            currentHealthBar.gameObject.SetActive(false);
        }

        // Apply damage after flashing completes
        BattleManager.Instance.OnAttackCompleted(clickPercentage);

        // Notify BattleManager after flashing is complete
        OnLineCoroutineComplete?.Invoke();

        // Destroy the line object after flashing and damage application
        Destroy(gameObject);

        isFlashing = false;
    }

    private IEnumerator MoveEnemyRapidly()
    {
        // Get the selected enemy from BattleManager
        GameObject enemy = BattleManager.Instance.GetSelectedEnemy();

        if (enemy == null)
            yield break;

        Transform enemyTransform = enemy.transform;
        Vector3 originalPosition = enemyTransform.position;
        float moveSpeed = 6.0f;   // Speed of movement

        while (true)
        {
            // Move Right
            float elapsed = 0f;
            while (elapsed < 0.05f)
            {
                enemyTransform.position += Vector3.right * moveSpeed * Time.deltaTime;
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Move Left
            elapsed = 0f;
            while (elapsed < 0.05f)
            {
                enemyTransform.position += Vector3.left * moveSpeed * Time.deltaTime;
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Repeat
        }
    }
}
