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
    public Slider slider;
    public float masterVolume;
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
        if (name == "Theme") {
            Debug.Log("Playing main theme");
            s.src.PlayOneShot(s.clip, masterVolume);
            return;
        }
        float lastVolume = s.src.volume;
        Debug.LogError(lastVolume + " last");
        s.src.volume = s.src.volume * masterVolume;
        Debug.LogError(s.src.volume + " after change");
        s.src.Play();
        StartCoroutine(changeVolume(s, lastVolume, 0.2f));
       
        //Debug.Log("Played Sound");
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
}
