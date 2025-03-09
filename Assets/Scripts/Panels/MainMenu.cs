using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void PlayGame() 
   {
      Time.timeScale = 1f; 
      SceneManager.LoadSceneAsync(1);

      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
   }

   public void QuitGame()
   {
      Debug.Log("Quitting game...");
      Application.Quit();
   }
}
