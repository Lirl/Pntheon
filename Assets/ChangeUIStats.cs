using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeUIStats : MonoBehaviour {

    public int health = 1;
    public int attack = 1;
    public int code;

    // Use this for initialization
    void Start () {
        var level = User.instance.cardLevels[code];	

        for (int i = 0; i < level; i++) {
            ChangeStats();
        }

        TextMeshProUGUI stats = transform.GetComponent<TextMeshProUGUI>();
        stats.text = health + "\n" + attack;
	}	


    public void ChangeStats() {

        health = (int)(health * 1.11f);
        attack = (int)(attack * 1.11f);

        TextMeshProUGUI stats = transform.GetComponent<TextMeshProUGUI>();
        stats.text = health + "\n" + attack;
    }
}
