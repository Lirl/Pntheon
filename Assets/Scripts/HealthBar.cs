using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    private Disk disk;
    private double totalHealth;
    private Image img;
    private Transform parent;

    // Use this for initialization
    void Start() {
        disk = gameObject.transform.parent.GetComponent<Disk>();
        totalHealth = disk.TotalHealth;
        img = GetComponent<Image>();
        img.enabled = false;

        parent = gameObject.transform.parent;
        gameObject.transform.parent = GameObject.FindGameObjectWithTag("Canvas").transform;
        
        // Setting health bar circle to match character alliance
        if (img) {
            img.color = new Color(1, 0, 0.3f);
            //img.color = (disk.Alliance == 1) ? new Color(0f, 0f, 160f, 0.2f) : new Color(200f, 0f, 0f, 0.2f);
        } else {
            Debug.LogError("Could not find img as a health bar");
        }

        GetComponent<RectTransform>().Rotate(new Vector3(90, 0, 0));

        img.enabled = true;
    }

    // Update is called once per frame
    void Update() {
        // Positioning 
        if(gameObject) {
            HandlePosition();
            HandleScale();
            HandleBarAccordingToHealth();
        }
    }

    private void HandleScale() { 
        // We divid by 6 because thats what makes it looks good
        if(transform) {
            transform.localScale = new Vector3(parent.transform.localScale.x / 6, parent.transform.localScale.z / 6, 10);
        }
    }

    private void HandleBarAccordingToHealth() {
        var health = disk.Health;
        img.fillAmount = (float) (health / totalHealth);
        img.color = new Color(1 - img.fillAmount, img.fillAmount, 0, 0.3f);
    }

    private void HandlePosition() {
        if (parent) {
            if (!Board.Instance.isHost && !Board.Instance.isTutorial) {
                transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y, parent.transform.position.z - 5);
            } else {
                transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y, parent.transform.position.z);
            }
        } else {
            if (gameObject) {
                Destroy(gameObject);
            }
        }
    }
}