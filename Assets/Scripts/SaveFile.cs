using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveFile : MonoBehaviour
{
   public static int currentScore = 0;



    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        LoadFile();
        SaveThisFile();
        
    }
    
    

    public void SaveThisFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        GameData data = new GameData(currentScore);
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
        

       
        Debug.Log(data.score);
       
    }

    [System.Serializable]
    public class GameData
    {
        public int score;
        

        public GameData(int scoreInt)
        {
            score = scoreInt;
           
        }
    }
}
