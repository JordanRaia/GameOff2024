using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button choice1Button;
    public Button choice2Button;
    public Button continueButton;
    public TextMeshProUGUI choice1Text;
    public TextMeshProUGUI choice2Text;
    private Dialogue currentDialogue;

    public event Action OnDialogueEnd;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        dialoguePanel.SetActive(false);
        choice1Button.gameObject.SetActive(false);
        choice2Button.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
    }

    void Update()
    {
        // Check for Space or Enter only if the continueButton is active
        //if (continueButton.gameObject.activeSelf && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        //{
        //    OnContinueSelected();
        //}
    }

    public void StartDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        dialoguePanel.SetActive(true);
        DisplayDialogue();
    }

    private void DisplayDialogue()
    {
        dialogueText.text = currentDialogue.dialogueText;

        if (currentDialogue.hasChoices)
        {
            choice1Button.gameObject.SetActive(true);
            choice2Button.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(false);
            choice1Text.text = currentDialogue.choice1Text;
            choice2Text.text = currentDialogue.choice2Text;
        }
        else
        {
            choice1Button.gameObject.SetActive(false);
            choice2Button.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(true);
        }
    }

    public void OnChoice1Selected()
    {
        if (currentDialogue.nextDialogueChoice1 != null)
        {
            StartDialogue(currentDialogue.nextDialogueChoice1);
        }
        else
        {
            EndDialogue();
        }
    }

    public void OnChoice2Selected()
    {
        if (currentDialogue.nextDialogueChoice2 != null)
        {
            StartDialogue(currentDialogue.nextDialogueChoice2);
        }
        else
        {
            EndDialogue();
        }
    }

    public void OnContinueSelected()
    {
        if (currentDialogue.nextDialogue != null)
        {
            StartDialogue(currentDialogue.nextDialogue);
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialogueText.text = "";
        choice1Button.gameObject.SetActive(false);
        choice2Button.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        dialoguePanel.SetActive(false);
        OnDialogueEnd?.Invoke();
    }
}
