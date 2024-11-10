using UnityEngine;


public class Stamina : MonoBehaviour
{
    public float currentSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float currentStamina;
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaDecay;
    [SerializeField] private float staminaRegen;
    [SerializeField] private float regenTimer;
    [SerializeField] private float regenDelay;
    [SerializeField] private float idleDelay;
    private float idleTimer = 2f;
    public UnityEngine.UI.Image staminaBarImage;
    public Sprite[] staminaStages;
    private Rigidbody2D rb2d;
    private Vector2 moveInput;
    private TopDownMovement topDownMovement;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        topDownMovement = GetComponent<TopDownMovement>();
        rb2d = GetComponent<Rigidbody2D>();
        staminaBarImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (topDownMovement.controlsEnabled)
        {
        stamina();
        updateStaminaUI();
        trackVisability();
        }
    }

    private void stamina()
    {
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && rb2d.linearVelocity.magnitude > 0f)
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
    }

    private void updateStaminaUI()
    {
        float percentage = Mathf.Clamp01(currentStamina / maxStamina);
        int stageIndex = Mathf.Clamp(Mathf.FloorToInt(percentage * (staminaStages.Length - 1)), 0, staminaStages.Length - 1);
        staminaBarImage.sprite = staminaStages[stageIndex];
    }

    private void trackVisability()
    {
        if(currentStamina < maxStamina)
        {
        staminaBarImage.gameObject.SetActive(true);
        idleTimer = idleDelay;
        }
        else if (currentStamina >= maxStamina)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            staminaBarImage.gameObject.SetActive(false);
        }
    }
}
