using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public Dialogue startingDialogue;
    private bool playerInRange = false;
    private bool dialogueActive = false;

    void Update()
    {
        if (playerInRange && !dialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            dialogueActive = true;
            TopDownMovement.Instance.DisableControls(); // Disable player controls
            DialogueManager.Instance.StartDialogue(startingDialogue);
            DialogueManager.Instance.OnDialogueEnd += EndDialogue; // Subscribe to end of dialogue event
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void EndDialogue()
    {
        dialogueActive = false;
        TopDownMovement.Instance.EnableControls(); // Enable player controls
        DialogueManager.Instance.OnDialogueEnd -= EndDialogue; // Unsubscribe from event
    }
}
