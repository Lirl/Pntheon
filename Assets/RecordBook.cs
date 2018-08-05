using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecordBook : MonoBehaviour {

    private int count = 0;
    private int code = 0;
    public bool favoriteCard;

    // Use this for initialization
    void Start() {
        GetTotalPlayes();
        var tmp = GetComponent<TextMeshProUGUI>();
        var user = User.instance;
        if (favoriteCard) {
            tmp.text = ConvertCodeToString(code);
        }
        else {
            tmp.text = "  " + user.LongestWinningStreak + "\n  " +
                        user.CurrentWinningStreak + "\n\n  " +
                        user.WinsAgainstAI + "\n  " +
                        user.LossesAgainstAI + "\n\n  " +
                        (user.wins - user.WinsAgainstAI) + "\n  " +
                        (user.losses - user.LossesAgainstAI) + "\n\n\n  " +
                        count;
        }
        
    }

    public void GetTotalPlayes() {
        for (int i = 0; i < User.instance.TimesPlayedCard.Count; i++) {
            count += User.instance.TimesPlayedCard[i];
            if (User.instance.TimesPlayedCard[i] > code) {
                code = i;
            }
        }
    }

    public string ConvertCodeToString(int characeterCode) {
        switch (characeterCode) {
            case 0:
                return "Priest";
            case 1:
                return "Warrior eagle";
            case 2:
                return "Wizard";
            case 3:
                return "Brute";
            case 4:
                return "Shaman";
            case 5:
                return "Assasin";
            case 6:
                return "Warlord";
            case 7:
                return "Arachne";
            default:
                return "Whaaaat?";
        }
    }
}
