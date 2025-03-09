using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestPanelUI : MonoBehaviour
{
    public Transform questListContainer;
    public GameObject panel;
    public GameObject questItemPrefab;
    public GameObject questDetailsPanel;
    public Text questTitleText;
    public Text questDescriptionText;
    public Button startQuestButton;

    private bool isToggledOpen = false;

    private void Start()
    {
        PopulateQuestPanel();
        SubscribeToQuestUpdates();
        questDetailsPanel.SetActive(false);
    }

    private void OnEnable() => SubscribeToQuestUpdates();

    private void OnDisable() => UnsubscribeFromQuestUpdates();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isToggledOpen = !isToggledOpen;
            panel.SetActive(isToggledOpen);
            Cursor.visible = isToggledOpen;
            Cursor.lockState = isToggledOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            panel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Input.GetKeyUp(KeyCode.Tab) && !isToggledOpen)
        {
            panel.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void SubscribeToQuestUpdates()
    {
        if (MainQuestManager.instance != null)
        {
            MainQuestManager.instance.OnQuestUpdated += PopulateQuestPanel;
        }
    }

    private void UnsubscribeFromQuestUpdates()
    {
        if (MainQuestManager.instance != null)
        {
            MainQuestManager.instance.OnQuestUpdated -= PopulateQuestPanel;
        }
    }

    public void PopulateQuestPanel()
    {
        if (MainQuestManager.instance == null)
        {
            Debug.LogWarning("MainQuestManager not found!");
            return;
        }

        foreach (Transform child in questListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var quest in MainQuestManager.instance.quests)
        {
            GameObject questItem = Instantiate(questItemPrefab, questListContainer);

            TMP_Text questText = questItem.GetComponentInChildren<TMP_Text>();
            if (questText == null)
            {
                Debug.LogError("QuestItemPrefab is missing a TMP_Text component!");
                return;
            }

            string progressText = quest.goal > 1 ? $" ({quest.progress}/{quest.goal})" : "";
            questText.text = $"{quest.questName} - {quest.state}{progressText}";

            Button questButton = questItem.GetComponentInChildren<Button>();
            if (questButton != null)
            {
                questButton.onClick.RemoveAllListeners();
                questButton.onClick.AddListener(() => ShowQuestDetails(quest));
            }
        }
    }

    private void ShowQuestDetails(MainQuestManager.Quest quest)
    {
        panel.SetActive(false);
        questDetailsPanel.SetActive(true);
        questTitleText.text = quest.questName;
        questDescriptionText.text = quest.description;

        startQuestButton.onClick.RemoveAllListeners();
        startQuestButton.onClick.AddListener(() =>
        {
            StartQuest(quest);
            questDetailsPanel.SetActive(false);
            panel.SetActive(true);
        });
    }

    private void StartQuest(MainQuestManager.Quest quest)
    {
        Debug.Log($"Started quest: {quest.questName}");
        quest.state = MainQuestManager.QuestState.InProgress;
        PopulateQuestPanel();
    }

    public void ShowQuestDetailsButton(int questIndex)
    {
        if (MainQuestManager.instance == null || questIndex < 0 || questIndex >= MainQuestManager.instance.quests.Count)
        {
            Debug.LogWarning("Invalid quest index or MainQuestManager not found!");
            return;
        }

        var quest = MainQuestManager.instance.quests[questIndex];
        ShowQuestDetails(quest);
    }
}
