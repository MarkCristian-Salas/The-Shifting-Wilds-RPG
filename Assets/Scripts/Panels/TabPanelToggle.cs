// using UnityEngine;

// public class TabPanelToggle : MonoBehaviour
// {
//     public GameObject panel;  
//     private bool isToggledOpen = false;  

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Tab))
//         {
//             if (!Input.GetKey(KeyCode.Tab)) 
//             {
//                 isToggledOpen = !isToggledOpen;
//                 panel.SetActive(isToggledOpen);
//             }
//         }

//         if (Input.GetKey(KeyCode.Tab))  
//         {
//             panel.SetActive(true);
//         }
//         else if (Input.GetKeyUp(KeyCode.Tab) && !isToggledOpen) 
//         {
//             panel.SetActive(false);
//         }
//     }
// }
