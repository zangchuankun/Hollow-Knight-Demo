using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Audiocontroller : MonoBehaviour
{
    public AudioMixer audioMixer;
    
    public void SetMasterAudio(float volume)
    {
        audioMixer.SetFloat("Master",volume);
    }
    public void SetMusicAudio(float volume)
    {
        audioMixer.SetFloat("Music", volume);
    }
    public void SetAcousticAudio(float volume)
    {
        audioMixer.SetFloat("Acoustic", volume);
    }
}
