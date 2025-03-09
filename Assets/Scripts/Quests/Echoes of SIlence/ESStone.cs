using UnityEngine;

public class ESStone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && MainQuestManager.instance != null)
        {
            if (MainQuestManager.instance.GetQuestState("Echoes of Silence") == MainQuestManager.QuestState.InProgress)
            {
                MainQuestManager.instance.CompleteQuest("Echoes of Silence");
                gameObject.SetActive(false); 
            }
        }
    }
}
