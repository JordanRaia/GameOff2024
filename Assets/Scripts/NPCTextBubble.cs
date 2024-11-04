using UnityEngine;

public class NPCTextBubble : MonoBehaviour
{
    private GameObject textBubble; // Reference to the text bubble Canvas

    private void Start()
    {
        // Find the text bubble GameObject by name
        textBubble = transform.Find("TextBubble")?.gameObject;
        if (textBubble != null)
        {
            textBubble.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textBubble.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textBubble.SetActive(false);
        }
    }
}
