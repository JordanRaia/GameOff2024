using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // SpriteRenderer for the health bar
    [SerializeField] private Sprite[] healthSprites; // Array of 22 health sprites

    private int currentHealth;
    private int maxHealth;
    private bool isInitialized = false;

    public void SetHealth(int health, int max)
    {
        if (!isInitialized)
        {
            currentHealth = max;
            maxHealth = max;
            UpdateSprite();
            isInitialized = true;
            StartCoroutine(AnimateHealthChange(health, max));
        }
        else
        {
            StartCoroutine(AnimateHealthChange(health, max));
        }
    }

    public IEnumerator UpdateHealthBar(int newHealth, int max)
    {
        int oldHealth = currentHealth;
        currentHealth = newHealth;
        maxHealth = max;

        int healthDifference = Mathf.Abs(newHealth - oldHealth);
        float totalDuration = 1f;
        float waitTime = healthDifference > 0 ? totalDuration / healthDifference : 0.05f;

        while (oldHealth != currentHealth)
        {
            if (oldHealth < currentHealth)
            {
                oldHealth++;
            }
            else
            {
                oldHealth--;
            }

            UpdateSprite(oldHealth);
            Debug.Log(waitTime);
            yield return new WaitForSeconds(waitTime); // Adjusted to complete in 1 second
        }
    }

    public void UpdateHealth(int newHealth, int max)
    {
        StartCoroutine(UpdateHealthBar(newHealth, max));
    }

    private void UpdateSprite(int health)
    {
        int spriteIndex = Mathf.Clamp(health * 20 / maxHealth, 0, 20);
        spriteRenderer.sprite = healthSprites[spriteIndex];
    }

    private void UpdateSprite()
    {
        UpdateSprite(currentHealth);
    }
    private IEnumerator AnimateHealthChange(int targetHealth, int max) { int startHealth = currentHealth; int endHealth = targetHealth; maxHealth = max; int healthDifference = Mathf.Abs(endHealth - startHealth); float totalDuration = 1f; float waitTime = healthDifference > 0 ? totalDuration / healthDifference : totalDuration; while (startHealth != endHealth) { if (startHealth < endHealth) { startHealth++; } else { startHealth--; } UpdateSprite(startHealth); yield return new WaitForSeconds(waitTime); } currentHealth = endHealth; }
}