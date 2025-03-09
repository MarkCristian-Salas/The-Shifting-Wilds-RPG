using UnityEngine;

public class WaterSource : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && MainQuestManager.instance.GetQuestState("The Parched Land") == MainQuestManager.QuestState.InProgress)
        {
            Debug.Log("You found the hidden water source!");
            MainQuestManager.instance.CompleteQuest("The Parched Land");
        }
    }
}
