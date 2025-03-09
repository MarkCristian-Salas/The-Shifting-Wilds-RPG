using UnityEngine;
using UnityEngine.UI;

public class LostCubQuest : MonoBehaviour
{
    public static LostCubQuest instance;

    public GameObject bearCub;
    public Transform motherBearLocation;

    private bool cubFollowing = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void StartQuest()
    {
        MainQuestManager.instance.StartQuest("A Lost Cub");
        Debug.Log("Quest Started: Help the lost bear cub find its mother.");
    }

    public void GuideCub()
    {
        if (MainQuestManager.instance.GetQuestState("A Lost Cub") == MainQuestManager.QuestState.InProgress && !cubFollowing)
        {
            cubFollowing = true;
            Debug.Log("GuideCub() called. Attempting to make cub follow...");
            bearCub.GetComponent<BearCub>().StartFollowing();
        }
    }

    public void CompleteQuest()
    {
        if (MainQuestManager.instance.GetQuestState("A Lost Cub") == MainQuestManager.QuestState.InProgress)
        {
            MainQuestManager.instance.CompleteQuest("A Lost Cub");
            Debug.Log("Quest Completed: The cub has been reunited with its mother!");

            QuestTriggerS4.instance.ShowQuestPanel();
        }
    }
}
