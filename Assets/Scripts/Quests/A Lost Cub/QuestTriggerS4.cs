using UnityEngine;
using UnityEngine.UI;

public class QuestTriggerS4 : MonoBehaviour
{
    private bool playerNearby = false;
    public GameObject questPanel;
    public Button questActionButton;
    public Text questDescriptionText;
    public Text questStatusText;
    public static QuestTriggerS4 instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        questPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            Debug.Log("Press 'E' to view quest details.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ShowQuestPanel();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ShowQuestPanel()
    {
        questPanel.SetActive(true);

        var questState = MainQuestManager.instance.GetQuestState("A Lost Cub");

        if (questState == MainQuestManager.QuestState.Completed)
        {
            questDescriptionText.text = "Quest Completed: The cub has been reunited with its mother!";
            questStatusText.text = "Status: Done";
            questActionButton.GetComponentInChildren<Text>().text = "Done";
            questActionButton.onClick.RemoveAllListeners();
            questActionButton.onClick.AddListener(HidePanel);
        }
        else
        {
            questDescriptionText.text = "Help the lost bear cub find its mother.";
            questStatusText.text = "Status: Not Started";
            questActionButton.GetComponentInChildren<Text>().text = "Start Quest";
            questActionButton.onClick.RemoveAllListeners();
            questActionButton.onClick.AddListener(StartQuest);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void StartQuest()
    {
        MainQuestManager.instance.StartQuest("A Lost Cub");
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
