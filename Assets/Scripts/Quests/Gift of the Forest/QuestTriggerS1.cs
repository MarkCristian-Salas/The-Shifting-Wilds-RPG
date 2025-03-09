using UnityEngine;

public class QuestTriggerS1 : MonoBehaviour
{
    public static QuestTriggerS1 instance;

    private bool questStarted = false;
    private bool playerNearby = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!questStarted && other.CompareTag("Player"))
        {
            questStarted = true;
            MainQuestManager.instance.StartQuest("Gift of the Forest");
        }

        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            Time.timeScale = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}
