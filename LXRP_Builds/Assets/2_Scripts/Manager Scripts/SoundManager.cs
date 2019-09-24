using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    AudioSource audioSource = null;
    [SerializeField] List<AudioClip> audioClips = null;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void EVENTFall()
    {
        PlayAudio(audioClips[0], 0.4f);
    }

    public void StartBgMusic()
    {
        PlayAudio(audioClips[1]);
    }

    private void PlayAudio(AudioClip clip, float vol = 0.6f)
    {
        audioSource.clip = clip;
        audioSource.volume = vol;
        audioSource.Play();
    }
}
