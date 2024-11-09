using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class TopDownMovement : MonoBehaviour
{
    [SerializeField] private float currentSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float currentStamina;
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaDecay;
    [SerializeField] private float staminaRegen;
    [SerializeField] private float regenTimer;
    [SerializeField] private float regenDelay;
    public UnityEngine.UI.Image staminaBarImage;
    public Sprite[] staminaStages;
    private Rigidbody2D rb2d;
    private Vector2 moveInput;
    private bool controlsEnabled = true; // Controls enabled by default

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
    }
    void FixedUpdate()
    {
        rb2d.linearVelocity = moveInput * currentSpeed;
    }
    private void playerMovement()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        moveInput.Normalize();

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
        {
            regenTimer = regenDelay;
            currentSpeed = moveSpeed * 1.5f;
            currentStamina -= (staminaDecay * Time.deltaTime);
        }
        else if (currentStamina == 0 && Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = moveSpeed;
            regenTimer = regenDelay;
        }
        else
        {
            currentSpeed = moveSpeed;

            if (regenTimer > 0)
                regenTimer -= Time.deltaTime;
            else
                currentStamina += staminaRegen * Time.deltaTime;

        }

        updateStaminaUI();
    }

    private void updateStaminaUI()
    {
        float percentage = Mathf.Clamp01(currentStamina / maxStamina);
        int stageIndex = Mathf.Clamp(Mathf.FloorToInt(percentage * (staminaStages.Length - 1)), 0, staminaStages.Length - 1);
        staminaBarImage.sprite = staminaStages[stageIndex];
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
