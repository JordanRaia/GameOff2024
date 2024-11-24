using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public Item item; // Reference to the item scriptable object
    private bool isPlayerInRange = false;
    public string ID; // Unique identifier for the item

    void Start()
    {
        // Add a CircleCollider2D component if not already present
        if (GetComponent<CircleCollider2D>() == null)
        {
            gameObject.AddComponent<CircleCollider2D>().isTrigger = true;
        }

        if (string.IsNullOrEmpty(ID))
        {
            ID = System.Guid.NewGuid().ToString();
        }

        if (WorldManager.Instance.PickedUpItems.Contains(ID))
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            WorldManager.Instance.AddItemToInventory(item, ID); // Pass the ID
            WorldManager.Instance.PickedUpItems.Add(ID);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
