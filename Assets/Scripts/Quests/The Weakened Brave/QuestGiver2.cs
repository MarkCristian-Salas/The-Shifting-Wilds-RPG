using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class QuestGiver2 : MonoBehaviour
{
    public string npcName = "Old Traveler";
    private bool playerNearby = false;
    private bool questAccepted = false;

    public GameObject questPanel;
    public Button questActionButton;
    public Text questDescriptionText;
    public Text questStatusText;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueTextComponent;
    public string[] dialogueLines;
    public float textSpeed = 0.05f;
    public Text pressEText;
    public Collider blockingCollider;

    private int dialogueIndex;
    private bool isDialogueActive = false;
    private bool isDialogueCompleted = false;
    private bool isNear = false;
    private bool isTyping = false;

    private void Start()
    {
        questPanel.SetActive(false);
        dialoguePanel.SetActive(false);
        pressEText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (!isDialogueActive && !isDialogueCompleted)
            {
                ShowDialoguePanel();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (isDialogueActive && Input.GetMouseButtonDown(0))
        {
            if (dialogueTextComponent.text == dialogueLines[dialogueIndex])
            {
                NextDialogueLine();
            }
            else
            {
                StopAllCoroutines();
                dialogueTextComponent.text = dialogueLines[dialogueIndex];
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            Debug.Log("Press [E] to speak with " + npcName);
            if (!isDialogueActive && !isDialogueCompleted)
            {
                pressEText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            pressEText.gameObject.SetActive(false);
            if (isDialogueActive)
            {
                dialoguePanel.SetActive(false);
                isDialogueActive = false;
                StopAllCoroutines();
                dialogueTextComponent.text = string.Empty;
            }
        }
    }

    private void ShowDialoguePanel()
    {
        dialoguePanel.SetActive(true);
        dialogueIndex = 0;
        StartCoroutine(TypeDialogueLine());
        isDialogueActive = true;
        pressEText.gameObject.SetActive(false);
    }

    IEnumerator TypeDialogueLine()
    {
        isTyping = true;
        foreach (char c in dialogueLines[dialogueIndex].ToCharArray())
        {
            dialogueTextComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
    }

    private void NextDialogueLine()
    {
        if (dialogueIndex < dialogueLines.Length - 1)
        {
            dialogueIndex++;
            dialogueTextComponent.text = string.Empty;
            StartCoroutine(TypeDialogueLine());
        }
        else
        {
            dialoguePanel.SetActive(false);
            isDialogueActive = false;
            isDialogueCompleted = true;
            if (blockingCollider != null)
            {
                blockingCollider.enabled = false;
            }
            ShowQuestPanel();
        }
    }

    public void ShowQuestPanel()
    {
        questPanel.SetActive(true);
        var questState = MainQuestManager.instance.GetQuestState("The Weakened Brave");

        if (questState == MainQuestManager.QuestState.Completed)
        {
            questDescriptionText.text = "Quest Completed: You cross the bridge!";
            questStatusText.text = "Status: Done";
            questActionButton.GetComponentInChildren<Text>().text = "Done";
            questActionButton.onClick.RemoveAllListeners();
            questActionButton.onClick.AddListener(HidePanel);
            Debug.Log("Quest panel updated to Completed state.");
        }
        else
        {
            questDescriptionText.text = "Cross the bridge and find the statue in the mountain.";
            questStatusText.text = "Status: Not Started";
            questActionButton.GetComponentInChildren<Text>().text = "Start Quest";
            questActionButton.onClick.RemoveAllListeners();
            questActionButton.onClick.AddListener(StartQuest);
            Debug.Log("Quest panel updated to Not Started state.");
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void StartQuest()
    {
        MainQuestManager.instance.StartQuest("The Weakened Brave");
        Debug.Log("Quest Started: Cross the bridge and find the statue in the mountain.");
        questPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void HidePanel()
    {
        questPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
