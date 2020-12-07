using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public static AudioClip clickSound;
    static AudioSource audioSrc;

    private void Awake()
    {
        clickSound = Resources.Load<AudioClip>("Sounds/Switch");
        audioSrc = GetComponent<AudioSource>();
        audioSrc.clip = clickSound;
        audioSrc.volume = 2.0f;
    }

    public static void PlayButton()
    {
        audioSrc.Play();
    }
}
