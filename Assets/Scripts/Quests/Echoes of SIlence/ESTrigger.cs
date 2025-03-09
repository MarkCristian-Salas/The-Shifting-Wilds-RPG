using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ESTrigger : MonoBehaviour
{
    public static ESTrigger instance;

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
            MainQuestManager.instance.StartQuest("Echoes of Silence");
            StartCoroutine(FlashQuestText("Quest Started: Echoes of Silence"));
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
