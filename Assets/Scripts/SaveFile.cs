using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveFile : MonoBehaviour
{
   public static int currentScore = 0;
    public static int[] ifOwnBadge; //0: not revealed 1: revealed 2: owned 

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
        ifOwnBadge = new int[3];

        //SaveThisFile();
        LoadFile();

        
    }

    
    public void SaveThisFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        GameData data = new GameData(currentScore,ifOwnBadge);
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

       
        Debug.Log(data.score);
       
    }

    [System.Serializable]
    public class GameData
    {
        public int score;
        public int[] BadgeArray;

        public GameData(int scoreInt, int[]BArray)
        {
            score = scoreInt;
            BadgeArray = BArray;
        }
    }
}
