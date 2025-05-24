using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    private AudioSource audioSource;

    private void Awake()
    {
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
}