using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class TopDownMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rb2d;
    private Stamina stamina;
    private Vector2 moveInput;
    public bool controlsEnabled = true; // Controls enabled by default
    public Animator animator;

    public static TopDownMovement Instance; // Singleton instance

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
        stamina = gameObject.GetComponent<Stamina>();
    }

    void Update()
    {
        if (controlsEnabled)
        {
            playerMovement();
        }
        else
        {
            moveInput = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetFloat("Horizontal", moveInput.x);
            animator.SetFloat("Vertical", moveInput.y);
            animator.SetFloat("Speed", moveInput.sqrMagnitude);
        }
    }
    void FixedUpdate()
    {
        if (stamina != null)
            rb2d.linearVelocity = moveInput * stamina.currentSpeed;
        else
            rb2d.linearVelocity = moveInput * moveSpeed;
    }
    private void playerMovement()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        moveInput.Normalize();
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
