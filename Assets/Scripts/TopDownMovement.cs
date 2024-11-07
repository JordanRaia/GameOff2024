using UnityEngine;

public class TopDownMovement : MonoBehaviour
{
    public static TopDownMovement Instance; // Singleton instance

    public float moveSpeed;
    private Rigidbody2D rb2d;
    private Vector2 moveInput;
    private bool controlsEnabled = true; // Controls enabled by default

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (controlsEnabled)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();
        }
        else
        {
            moveInput = Vector2.zero; // No movement input if controls are disabled
        }

        rb2d.linearVelocity = moveInput * moveSpeed;
    }

    // Method to disable controls
    public void DisableControls()
    {
        controlsEnabled = false;
    }

    // Method to enable controls
    public void EnableControls()
    {
        controlsEnabled = true;
    }
}
