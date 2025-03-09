using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("-------- Audio Source --------")]
    [SerializeField] AudioSource m_Source;
    [SerializeField] AudioClip m_SFX;

    [Header("-------- Audio Clip --------")]
    public AudioClip MenuAudio;
    public AudioClip ClickSound;

}
