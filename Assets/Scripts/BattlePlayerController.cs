using UnityEngine;
using System.Collections;

public class BattlePlayerController : MonoBehaviour
{
    private Player playerData;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetPlayerData(Player data)
    {
        playerData = Instantiate(data); // Create a unique instance to avoid shared state
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet") && !isInvincible)
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                int damage = bullet.Damage;
                TakeDamage(damage);
                StartCoroutine(FlashAndInvincibility());
            }
        }
    }

    public void TakeDamage(int damage)
    {
        BattleManager.Instance.NotifyDamageTaken(damage);
    }

    private IEnumerator FlashAndInvincibility()
    {
        isInvincible = true;
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        isInvincible = false;
    }
}
