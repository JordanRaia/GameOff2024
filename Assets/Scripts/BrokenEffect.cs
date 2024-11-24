
using UnityEngine;

public class BrokenEffect : MonoBehaviour
{
    [SerializeField] private float upwardForce = 2f;
    [SerializeField] private float fadeDuration = 1f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // Apply upward force
        rb.linearVelocity = new Vector2(Random.Range(-1f, 1f), upwardForce);

        // Start fading
        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color initialColor = spriteRenderer.color;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(initialColor.a, 0, elapsed / fadeDuration);
            spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}