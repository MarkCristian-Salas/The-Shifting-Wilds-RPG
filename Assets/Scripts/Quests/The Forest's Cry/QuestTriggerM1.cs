using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestTriggerM1 : MonoBehaviour
{
    public static QuestTriggerM1 instance;
    public Text questText; 

    private bool questStarted = false;

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
            MainQuestManager.instance.StartQuest("The Forest’s Cry");
            StartCoroutine(FlashQuestText("Quest Started: Verdancia's Prophecy")); 
        }
    }

    public void CompleteQuest()
    {
        StartCoroutine(FlashQuestText("Quest Completed!"));
    }

    private IEnumerator FlashQuestText(string message)
    {
        questText.text = message;
        questText.enabled = true;
        yield return new WaitForSeconds(2); 
        questText.enabled = false;
    }
}
