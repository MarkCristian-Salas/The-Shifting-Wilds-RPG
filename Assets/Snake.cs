using UnityEngine;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    public GameObject panel; // Reference to the UI panel

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (panel != null)
        {
            panel.SetActive(false); // Ensure the panel is initially hidden
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (panel != null)
            {
                panel.SetActive(!panel.activeSelf); // Toggle the panel's visibility
            }
        }
    }
}
