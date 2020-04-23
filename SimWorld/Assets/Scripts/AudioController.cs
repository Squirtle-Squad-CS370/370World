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
        
        //Used for developing while listening to music.
        //AudioListener.volume = 0;
    }
    
    private void Update()
    {
        //mute/unmute
        if (Input.GetKeyDown("m"))
        {
            if (AudioListener.volume != 0)
            {
                AudioListener.volume = 0;
            }
            else
            {
                AudioListener.volume = 1;
            }
        }
    }

    /*
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
    */

    // Parameters with default values are optional
    public void PlaySound( AudioClip audioClip, Vector3 position = default, bool pitchShift = false )
    {
        // Make sure we have have our clip set up
        if (audioClip == null)
        {
            Debug.LogError("AudioController - No audioClip assigned");
            return;
        }

        // Create GameObject for sound, set its parent, and add AudioSource component
        GameObject sound_go = new GameObject("Sound");
        sound_go.transform.SetParent(Instance.transform);
        AudioSource audioSource = sound_go.AddComponent<AudioSource>();

        // If we have enabled pitch shifting... do that
        if( pitchShift == true )
        {
            audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        }

        // If we have provided a position, play in world space
        if( position != default )
        {
            sound_go.transform.position = position;
            audioSource.clip = audioClip;
            audioSource.maxDistance = 100f;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.dopplerLevel = 0f;
            audioSource.Play();
        }
        // Otherwise just play it outright
        else
        {
            audioSource.PlayOneShot(audioClip);
        }

        // Destroy the sound GameObject now that we are done
        Destroy(sound_go, audioClip.length);
    }
}
