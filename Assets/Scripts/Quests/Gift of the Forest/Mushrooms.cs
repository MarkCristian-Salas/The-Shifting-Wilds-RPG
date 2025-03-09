using UnityEngine;
using UnityEngine.UI;

public class Mushroom : MonoBehaviour
{
    private bool isNear = false;
    public Text interactText;

    private void Update()
    {
        if (isNear && Input.GetKeyDown(KeyCode.E))
        {
            CollectMushroom();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = true;
            ShowQuestText("Press 'E' to collect");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = false;
            HideQuestText();
        }
    }

    private void CollectMushroom()
    {
        var questState = MainQuestManager.instance.GetQuestState("Gift of the Forest");

        if (questState != MainQuestManager.QuestState.InProgress)
        {
            Debug.Log("You haven't started the quest or it's already completed.");
            return;
        }

        MainQuestManager.instance.UpdateProgress("Gift of the Forest");

        int progress = MainQuestManager.instance.GetProgress("Gift of the Forest");
        int goal = MainQuestManager.instance.GetGoal("Gift of the Forest");

        Debug.Log($"Mushroom collected! Progress: {progress}/{goal}");

        if (progress >= goal)
        {
            Debug.Log("Mushroom Collection Quest Completed!");
        }

        Destroy(gameObject);
        HideQuestText();
    }

    private void ShowQuestText(string message)
    {
        interactText.text = message;
        interactText.gameObject.SetActive(true);
    }

    private void HideQuestText()
    {
        interactText.gameObject.SetActive(false);
    }
}
