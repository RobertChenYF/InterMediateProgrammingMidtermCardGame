using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class StoreManagerScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentMoney;
    public int CurrentMoneyAmount;

    private GameObject currentSelectGameObject;
    public Button purchaseButton;
    public List<GameObject> Badges;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("SaveSystem").GetComponent<SaveFile>().LoadFile();
        CurrentMoneyAmount = SaveFile.currentScore;
        currentMoney.text = "Money: " + CurrentMoneyAmount;
        Debug.Log(SaveFile.ifOwnBadge[0]);
        for (int i =0; i< Badges.Count; i++)
        {
            if (SaveFile.ifOwnBadge[i] == 1)
            {
                Badges[i].GetComponent<BadgeController>().revealed = true;
            }
            else if (SaveFile.ifOwnBadge[i] == 2)
            {
                Badges[i].GetComponent<BadgeController>().purchased = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentMoney.text = "Money: " + CurrentMoneyAmount; 
        if (EventSystem.current.currentSelectedGameObject!=null && EventSystem.current.currentSelectedGameObject.CompareTag("BadgeButton"))
        {
            Debug.Log("select item");
            currentSelectGameObject = EventSystem.current.currentSelectedGameObject;
        }
        else if (EventSystem.current.currentSelectedGameObject == null)
        {
            currentSelectGameObject = null;
        }
        
        if (currentSelectGameObject!=null && CurrentMoneyAmount >= currentSelectGameObject.GetComponent<BadgeController>().badge.price)
        {
            purchaseButton.interactable = true;
        }
        else
        {
            purchaseButton.interactable = false;
        }
    }

    public void Purchase()
    {
        if (CurrentMoneyAmount>=currentSelectGameObject.GetComponent<BadgeController>().badge.price)
        {
            CurrentMoneyAmount -= currentSelectGameObject.GetComponent<BadgeController>().badge.price;
            SaveFile.currentScore = CurrentMoneyAmount;
            currentSelectGameObject.GetComponent<BadgeController>().purchased = true;
            SaveFile.ifOwnBadge[currentSelectGameObject.GetComponent<BadgeController>().badge.badgeCode] = 2;
            //reveal a new badge
            for (int i = 0; i < Badges.Count; i++)
            {
                if (SaveFile.ifOwnBadge[i] == 0)
                {
                    SaveFile.ifOwnBadge[i] = 1;
                    Badges[i].GetComponent<BadgeController>().revealed = true;
                    break;
                }
            }
            GameObject.Find("SaveSystem").GetComponent<SaveFile>().SaveThisFile();
            currentSelectGameObject = null;
        }
        
    }
}
