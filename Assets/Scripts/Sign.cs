using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Localization;

public class Sign : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private LocalizedString dialogueText;
    private bool canInteract;
    private InputAction inputAction;

    private void Start()
    {
        this.inputAction = InputSystem.actions.FindAction("Attack");
        this.inputAction.performed += ToggleDialogueBox;

        this.dialogueBox.SetActive(false);
        this.canInteract = false;
    }

    private void ToggleDialogueBox(InputAction.CallbackContext context)
    {
        if (this.canInteract)
        {
            if (this.dialogueBox.activeInHierarchy)
            {
                this.dialogueBox.SetActive(false);
            }
            this.dialogueBox.SetActive(!this.dialogueBox.activeInHierarchy);

            var uiDocument = this.dialogueBox.GetComponent<UIDocument>();
            var label = uiDocument.rootVisualElement.Q<Label>();
            label.text = this.dialogueText.GetLocalizedString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        this.canInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

    }

}
