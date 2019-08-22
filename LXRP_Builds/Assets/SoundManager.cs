using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource = null;
    [SerializeField] List<AudioClip> audioClips = null;
    /// <summary>
    /// The list of audio clips should be listed as follows:
    /// 1. 
    /// </summary>

    public void EVENTFall()
    {
        audioSource.clip = audioClips[0];
        audioSource.Play();
    }
}
