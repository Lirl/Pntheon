using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UseCard : MonoBehaviour {

    public int code;
    public int cost = 100;
    public GameObject card;
    public CollectionManager cm;
    private User user;

    public TextMeshProUGUI use;
    public TextMeshProUGUI upgrade;
    public TextMeshProUGUI Gold;

    private void Start() {
        user = FindObjectOfType<User>();
        cost = 100;
        for(int i = 0; i < User.instance.cardLevels[code]; i++) {
            cost *= 2;
        }
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
            user.cardLevels[code] = 1;
            user.gold -= cost;
            cost *= 2;
            //var d = Instantiate(card);
            //d.transform.parent = cm.Store.transform;
            card.transform.parent = cm.Cards.transform;
            DecreaseGold();
        }
    }

    public void Upgrade() {

    }

    private void DecreaseGold() {
        if (user.gold != user.prevGold) {
            user.prevGold -= 1;
            Gold.text = "Gold: " + user.prevGold.ToString();
            Invoke("DecreaseGold", 0.01f);
        }
    }
}
