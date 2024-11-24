using UnityEngine;

public class MovingLine : MonoBehaviour
{
    [SerializeField] private BoxScaler boxScaler; // Reference to BoxScaler to get Box boundaries
    [SerializeField] private float speed = 5f;
    private float leftEdge;
    private float rightEdge;

    // Public property to control whether the line can move
    public bool CanMove { get; set; } = true;

    void Start()
    {
        if (boxScaler == null)
        {
            boxScaler = Object.FindFirstObjectByType<BoxScaler>();
        }

        if (boxScaler == null)
        {
            Debug.LogError("BoxScaler component not found! Please assign it in the Inspector.");
            return;
        }

        float boxWidth = boxScaler.transform.localScale.x;
        float boxHeight = boxScaler.transform.localScale.y;
        leftEdge = boxScaler.transform.position.x - boxWidth / 2;
        rightEdge = boxScaler.transform.position.x + boxWidth / 2;

        transform.localScale = new Vector3(transform.localScale.x, boxHeight, transform.localScale.z);
        transform.position = new Vector3(rightEdge - (transform.localScale.x / 2), boxScaler.transform.position.y, 0);
    }

    void Update()
    {
        // Only move if CanMove is true
        if (CanMove)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);

            if (transform.position.x <= leftEdge)
            {
                Destroy(gameObject); // Destroy the line when it reaches the left edge
            }
        }
    }
}
