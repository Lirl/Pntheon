using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySound : MonoBehaviour {

    public string sound;

	// Use this for initialization
	void Start () {

        GetComponent<Button>().onClick.AddListener(Play);

    }

    private void Play() {
        AudioManager.Instance.Play(sound);
    }
}
