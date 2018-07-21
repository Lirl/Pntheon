

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {
    public TextMeshProUGUI Message;
    private bool stopped;
    private bool currentlyShowing;
    public bool IsShowing = false;

    private class TutorialMessage {
        public string text;
        public int time;
        public int fontSize;

        public TutorialMessage(string text, int time, int fontSize) {
            this.text = text;
            this.time = time;
            this.fontSize = fontSize;
        }
    }

    public static TutorialManager Instance { get; private set; }

    List<TutorialMessage> queue = new List<TutorialMessage>();
    private bool textIsShown;

    // Use this for initialization
    void Start() {
        Instance = this;
    }

    // Update is called once per frame
    void Update() {

    }

    public void ShowTutorialClassMessageInDelay() {

    }

    public void ShowMessage(string text, int time, int fontSize) {
        if (IsShowing) {
            queue.Add(new TutorialMessage(text, time, fontSize));
            return;
        }

        IsShowing = true;

        ShowMessageActual(text, time, fontSize);
    }

    private void ShowMessageActual(string text, int time, int fontSize) {
        Message.fontSize = Math.Max(fontSize, 20);
        Message.color = new Color(Message.color.r, Message.color.g, Message.color.b, 0);
        Message.text = text;
        StartCoroutine(FadeTextToFullAlpha(1f, Message));
        Invoke("FadeOut", time);
    }

    public void ShowMessageInSeconds(float delay, string text, int time, int fontSize) {
        StartCoroutine(ShowInDelay(delay, text, time, fontSize));
    }

    public IEnumerator ShowInDelay(float delay, string text, int time, int fontSize) {
        yield return new WaitForSeconds(delay);
        ShowMessage(text, time, fontSize);
    }

    public void ShowMessage(string text, int time) {
        ShowMessage(text, time, 25);
    }

    public void FadeOut() {
        StartCoroutine(FadeTextToZeroAlpha(1f, Message));
        StartCoroutine(CheckFadingOver());
    }

    public IEnumerator CheckFadingOver() {
        while (textIsShown) {
            yield return new WaitForSeconds(0.01f);
        }
        SetIsShowingToFalse();
    }

    public void SetIsShowingToFalse() {
        if(queue.Count > 0) {
            var msg = queue[0];
            queue.RemoveAt(0);
            ShowMessageActual(msg.text, msg.time, msg.fontSize);
            return;
        } else {
            IsShowing = false;
        }
    }

    public IEnumerator FadeTextToFullAlpha(float t, TextMeshProUGUI i) {
        textIsShown = true;
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f && !stopped) {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return new WaitForSeconds(0.01f);
        }
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
    }

    public IEnumerator FadeTextToZeroAlpha(float t, TextMeshProUGUI i) {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f && !stopped) {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return new WaitForSeconds(0.01f);
        }
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        textIsShown = false;
    }
}
