using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public GameObject panel;
    public string[] lines;
    public float textSpeed = 0.05f;
    public Text pressEText;
    public Collider blockingCollider;

    private int index;
    private bool isDialogueActive = false;
    private bool isDialogueCompleted = false;
    private bool isNear = false;

    void Start()
    {
        textComponent.text = string.Empty;
        panel.SetActive(false);
        pressEText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isNear && Input.GetKeyDown(KeyCode.E))
        {
            if (!isDialogueActive && !isDialogueCompleted)
            {
                panel.SetActive(true);
                StartDialogue();
                isDialogueActive = true;
                pressEText.gameObject.SetActive(false);
            }
        }

        if (isDialogueActive && Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            panel.SetActive(false);
            isDialogueActive = false;
            isDialogueCompleted = true;
            if (blockingCollider != null)
            {
                blockingCollider.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDialogueCompleted)
        {
            isNear = true;
            if (!isDialogueActive)
            {
                pressEText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = false;
            pressEText.gameObject.SetActive(false);
            if (isDialogueActive)
            {
                panel.SetActive(false);
                isDialogueActive = false;
                StopAllCoroutines();
                textComponent.text = string.Empty;
            }
        }
    }
}
