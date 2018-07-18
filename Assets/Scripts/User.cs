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
    public int wins;
    public int losses;
    public int gold;
    public int prevGold;
    public int god;
    public static int handNextSlotIndex = 0;
    public int xp;
    public int playerLevel;

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
        userData.wins = instance.wins;
        userData.losses = instance.losses;
        userData.gold = instance.gold;
        userData.prevGold = instance.prevGold;
        userData.deck = instance.deck;
        userData.god = instance.god;
        userData.xp = instance.xp;
        userData.playerLevel = instance.playerLevel;


        bf.Serialize(file, userData);
        file.Close();
    }

    public void Load() {
        if(File.Exists(Application.persistentDataPath + "/userInfo.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/userInfo.dat", FileMode.Open);

            UserData userDate = (UserData)bf.Deserialize(file);
            disks = userDate.disks;
            userName = userDate.userName;
            wins = userDate.wins;
            losses = userDate.losses;
            gold = userDate.gold;
            prevGold = userDate.prevGold;
            deck = userDate.deck;
            god = userDate.god;
            xp = userDate.xp;
            playerLevel = userDate.playerLevel;
            BackFromGame = userDate.BackFromGame;

            file.Close();
        }

    }

    public void NewUser() {
        //Debug.LogError(Application.persistentDataPath);
        var texts = FindObjectsOfType<Text>();
        string newName = texts[0].text;

        instance.userName = newName;
        instance.disks = new List<int> { 3 };
        instance.wins = 0;
        instance.losses = 0;
        instance.god = 0;
        instance.gold = 1000;
        instance.prevGold = 1000;
        instance.deck = new List<int> { 0, 1, 2 };
        instance.deck.Capacity = 5;
        instance.xp = 0;
        instance.playerLevel = 0;
        instance.BackFromGame = false;
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
    public int wins;
    public int losses;
    public int gold;
    public int prevGold;
    public int god;
    public int xp;
    public int playerLevel;
    public bool BackFromGame;
}
