using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopCard : MonoBehaviour {

    public GameObject[] Images;
    private int code = 0;

	// Use this for initialization
	void Start () {        
        if (Random.Range(0, 1f) < 0.4) {
            code = User.instance.deck[Random.Range(0, 3)];
        } else {
            GetTopCard();
        }
        Images[code].SetActive(true);
	}

    public void GetTopCard() {
        for (int i = 0; i < User.instance.TimesPlayedCard.Count; i++) {            
            if (User.instance.TimesPlayedCard[i] > code) {
                code = i;
            }
        }
    }
}
