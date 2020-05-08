using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
   

    public void LoadSceneWithNum(int sceneNumber) //load the scene
    {
        GameObject.Find("SaveSystem").GetComponent<SaveFile>().SaveThisFile();
        SceneManager.LoadScene(sceneNumber);
    }
  public void QuitGame()//quit the game
    {
        GameObject.Find("SaveSystem").GetComponent<SaveFile>().SaveThisFile();
        Application.Quit();
    }
}
