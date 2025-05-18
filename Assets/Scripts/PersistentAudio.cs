using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    private AudioSource audioSource;
    private static AudioManager instance;

    private void Awake()
    {
        {
            // Asegura que solo haya una instancia
            if (instance != null && instance != this)
            {
                Destroy(gameObject); // Elimina duplicados
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escenas
        }
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
            LoadAllAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAllAudio()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds");
        foreach (AudioClip clip in clips)
        {
            audioClips[clip.name] = clip;
        }
    }

    public void PlaySound(string clipName)
    {
        if (audioClips.ContainsKey(clipName))
        {
            audioSource.PlayOneShot(audioClips[clipName]);
        }
        else
        {
            Debug.LogWarning($"AudioClip {clipName} no encontrado!");
        }
    }
    public void PlaySound(AudioClip clip)
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null && clip != null)
        {
            audio.clip = clip;
            audio.Play();
        }
    }

public void StopMusic()
    {
        audioSource.Stop();
    }
}
