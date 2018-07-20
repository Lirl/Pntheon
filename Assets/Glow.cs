using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Glow : MonoBehaviour {

    public Image image;
    private Color color;

    private void Start() {
        color = image.color;
        image.enabled = false;
    }

    private void Update() {
        if (image.enabled) {
            image.color = Color.Lerp(Color.white, color, Mathf.PingPong(Time.time, .5f));
        }
        
    }
}
