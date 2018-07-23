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
        } else {
            GetComponent<Image>().enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
        canvas.alpha -= Time.deltaTime;
    }
}
