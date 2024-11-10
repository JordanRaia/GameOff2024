using UnityEngine;

public class BoxScaler : MonoBehaviour
{
    [SerializeField] private float widthPercentage = 0.99f; // 99% width
    [SerializeField] private float heightPercentage = 0.35f; // 35% height
    [SerializeField] private float yOffsetPercentage = -0.25f; // Y offset as a percentage of screen height
    [SerializeField] private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    [SerializeField] private GameObject topBorder, bottomBorder, leftBorder, rightBorder; // Border GameObjects
    [SerializeField] private float borderThicknessPixels = 5f; // Desired border thickness in pixels

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
        float targetWidth = screenWorldWidth * widthPercentage;
        float targetHeight = screenWorldHeight * heightPercentage;

        // Adjust the scale of the GameObject to match the calculated width and height
        transform.localScale = new Vector3(targetWidth, targetHeight, 1);

        // Calculate the Y position based on the offset percentage
        float yPosition = screenWorldHeight * yOffsetPercentage;

        // Set the position, keeping it centered on the X-axis and applying the Y position
        transform.position = new Vector3(0, yPosition, 0);

        // Adjust border positions and sizes
        AdjustBorders(targetWidth, targetHeight);
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
