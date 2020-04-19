using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    #region Singleton

    public static AudioController Instance { get; protected set; }

    void Awake()
    {
        // Make sure this is the only instance of AudioController
        if (Instance != null)
        {
            Debug.LogError("AudioController - Another AudioController already exists.");
            return;
        }

        Instance = this;

        // This line ensures that we will not restart music if
        // we implement a main menu and start the game from there
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    public AudioClip music;

    private void Start()
    {
        // Play theme music!
    }

    public void PlaySound(AudioClip audioClip)
    {
        // Make sure we have have our clip set up
        if( audioClip == null )
        {
            Debug.LogError("AudioController - No audioClip assigned");
            return;
        }
        GameObject sound_go = new GameObject("Sound");
        sound_go.transform.SetParent(Instance.transform);
        AudioSource audioSource = sound_go.AddComponent<AudioSource>();
        audioSource.PlayOneShot( audioClip );
        Destroy(sound_go, audioClip.length);
    }
    // Overloaded version which plays sounds in world space
    public void PlaySound(AudioClip audioClip, Vector3 position)
    {
        // Make sure we have have our clip set up
        if (audioClip == null)
        {
            Debug.LogError("AudioController - No audioClip assigned");
            return;
        }
        GameObject sound_go = new GameObject("Sound");
        sound_go.transform.SetParent(Instance.transform);
        sound_go.transform.position = position;
        AudioSource audioSource = sound_go.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.maxDistance = 100f;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0f;
        audioSource.Play();
        Destroy(sound_go, audioClip.length);
    }
}
