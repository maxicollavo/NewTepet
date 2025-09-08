using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGSound : MonoBehaviour
{
    public ScenesManager scenesManager;
    public static BGSound instance;
    [SerializeField] AudioSource bgAudio;
    [SerializeField] AudioSource bgAudio2;
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
        bgAudio2.Play();
    }
    public void PlayBG()
    {
        if (!bgAudio.isPlaying)
        {
            bgAudio.Play();
        }
    }   
}
