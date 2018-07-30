using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {
    //public AudioMixer audiomix;
    public Sound[] sounds;
    public static AudioManager Instance;
    public Slider slider;

    void Awake() {
        Instance = this;
        foreach (Sound s in sounds) {
            s.src = gameObject.AddComponent<AudioSource>();
            s.src.clip = s.clip;

            s.src.volume = s.volume;
            s.src.pitch = s.pitch;
        }
    }

    public void Play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        float lastVolume = s.src.volume;
        Debug.LogError(lastVolume + " last");
        s.src.volume = s.src.volume * slider.value;
        Debug.LogError(s.src.volume + " after change");
        s.src.Play();
        changeVolume(s, lastVolume);
       
        //Debug.Log("Played Sound");
    }

    public void PlayFor(string name, float time) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.src.PlayScheduled(time);
    }

    public void changeVolume(Sound s, float volume) {
         s.src.volume = volume;
    }
    /*
    public void setVolumeBack (float volume) {
        audiomix.SetFloat("volume", volume);
        foreach (Sound s in sounds) {
            s.src = gameObject.AddComponent<AudioSource>();
            s.src.clip = s.clip;

            s.src.volume = s.volume * volume;
            s.src.pitch = s.pitch;
        }
    }*/
}
