using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UseCard : MonoBehaviour {

    public int code;
    public int cost;
    public GameObject card;
    public CollectionManager cm;
    private User user;

    public TextMeshProUGUI use;
    public TextMeshProUGUI upgrade;
    public TextMeshProUGUI Gold;
    public TextMeshProUGUI Rank;
    public Color notAvailable;

    private void Start() {
        user = FindObjectOfType<User>();
        int cost = 100;
        /*
        for(int i = 0; i < user.cardLevels[code]; i++) {
            Debug.Log("Card " + code + " Level is: " + user.cardLevels[code]);
            cost *= 2;
        }*/
        if (!user) {
            Debug.Log("Card cant find user");
        }
        if (transform.parent == cm.Store.transform) {
            upgrade.color = Color.gray;
            var b = upgrade.transform.GetComponent<Button>();
            b.interactable = false;
            Debug.Log("found card " + code + " in store");
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
            card.transform.parent = cm.Cards.transform;
            var up = transform.parent.Find("Upgrade");
            var button = up.GetComponent<Button>();
            button.interactable = true;
            upgrade.text = "Upgrade " + cost;
            upgrade.color = Color.white;
            DecreaseGold();
            user.Save();
        }
    }

    public void Upgrade() {
        if (user.gold >= cost) {            
            user.cardLevels[code]++;
            user.gold -= cost;
            cost *= 2;
            /*if (user.cardLevels[code] == 6) {
                RectTransform rt = Rank.rectTransform;
                
            }*/
            Rank.text = "" + user.cardLevels[code];
            //var d = Instantiate(card);
            //d.transform.parent = cm.Store.transform;            
            DecreaseGold();
            upgrade.text = "Upgrade " + cost;
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
