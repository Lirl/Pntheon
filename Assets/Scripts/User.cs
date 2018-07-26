using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class User : MonoBehaviour {

    public static User instance;


    #region player saved variables

    public string userName;

    public List<int> disks;
    public List<int> deck;
    public List<int> cardLevels;

    public int wins;
    public int losses;

    
    public int WinsAgainstAI;
    public int LossesAgainstAI;
    public int GoldSpent;
    public List<int> TimesPlayedCard;
    public int CurrentWinningStreak;
    public int LongestWinningStreak;

    public int gold;
    public int prevGold;
    public int god;
    public static int handNextSlotIndex = 0;

    public bool wonLastGame;
    public int xp;
    public int playerLevel;
    public int prevXp;
    public List<int> levelsAndXp = new List<int> { 0, 100, 300, 1000, 5000, 15000 };

    public bool BackFromGame;

    #endregion

    void Awake () {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
            this.Load();
        } else if (instance != this) {
            Destroy(gameObject);
        }   
	}

    public void Save() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/userInfo.dat");

        UserData userData = new UserData();
        userData.userName = instance.userName;
        userData.disks = instance.disks;
        userData.cardLevels = instance.cardLevels;
        userData.wins = instance.wins;
        userData.losses = instance.losses;
        userData.gold = instance.gold;
        userData.prevGold = instance.prevGold;
        userData.deck = instance.deck;
        userData.god = instance.god;
        userData.xp = instance.xp;
        userData.prevXp = instance.prevXp;
        userData.playerLevel = instance.playerLevel;
        userData.wonLastGame = instance.wonLastGame;

        userData.LongestWinningStreak = instance.LongestWinningStreak;
        userData.CurrentWinningStreak = instance.CurrentWinningStreak;
        userData.WinsAgainstAI = instance.WinsAgainstAI;
        userData.LossesAgainstAI = instance.LossesAgainstAI;
        userData.TimesPlayedCard = instance.TimesPlayedCard;
        userData.GoldSpent = instance.GoldSpent;

        bf.Serialize(file, userData);
        file.Close();
    }

    public void Load() {
        if(File.Exists(Application.persistentDataPath + "/userInfo.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/userInfo.dat", FileMode.Open);

            UserData userDate = (UserData)bf.Deserialize(file);
            disks = userDate.disks;
            cardLevels = userDate.cardLevels;
            userName = userDate.userName;
            wins = userDate.wins;
            losses = userDate.losses;
            gold = userDate.gold;
            prevGold = userDate.prevGold;
            prevXp = userDate.prevXp;
            deck = userDate.deck;
            god = userDate.god;
            xp = userDate.xp;
            playerLevel = userDate.playerLevel;
            BackFromGame = userDate.BackFromGame;
            wonLastGame = userDate.wonLastGame;

            TimesPlayedCard = userDate.TimesPlayedCard;
            CurrentWinningStreak = userDate.CurrentWinningStreak;
            LongestWinningStreak = userDate.LossesAgainstAI;
            WinsAgainstAI = userDate.WinsAgainstAI;
            LossesAgainstAI = userDate.LossesAgainstAI;
            GoldSpent = userDate.GoldSpent;

            file.Close();
        }

    }

    public void NewUser() {
        //Debug.LogError(Application.persistentDataPath);
        var texts = FindObjectsOfType<Text>();
        string newName = texts[0].text;

        instance.userName = newName;
        instance.disks = new List<int> { 3};
        instance.cardLevels = new List<int> { 1, 1, 1, 0, 0, 0, 0, 0 };
        instance.wins = 0;
        instance.losses = 0;
        instance.god = 0;
        instance.gold = 1000;
        instance.prevGold = 1000;
        instance.deck = new List<int> { 0, 1, 2 };
        instance.deck.Capacity = 5;
        instance.xp = 0;
        instance.prevXp = 0;
        instance.playerLevel = 1;
        instance.BackFromGame = false;
        instance.wonLastGame = false;

        instance.TimesPlayedCard = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 };
        instance.CurrentWinningStreak = 0;
        instance.LongestWinningStreak = 0;
        instance.GoldSpent = 0;
        instance.WinsAgainstAI = 0;
        instance.LossesAgainstAI = 0;

        Save();
    }

    public void ChangeGod(int code) {
        god = code;
        Save();
    }

}

[Serializable]
class UserData {
    public string userName;
    public List<int> disks;
    public List<int> deck;
    public List<int> cardLevels;
    public int wins;
    public int losses;
    public int gold;
    public int prevGold;
    public int god;
    public int xp;
    public int playerLevel;
    public bool BackFromGame;
    public bool wonLastGame;
    public int prevXp;

    public int WinsAgainstAI;
    public int LossesAgainstAI;
    public int GoldSpent;
    public List<int> TimesPlayedCard;
    public int CurrentWinningStreak;
    public int LongestWinningStreak;
}
