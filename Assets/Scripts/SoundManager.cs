using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static AudioClip engineSound;
    static AudioSource audioSrc;

    // Start is called before the first frame update
    void Start()
    {
        engineSound = Resources.Load<AudioClip>("Sounds/Engine");
        audioSrc = GetComponent<AudioSource>();
        audioSrc.clip = engineSound;
    }

    public static void playSound(float volume)
    {
        audioSrc.volume = volume * 0.4f;
        if (!audioSrc.isPlaying)
            audioSrc.Play();
    }
   
    
}
