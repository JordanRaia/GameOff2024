using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string dialogueText;
    public bool hasChoices; // Set to true if there are choices
    public string choice1Text;
    public string choice2Text;
    public Dialogue nextDialogueChoice1;
    public Dialogue nextDialogueChoice2;
    public Dialogue nextDialogue; // For linear progression if there are no choices
}
