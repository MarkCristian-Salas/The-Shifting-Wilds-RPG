// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;
// using System.Collections.Generic;

// public class MainQuestTrigger : MonoBehaviour
// {
//     public static MainQuestTrigger instance;
//     public Text questText; 

//     private bool questStarted = false;
//     private bool playerNearby = false;

//     // Dictionary for dynamic quest management
//     private Dictionary<string, string> questMessages = new Dictionary<string, string>()
//     {
//         { "A Whisper on the Wind", "Quest Started: Inscription of the Past" },
//         { "Echoes of Silence", "Quest Started: Echoes of Silence" },
//         { "Gift of the Forest", "Quest Started: Collect Mushrooms" },
//         { "The Forest’s Cry", "Quest Started: Verdancia's Prophecy" },
//         { "The Parched Land", "Quest Started: Hidden Water Source" },
//         { "The Weakened Brave", "Quest Started: Find Medicinal Herbs" },
//         { "A Lost Cub", "Quest Started: Help the Lost Cub" }
//     };

//     private void Awake()
//     {
//         if (instance == null)
//         {
//             instance = this;
//         }
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (!questStarted && other.CompareTag("Player"))
//         {
//             playerNearby = true;
//             ShowQuestText("Press 'E' to start quest");
//         }
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             playerNearby = false;
//             questText.enabled = false;
//         }
//     }

//     private void Update()
//     {
//         if (playerNearby && Input.GetKeyDown(KeyCode.E))
//         {
//             StartQuest("The Parched Land"); 
//         }
//     }

//     // Unified method to start quests
//     public void StartQuest(string questName)
//     {
//         if (!questStarted && questMessages.ContainsKey(questName))
//         {
//             questStarted = true;
//             MainQuestManager.instance.StartQuest(questName);
//             StartCoroutine(FlashQuestText(questMessages[questName]));
//         }
//     }

//     public void CompleteQuest(string questName)
//     {
//         MainQuestManager.instance.CompleteQuest(questName);
//         StartCoroutine(FlashQuestText("Quest Completed!"));
//     }

//     // UI Display Logic
//     private IEnumerator FlashQuestText(string message)
//     {
//         questText.text = message;
//         questText.enabled = true;
//         yield return new WaitForSeconds(2);
//         questText.enabled = false;
//     }

//     // Show brief text prompt near the player
//     private void ShowQuestText(string message)
//     {
//         questText.text = message;
//         questText.enabled = true;
//     }
// }
