using UnityEngine;

public class BearCubTrigger : MonoBehaviour
{
    private bool isNear = false;
    private bool hasTriggered = false;

    private void Update()
    {
        if (isNear && !hasTriggered && Input.GetKeyDown(KeyCode.E))
        {
            if (MainQuestManager.instance.GetQuestState("A Lost Cub") == MainQuestManager.QuestState.InProgress)
            {
                hasTriggered = true;
                LostCubQuest.instance.GuideCub();  // Keep this since `GuideCub` handles movement logic
                Debug.Log("The bear cub is now following you!");
            }
            else
            {
                Debug.Log("You must start the quest first before guiding the cub.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = true;
            Debug.Log("Press 'E' to guide the cub.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = false;
        }
    }
}
