using UnityEngine;

public class Bridge : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && MainQuestManager.instance.GetQuestState("The Weakened Brave") == MainQuestManager.QuestState.InProgress)
        {
            Debug.Log("You found the hidden water source!");
            MainQuestManager.instance.CompleteQuest("The Weakened Brave");
        }
    }
}
