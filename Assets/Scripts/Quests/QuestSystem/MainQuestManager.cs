using UnityEngine;
using System.Collections.Generic;

public class MainQuestManager : MonoBehaviour
{
    public static MainQuestManager instance;

    public enum QuestState { NotStarted, InProgress, Completed }

    [System.Serializable]
    public class Quest
    {
        public string questName;
        public string description; 
        public QuestState state;
        public int progress;
        public int goal;

        public Quest(string name, string description, int goal = 1)
        {
            questName = name;
            this.description = description; 
            state = QuestState.NotStarted;
            progress = 0;
            this.goal = goal;
        }
    }

    private string saveKey = "QuestData";

    public List<Quest> quests = new List<Quest>();

    public System.Action OnQuestUpdated;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeQuests();
            LoadQuests();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeQuests()
    {
        quests = new List<Quest>()
        {
            new Quest("A Lost Cub", "Find the bear statue. Complete the quest and it grants you a gift."),
            new Quest("A Whisper on the Wind", "Listen to the whispers of the ancient stone."),
            new Quest("Echoes of Silence", "There is something in the maze that you need to find that gives you a gift."),
            new Quest("Gift of the Forest", "Collect 5 mushrooms.", 5),
            new Quest("The Forest’s Cry", "Find the statue near the tree of life and reveal the prophecy."),
            new Quest("The Parched Land", "Talk to the camel in the boundary of the desert and find out what is wrong."),
            new Quest("The Weakened Brave", "Talk to the tiger in the bottom of the mountain and help him find his strength.")
        };
    }

    public void StartQuest(string questName)
    {
        Quest quest = FindQuest(questName);
        if (quest != null && quest.state == QuestState.NotStarted)
        {
            quest.state = QuestState.InProgress;
            SaveQuests();
            OnQuestUpdated?.Invoke(); 
            Debug.Log($"{questName} started!");
        }
    }

    public void CompleteQuest(string questName)
    {
        Quest quest = FindQuest(questName);
        if (quest != null && quest.state == QuestState.InProgress)
        {
            quest.state = QuestState.Completed;
            SaveQuests();
            OnQuestUpdated?.Invoke(); 
            Debug.Log($"{questName} completed!");
        }
    }

    public void UpdateProgress(string questName, int amount = 1)
    {
        Quest quest = FindQuest(questName);
        if (quest != null && quest.state == QuestState.InProgress)
        {
            quest.progress += amount;
            if (quest.progress >= quest.goal)
            {
                CompleteQuest(questName);
            }
            SaveQuests();
            OnQuestUpdated?.Invoke(); 
        }
    }

    public QuestState GetQuestState(string questName)
    {
        Quest quest = FindQuest(questName);
        return quest != null ? quest.state : QuestState.NotStarted;
    }

    public int GetProgress(string questName)
    {
        Quest quest = FindQuest(questName);
        return quest != null ? quest.progress : 0;
    }

    public int GetGoal(string questName)
    {
        Quest quest = FindQuest(questName);
        return quest != null ? quest.goal : 0;
    }

    private Quest FindQuest(string questName)
    {
        return quests.Find(q => q.questName == questName);
    }

    private void SaveQuests()
    {
        string json = JsonUtility.ToJson(new QuestWrapper { quests = quests });
        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save();
    }

    private void LoadQuests()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            string json = PlayerPrefs.GetString(saveKey);
            QuestWrapper wrapper = JsonUtility.FromJson<QuestWrapper>(json);
            if (wrapper != null && wrapper.quests != null)
            {
                quests = wrapper.quests;
            }
        }
    }

    public void ResetAllQuests()
    {
        PlayerPrefs.DeleteKey(saveKey);
        InitializeQuests();
        SaveQuests();
        OnQuestUpdated?.Invoke(); 
    }

    [System.Serializable]
    private class QuestWrapper
    {
        public List<Quest> quests;
    }
}