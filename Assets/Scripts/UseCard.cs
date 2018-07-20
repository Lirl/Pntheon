﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UseCard : MonoBehaviour {

    public int code;
    public int cost;
    public GameObject card;
    public CollectionManager cm;
    private User user;

    public TextMeshProUGUI use;
    public TextMeshProUGUI Gold;

    private void Start() {
        user = FindObjectOfType<User>();
        if (!user) {
            Debug.Log("Card cant find user");
        }
        if (transform.parent == cm.Store.transform) {
            Debug.Log("found card in store");
            use.text = "Buy " + cost;
        }
    }

    public void Use() {
        if (use.text.Contains("Buy")) {
            Buy();
            cm.ChangeText("Use", use.transform.parent.gameObject);
            return;
        }
        cm.Swap(this);
    }

    public void Buy() {
        if (user.gold >= cost) {
            user.disks.Add(code);
            user.gold -= cost;
            //var d = Instantiate(card);
            //d.transform.parent = cm.Store.transform;
            card.transform.parent = cm.Cards.transform;
            DecreaseGold();
        }
    }

    private void DecreaseGold() {
        if (user.gold != user.prevGold) {
            user.prevGold -= 1;
            Gold.text = "Gold: " + user.prevGold.ToString();
            Invoke("DecreaseGold", 0.01f);
        }
    }
}
