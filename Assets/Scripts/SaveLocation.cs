using UnityEngine;

public class SaveLocation : MonoBehaviour
{
    private bool playerInRange = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Ensure the GameObject has a CircleCollider2D
        if (GetComponent<CircleCollider2D>() == null)
        {
            gameObject.AddComponent<CircleCollider2D>().isTrigger = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (WorldManager.IsUIDocumentActive())
            {
                WorldManager.DisableUIDocument();
            }
            else
            {
                WorldManager.EnableUIDocument();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            WorldManager.DisableUIDocument();
        }
    }
}
