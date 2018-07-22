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
    public int xp;

    public float lastModified;
    public bool ChStats = true;

    public TextMeshProUGUI Games;
    public TextMeshProUGUI Wins;
    public TextMeshProUGUI Gold;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Rank;
    public TextMeshProUGUI XP;

    public Slider XPSlider;

    public User player;

    // Use this for initialization
    void Start() {
        player = FindObjectOfType<User>();

        //Get general stats
        wins = player.wins;
        games = player.wins + player.losses;        
        Games.text = "Games: " + games.ToString();
        Wins.text = "Wins: " + wins.ToString();

        //lvl and xp ui
        xp = player.prevXp;
        XPSlider.maxValue = player.levelsAndXp[player.playerLevel];
        XPSlider.value = xp;
        Rank.text = "lvl: " + player.playerLevel.ToString();
        XP.text = xp.ToString() + "/" + player.levelsAndXp[player.playerLevel].ToString();        

        //gold ui
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
            Invoke("ChangeGold", .15f);
            ChStats = false;
        }
    }

    private void ChangeStats() {
        player.prevXp = player.xp;
        if (xp == player.xp) {
            //ChStats = false;
            player.BackFromGame = false;
            player.prevXp = xp;
            player.Save();
        }
        else if (User.instance.wonLastGame) {
            xp += 1;
            if (xp > player.levelsAndXp[player.playerLevel]) {
                player.xp -= player.levelsAndXp[player.playerLevel];
                xp = 1;
                User.instance.playerLevel++;
                Rank.text = "lvl: " + player.playerLevel.ToString();
                XPSlider.maxValue = player.levelsAndXp[player.playerLevel];
            }
            XPSlider.value = xp;
            XP.text = xp.ToString() + "/" + player.levelsAndXp[player.playerLevel];
            Invoke("ChangeStats", 0.03f * player.playerLevel);
        }
        else if (!User.instance.wonLastGame) {
            xp -= 1;
            if (xp == 0) {
                if (player.playerLevel - 1 == 0) {
                    User.instance.playerLevel = 1;
                    User.instance.xp = 1;
                    User.instance.prevXp = 1;
                    User.instance.BackFromGame = false;
                    User.instance.Save();
                    return;
                }
                player.xp = player.levelsAndXp[(player.playerLevel - 1)] + player.xp;
                XPSlider.maxValue = player.levelsAndXp[player.playerLevel - 1];                
                xp = player.levelsAndXp[(player.playerLevel - 1)] - 1;
                XP.text = xp.ToString() + "/" + player.levelsAndXp[player.playerLevel];
                User.instance.playerLevel--;
                Rank.text = "lvl: " + player.playerLevel.ToString();
            }
            XPSlider.value = xp;
            XP.text = xp.ToString() + "/" + player.levelsAndXp[player.playerLevel];
            Invoke("ChangeStats", 0.03f * player.playerLevel);
        }  
    }

    private void ChangeGold() {
        player.prevGold = player.gold;
        if (gold != player.gold) {            
            gold += 1;
            Gold.text = "Gold: " + gold.ToString();
            Invoke("ChangeGold", 0.02f);
        }  else {
            player.Save();
        }        
    }
}
