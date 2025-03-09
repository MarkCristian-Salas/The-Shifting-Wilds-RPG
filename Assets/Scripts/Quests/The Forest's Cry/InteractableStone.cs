using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableStone : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public GameObject panel;
    public string[] lines;
    public float textSpeed = 0.05f;
    public Text pressEText;
    public Collider blockingCollider;
    public AudioSource audioSource;
    public AudioClip[] dialogueClips;

    private int index;
    private bool isDialogueActive = false;
    private bool isDialogueCompleted = false;
    private bool isNear = false;
    private bool isTyping = false;

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
        isTyping = true;
        if (audioSource != null && dialogueClips.Length > index)
        {
            audioSource.clip = dialogueClips[index];
            audioSource.Play();
        }
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
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
            if (audioSource != null)
            {
                audioSource.Stop();
            }
            MainQuestManager.instance.CompleteQuest("The Forest’s Cry");
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
                if (audioSource != null)
                {
                    audioSource.Stop();
                }
            }
        }
    }
}
