using UnityEngine;
using System.Collections;

public class LinePositionTracker : MonoBehaviour
{
    [SerializeField] private Transform boxTransform; // Reference to the BoxScaler's Transform
    [SerializeField] private SpriteRenderer lineSpriteRenderer; // Reference to the line's SpriteRenderer component
    [SerializeField] private float timeToFlash = 3f; // Time in seconds to flash the line
    private MovingLine movingLine; // Reference to the MovingLine component
    private float boxWidth; // Width of the box in world units
    private bool isFlashing = false;

    public delegate void LineCoroutineComplete();
    public event LineCoroutineComplete OnLineCoroutineComplete;

    void Start()
    {
        if (boxTransform == null)
        {
            // Attempt to find the Box object in the scene by name
            GameObject boxObject = GameObject.Find("Box/Box");
            if (boxObject != null)
            {
                boxTransform = boxObject.transform;
            }
            else
            {
                Debug.LogError("Box Transform not found in the scene.");
                return;
            }
        }

        // Calculate the box width by getting its X scale (assuming the BoxScaler script sets the scale appropriately)
        boxWidth = boxTransform.localScale.x;
        movingLine = GetComponent<MovingLine>(); // Get the MovingLine component
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isFlashing) // Check if the left mouse button is clicked and not already flashing
        {
            float clickPercentage = CalculateClickPercentage();
            Debug.Log("Click Percentage: " + clickPercentage + "%");
            StartCoroutine(FlashLineForThreeSeconds());
        }
    }

    private float CalculateClickPercentage()
    {
        // Get the current X position of the line relative to the box
        float linePosition = transform.position.x;

        // Calculate the bounds of the box in world coordinates
        float boxLeftEdge = boxTransform.position.x - (boxWidth / 2);
        float boxRightEdge = boxTransform.position.x + (boxWidth / 2);
        float boxCenter = boxTransform.position.x;

        // Calculate the relative distance from the center
        float distanceFromCenter = Mathf.Abs(linePosition - boxCenter);

        // Normalize this distance relative to half the box width to get a range from 0 (center) to 1 (edges)
        float normalizedDistance = Mathf.InverseLerp(0, boxWidth / 2, distanceFromCenter);

        // Invert to make the center 100% and edges 0%
        float percentage = (1 - normalizedDistance) * 100f;

        return percentage;
    }

    private IEnumerator FlashLineForThreeSeconds()
    {
        isFlashing = true;

        // Stop movement by setting CanMove to false
        if (movingLine != null)
        {
            movingLine.CanMove = false;
        }

        float duration = timeToFlash; // Duration of the flashing effect
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Toggle between black and white
            lineSpriteRenderer.color = lineSpriteRenderer.color == Color.black ? Color.white : Color.black;

            // Wait for a short time before toggling again
            yield return new WaitForSeconds(0.1f); // Adjust the time to control the flashing speed

            elapsedTime += 0.1f;
        }

        // Notify BattleManager after flashing is complete
        OnLineCoroutineComplete?.Invoke();

        // Destroy the line object after 3 seconds
        Destroy(gameObject);
    }
}
