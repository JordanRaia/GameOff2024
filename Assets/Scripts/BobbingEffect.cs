using UnityEngine;

public class BobbingEffect : MonoBehaviour
{
    public float bobbingHeight = 0.05f; // How high it moves
    public float bobbingSpeed = 2f;    // How fast it moves

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position; // Store the original position
    }

    void Update()
    {
        // Create a bobbing effect
        transform.position = new Vector3(
            originalPosition.x,
            originalPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight,
            originalPosition.z);
    }
}
