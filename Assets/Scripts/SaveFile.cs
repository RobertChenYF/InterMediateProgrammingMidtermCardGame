using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveFile : MonoBehaviour //very basic save file script, save the money earned, the badge purchased and the badge equiped
{
   public static int currentScore = 9999;
    public static int[] ifOwnBadge; //0: not revealed 1: revealed 2: owned 
    public static int[] equipedBadges;
    private static SaveFile _instance;

    public static SaveFile Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        ifOwnBadge = new int[] { 1,1,1,0};
        equipedBadges = new int[]{ -1,-1,-1};
        //SaveThisFile();
        LoadFile();

        
    }

    
    public void SaveThisFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else { file = File.Create(destination);
            ifOwnBadge = new int[] { 1, 1, 1, 0 };
            equipedBadges = new int[] { -1, -1, -1 };
            currentScore = 0;
        }
        

        GameData data = new GameData(currentScore,ifOwnBadge,equipedBadges);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        GameData data = (GameData)bf.Deserialize(file);
        file.Close();

        currentScore = data.score;
        ifOwnBadge = data.BadgeArray;
        equipedBadges = data.equip;

       
    }

    [System.Serializable]
    public class GameData
    {
        public int score;
        public int[] BadgeArray;
        public int[] equip;
        public GameData(int scoreInt, int[]BArray, int[] equipedBadges)
        {
            score = scoreInt;
            BadgeArray = BArray;
            equip = equipedBadges;
        }
    }
}
