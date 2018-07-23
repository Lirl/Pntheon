using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowUserMessage : MonoBehaviour {

    string message;
    CanvasGroup canvas;
    private TextMeshProUGUI text;
    float opacity = 0;

    bool fadeOut = false;

	// Use this for initialization
	void Start () {
        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 1;
        text = GetComponentInChildren<TextMeshProUGUI>();

        if (GameManager.Instance && GameManager.Instance.ShowMessage != null) {
            GetComponent<Image>().enabled = true;
            message = GameManager.Instance.ShowMessage;
            text.text = message;
            GameManager.Instance.ShowMessage = null;

            Invoke("SetFadeOutTrue", 2);
        } else {
            GetComponent<Image>().enabled = false;
        }
	}

    public void SetFadeOutTrue() {
        fadeOut = true;
    }
	
	// Update is called once per frame
	void Update () {
        if(fadeOut) {
            canvas.alpha -= Time.deltaTime;
        }
    }
}
