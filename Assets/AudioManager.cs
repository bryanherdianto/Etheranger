using UnityEngine;
using UnityEngine.Scripting;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource backgroundMusic;
    public AudioClip background;

    private static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (instance == this)
        {
            backgroundMusic.clip = background;
            backgroundMusic.Play();
        }
    }
}
