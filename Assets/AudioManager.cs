using UnityEngine;
using UnityEngine.Scripting;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource backgroundMusic;

    public AudioClip background;
    

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        backgroundMusic.clip = background;
        backgroundMusic.Play();
    }

    
}