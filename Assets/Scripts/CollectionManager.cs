using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionManager : MonoBehaviour {

    [SerializeField]
    private User user;

    [Header("GameObjects For Reference")]
    public GameObject Deck;
    public GameObject Cards;
    public GameObject Store;
    public GameObject CollectionsMenu;
    public GameObject StoreMenu;
    public TextMeshProUGUI Gold;
     
    

    public GameObject god;
    public Image image;
    public GameObject[] Descriptions;
    public GameObject alreadyInUse;
    public Sprite[] God;

    [Header("Booleans for card switching")]
    public bool firstCardClicked = false;
    public bool firstClickedIsOnDeck;
    public UseCard lastClickedCard;

    [Header("Shake Values")]
    [Range(0, 10f)]
    public float magnitude;
    [Range(0, 10f)]
    public float duration;
    public Color NotAvailable;

    private void Start() {       
        user = GameObject.FindObjectOfType<User>();
        if (!user) {
            Debug.Log("Cant find user");
        }
        for (int i = 0; i < user.deck.Count; i++) {
            //Debug.Log("Card " + user.deck[i]);
            var card = GameObject.Find("" + user.deck[i]);
            card.transform.parent = Deck.transform;
        }
        for (int i = 0; i < user.disks.Count; i++) {
            //Debug.Log("Card " + user.disks[i]);
            var card = GameObject.Find("" + user.disks[i]);
            card.transform.parent = Cards.transform;
        }
        foreach (GameObject go in Descriptions) {
            var useCard = go.GetComponentInChildren<UseCard>();
            for (int i = 0; i < user.cardLevels[useCard.code]; i++) {
                //Debug.Log("Card " + code + " Level is: " + user.cardLevels[code]);
                useCard.cost *= 2;
            }
            if (!useCard) {
                Debug.Log("Cant find UseCard");
            }
            if (useCard && useCard.card.transform.parent == Store.transform) {
                Debug.Log("Found card in store");
                ChangeText("Buy " + useCard.cost, go);                   
                useCard.upgrade.color = Color.gray;
                var button = useCard.upgrade.transform.parent.GetComponent<Button>();                    
                button.interactable = false;                
            } else {
                Debug.Log("Found card in Army");
                useCard.upgrade.text = "Upgrade " + useCard.cost;
            }
        }
        god = GameObject.FindGameObjectWithTag("God");
        if (god) {
            //Debug.Log("God Found");
            image = god.GetComponent<Image>();
            //god.SetActive(false);
        } else {
            Debug.Log("God is not here right now");
        }
        CollectionsMenu.SetActive(false);
        StoreMenu.SetActive(false);
    }

    internal void Buy(UseCard useCard) {
        if (user.gold >= useCard.cost) {
            user.disks.Add(useCard.code);
            user.gold -= useCard.cost;            
            transform.parent = Deck.transform;            
            //useCard.upgrade.text = "Upgrade " + useCard.cost;
            Gold.text = "Gold: " + user.gold.ToString();
        }
    }

    public void PutIn(UseCard card) {
        card.card.transform.parent = Deck.transform;
    }


    public void Swap(UseCard card) {
        //Pressing The First Card
        if (!firstCardClicked) {
            ChangeTextToAll("Replace");
            firstCardClicked = true;
            lastClickedCard = card;
            firstClickedIsOnDeck = (card.card.transform.parent == Deck.transform);
            Glow(true);
            Debug.Log("First card clicked is in " + card.card.transform.parent);
        //Choosing which card to replace
        } else {
            ChangeTextToAll("Use");
            Glow(false);
            if (!firstClickedIsOnDeck) { 
                if (!user) {
                    Debug.LogError("Cant find user");
                }
                user.deck.Remove(card.code);
                user.deck.Add(lastClickedCard.code);
                user.disks.Remove(lastClickedCard.code);
                user.disks.Add(card.code);
                card.card.transform.parent = Cards.transform;
                lastClickedCard.card.transform.parent = Deck.transform;
                firstCardClicked = false;
            } else {
                user.deck.Add(card.code);
                user.deck.Remove(lastClickedCard.code);
                user.disks.Add(lastClickedCard.code);
                user.disks.Remove(card.code);
                card.card.transform.parent = Deck.transform;
                lastClickedCard.card.transform.parent = Cards.transform;
                firstCardClicked = false;
            }
        }
    }

    private void ChangeTextToAll(string v) {
        foreach (GameObject go in Descriptions) {
            var useCard = go.GetComponentInChildren<UseCard>();
            useCard.use.text = v;
        }
    }

    public void ChangeText(string v, GameObject go) {
        var useCard = go.GetComponentInChildren<UseCard>();
        useCard.use.text = v;
    }

    private void Glow(bool glow) {
        var section = (firstClickedIsOnDeck == true) ? Cards : Deck;
        var children = section.GetComponentsInChildren<Glow>();        
        for (int i = 0; i < children.Length; i++) {
            bool yayOrNay = glow == true ? true : false;
            children[i].image.enabled = yayOrNay;            
        }
    }

    public void InsertCard(UseCard card) {
        if (user.deck.Count < 6) {
            PutIn(card);
        } else {
            Swap(card);
        }
    }

    public void ChangeGod(int code) {
        user.ChangeGod(code);
        Debug.Log("God Selected is " + code);
        
        image.sprite = God[code];
    }

    public void Save() {
        user.Save();
    }

}
