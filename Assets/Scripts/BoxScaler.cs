using UnityEngine;
using System.Collections;

public class BoxScaler : MonoBehaviour
{
    [SerializeField] private float widthPercentage = 0.99f; // 99% width
    [SerializeField] private float heightPercentage = 0.35f; // 35% height
    [SerializeField] private float yOffsetPercentage = -0.25f; // Y offset as a percentage of screen height
    [SerializeField] private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    [SerializeField] private GameObject topBorder, bottomBorder, leftBorder, rightBorder; // Border GameObjects
    [SerializeField] private float borderThicknessPixels = 5f; // Desired border thickness in pixels
    [SerializeField] private float resizeDuration = 0.5f; // Duration of the resizing animation

    private Coroutine resizeCoroutine; // Reference to the resize coroutine
    private bool forceSquare = false;

    // Public getters for width and height percentages
    public float WidthPercentage => widthPercentage;
    public float HeightPercentage => heightPercentage;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        ScaleAndPositionBoxZone();
    }

    void Update()
    {
        // This will keep updating the scale and position every frame
        ScaleAndPositionBoxZone();
    }

    public void SetWidthPercentage(float newWidthPercentage)
    {
        widthPercentage = newWidthPercentage;
    }

    public void SetHeightPercentage(float newHeightPercentage)
    {
        heightPercentage = newHeightPercentage;
    }

    public void SetYOffsetPercentage(float newYOffsetPercentage)
    {
        yOffsetPercentage = newYOffsetPercentage;
    }

    public void SetSprite(Sprite newSprite)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = newSprite;
    }

    void ScaleAndPositionBoxZone()
    {
        // Get screen width and height in world units
        float screenWorldWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        float screenWorldHeight = Camera.main.orthographicSize * 2;

        // Calculate target width and height based on percentages
        float targetWidth, targetHeight;
        if (forceSquare)
        {
            float minScreenDimension = Mathf.Min(screenWorldWidth, screenWorldHeight);
            targetWidth = targetHeight = minScreenDimension * widthPercentage;
        }
        else
        {
            targetWidth = screenWorldWidth * widthPercentage;
            targetHeight = screenWorldHeight * heightPercentage;
        }

        // Adjust the scale of the GameObject to match the calculated width and height
        transform.localScale = new Vector3(targetWidth, targetHeight, 1);

        // Calculate the Y position based on the offset percentage
        float yPosition = screenWorldHeight * yOffsetPercentage;

        // Set the position, keeping it centered on the X-axis and applying the Y position
        transform.position = new Vector3(0, yPosition, 0);

        // Adjust border positions and sizes
        AdjustBorders(targetWidth, targetHeight);
    }

    public void ResizeBox(float newWidthPercentage, float newHeightPercentage, bool forceSquare = false, System.Action onComplete = null)
    {
        if (resizeCoroutine != null)
            StopCoroutine(resizeCoroutine);

        this.forceSquare = forceSquare;
        resizeCoroutine = StartCoroutine(GradualResize(newWidthPercentage, newHeightPercentage, onComplete));
    }

    private IEnumerator GradualResize(float targetWidthPercentage, float targetHeightPercentage, System.Action onComplete)
    {
        float startWidth = widthPercentage;
        float startHeight = heightPercentage;
        float elapsedTime = 0f;

        while (elapsedTime < resizeDuration)
        {
            elapsedTime += Time.deltaTime;
            if (forceSquare)
            {
                widthPercentage = Mathf.Lerp(startWidth, targetWidthPercentage, elapsedTime / resizeDuration);
                heightPercentage = widthPercentage;
            }
            else
            {
                widthPercentage = Mathf.Lerp(startWidth, targetWidthPercentage, elapsedTime / resizeDuration);
                heightPercentage = Mathf.Lerp(startHeight, targetHeightPercentage, elapsedTime / resizeDuration);
            }

            ScaleAndPositionBoxZone();
            yield return null;
        }

        // Ensure final values are set exactly to target percentages
        widthPercentage = targetWidthPercentage;
        if (forceSquare)
            heightPercentage = widthPercentage;
        else
            heightPercentage = targetHeightPercentage;
        ScaleAndPositionBoxZone();

        if (onComplete != null)
            onComplete();
    }

    private void AdjustBorders(float boxWidth, float boxHeight)
    {
        // Convert border thickness from pixels to world units
        float borderThicknessWorld = borderThicknessPixels / Screen.height * Camera.main.orthographicSize * 2;

        // Top Border - Position exactly at the top of the box
        topBorder.transform.position = transform.position + new Vector3(0, boxHeight / 2, 0);
        topBorder.transform.localScale = new Vector3(boxWidth, borderThicknessWorld, 1);

        // Bottom Border - Position exactly at the bottom of the box
        bottomBorder.transform.position = transform.position + new Vector3(0, -boxHeight / 2, 0);
        bottomBorder.transform.localScale = new Vector3(boxWidth, borderThicknessWorld, 1);

        // Left Border - Position exactly at the left edge of the box
        leftBorder.transform.position = transform.position + new Vector3(-boxWidth / 2, 0, 0);
        leftBorder.transform.localScale = new Vector3(borderThicknessWorld, boxHeight, 1);

        // Right Border - Position exactly at the right edge of the box
        rightBorder.transform.position = transform.position + new Vector3(boxWidth / 2, 0, 0);
        rightBorder.transform.localScale = new Vector3(borderThicknessWorld, boxHeight, 1);
    }
}
