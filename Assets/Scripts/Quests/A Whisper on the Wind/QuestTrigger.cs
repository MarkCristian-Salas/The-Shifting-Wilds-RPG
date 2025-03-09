using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestTrigger : MonoBehaviour
{
    public static QuestTrigger instance;
    public Text questText; 

    private bool questStarted = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    public void CompleteQuest()
    {
        StartCoroutine(FlashQuestText("Quest Completed!"));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!questStarted && other.CompareTag("Player"))
        {
            questStarted = true;
            MainQuestManager.instance.StartQuest("A Whisper on the Wind");
            StartCoroutine(FlashQuestText("Quest Started: Inscription of the Past")); 
        }
    }

    private IEnumerator FlashQuestText(string message)
    {
        questText.text = message;
        questText.enabled = true;
        yield return new WaitForSeconds(2); 
        questText.enabled = false;
    }

}
