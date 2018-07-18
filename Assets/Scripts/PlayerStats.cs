using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    public int games;
    public int wins;
    public int gold;

    public float lastModified;
    public bool ChStats = false;

    public TextMeshProUGUI Games;
    public TextMeshProUGUI Wins;
    public TextMeshProUGUI Gold;
    public TextMeshProUGUI Name;

    User player;

    // Use this for initialization
    void Start() {
        player = FindObjectOfType<User>();

        wins = player.wins;
        games = player.wins + player.losses;        
        Games.text = "Games: " + games.ToString();
        Wins.text = "Wins: " + wins.ToString();

        gold = player.prevGold;
        Gold.text = "Gold: " + player.prevGold.ToString();
        Name.text = player.userName;

        if (player.BackFromGame) {
            ChStats = true;
        }
    }


    // Update is called once per frame
    void Update() {
        if (ChStats) {
            Invoke("ChangeStats", .15f);
            ChStats = false;
        }
    }

    private void ChangeStats() {
        if (gold != player.gold) {
            gold += 1;
            Gold.text = "Gold: " + gold.ToString();
            Invoke("ChangeStats", 0.02f);
        } else {
            //ChStats = false;
            User.instance.BackFromGame = false;
            player.prevGold = gold;
        }
    }
}
