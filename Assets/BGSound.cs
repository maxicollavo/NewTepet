using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGSound : MonoBehaviour
{
    public ScenesManager scenesManager;
    public static BGSound instance;
    [SerializeField] AudioSource bgAudio;
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
    public void PlayBG()
    {
        if (!bgAudio.isPlaying)
        {
            bgAudio.Play();
        }
    }   
}
