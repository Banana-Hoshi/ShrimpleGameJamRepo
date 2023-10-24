using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour
{
	public AudioMixer mixer;
    public void SetMusicVolume(float val) {
        mixer.SetFloat("Music", val);
    }
    public void SetSFXVolume(float val) {
        mixer.SetFloat("SFX", val);
    }
    public void SetMasterVolume(float val) {
        mixer.SetFloat("Master", val);
    }
}
