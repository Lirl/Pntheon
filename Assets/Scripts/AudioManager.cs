﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {
    //public AudioMixer audiomix;
    public Sound[] sounds;
    public static AudioManager Instance;
    public float masterVolume;
    public float effectVolume;
    void Awake() {
        DontDestroyOnLoad(this);
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
        if ((name != "Theme") && (name != "Lose") && (name != "Win")) {
            s.src.volume = s.src.volume * effectVolume;
        }
     
        
        if (name == "Theme") {
            s.src.PlayOneShot(s.clip, masterVolume);
            return;
        }
        
        s.src.volume = s.src.volume * masterVolume;

        s.src.Play();
        StartCoroutine(changeVolume(s, lastVolume, 0.2f));
       
        //Debug.Log("Played Sound");
    }

    public void StopPlaying(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s.src.isPlaying) {
            s.src.Stop();
        }        
    }

    public void PlayFor(string name, float time) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.src.PlayScheduled(time);
    }

    IEnumerator changeVolume(Sound s, float volume, float delayTime) {
        yield return new WaitForSeconds(delayTime);
        s.src.volume = volume;
        
    }

    public void setVolume(float volume) {
        masterVolume = volume;
    }

    public void setEffectVolume(float volume) {
        effectVolume = volume;
    }
}
