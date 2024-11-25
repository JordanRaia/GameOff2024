using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningSceneDialogue : MonoBehaviour
{
    public Dialogue startingDialogue;
    private bool dialogueActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueActive = true;
        DialogueManager.Instance.StartDialogue(startingDialogue);
        DialogueManager.Instance.OnDialogueEnd += EndDialogue; // Subscribe to end of dialogue event
    }

    private void EndDialogue()
    {
        SceneManager.LoadScene((int)SceneIndexes.TownSquare);
    }
}
