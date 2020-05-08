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
    public Button EquipButton;
    public TextMeshProUGUI equipButtonText;
    public List<GameObject> Badges;
    public List<Image> DisplayOfEquipedBadge;
     
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("SaveSystem").GetComponent<SaveFile>().LoadFile();
        CurrentMoneyAmount = SaveFile.currentScore;
        currentMoney.text = "Money: " + CurrentMoneyAmount;
        Debug.Log(SaveFile.ifOwnBadge[0]);
        RefreshBadge();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Badges.Count; i++) //update the badge infomation based on data saved in the save file
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
        currentMoney.text = "Money: " + CurrentMoneyAmount; //display money

        if (EventSystem.current.currentSelectedGameObject!=null && EventSystem.current.currentSelectedGameObject.CompareTag("BadgeButton"))
        {
            Debug.Log("select item");
            currentSelectGameObject = EventSystem.current.currentSelectedGameObject;
        }//determine whether you can click the purchase button
        else if (EventSystem.current.currentSelectedGameObject == null)
        {
            currentSelectGameObject = null;
        }
        
        if (currentSelectGameObject!=null && CurrentMoneyAmount >= currentSelectGameObject.GetComponent<BadgeController>().badge.price&&currentSelectGameObject.GetComponent<BadgeController>().purchased!=true)
        {
            purchaseButton.interactable = true;
        }
        else
        {
            purchaseButton.interactable = false;
        }


        if (currentSelectGameObject!=null) //determine whether you can click the equip button
        {
        if (SaveFile.equipedBadges[2] == -1&&currentSelectGameObject.GetComponent<BadgeController>().purchased == true&&SaveFile.equipedBadges[0]!= currentSelectGameObject.GetComponent<BadgeController>().badge.badgeCode&& SaveFile.equipedBadges[1] != currentSelectGameObject.GetComponent<BadgeController>().badge.badgeCode)
        {
            EquipButton.interactable = true;
            equipButtonText.text = "equip";
            }
        else if (SaveFile.equipedBadges[2] == currentSelectGameObject.GetComponent<BadgeController>().badge.badgeCode|| SaveFile.equipedBadges[1] == currentSelectGameObject.GetComponent<BadgeController>().badge.badgeCode|| SaveFile.equipedBadges[0] == currentSelectGameObject.GetComponent<BadgeController>().badge.badgeCode)
        {
            EquipButton.interactable = true;
            equipButtonText.text = "unequip";
        }
            else
            {
                EquipButton.interactable = false;
            }
        }
        else
        {
            EquipButton.interactable = false;
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
        
    }//purchase function when click the purchase button

    public void Equip()
    {
        //equip
        if (SaveFile.equipedBadges[0] != currentSelectGameObject.GetComponent<BadgeController>().badge.badgeCode && SaveFile.equipedBadges[1] != currentSelectGameObject.GetComponent<BadgeController>().badge.badgeCode &&SaveFile.equipedBadges[2]!= currentSelectGameObject.GetComponent<BadgeController>().badge.badgeCode)
        {
            for (int i = 0; i < SaveFile.equipedBadges.Length; i++)
            {
                if (SaveFile.equipedBadges[i] == -1)
                {
                    SaveFile.equipedBadges[i] = currentSelectGameObject.GetComponent<BadgeController>().badge.badgeCode;
                    GameObject.Find("SaveSystem").GetComponent<SaveFile>().SaveThisFile();
                    break;
                }
            }
        }


        //unequip


        else if (SaveFile.equipedBadges[2] == currentSelectGameObject.GetComponent<BadgeController>().badge.badgeCode || SaveFile.equipedBadges[1] == currentSelectGameObject.GetComponent<BadgeController>().badge.badgeCode || SaveFile.equipedBadges[0] == currentSelectGameObject.GetComponent<BadgeController>().badge.badgeCode)
        {
            for (int i = 0; i < SaveFile.equipedBadges.Length; i++)
            {
                if (SaveFile.equipedBadges[i] == currentSelectGameObject.GetComponent<BadgeController>().badge.badgeCode)
                {
                    
                    for (int j = i; j < SaveFile.equipedBadges.Length-1; j++)
                    {
                        SaveFile.equipedBadges[j] = SaveFile.equipedBadges[j + 1];
                    }
                    SaveFile.equipedBadges[SaveFile.equipedBadges.Length-1] = -1;
                    GameObject.Find("SaveSystem").GetComponent<SaveFile>().SaveThisFile();
                    break;
                }
            }
        }

        RefreshBadge();
    }//equip function when click the equip button, equip the badge if not equiped already, unequip the badge if already equip the badge

    public void RefreshBadge()//refresh the badge display of currently equiped badge
    {
        for (int i = 0; i < SaveFile.equipedBadges.Length; i++)
        {
            if (SaveFile.equipedBadges[i]!=-1)
            {
                int tempBadgeCode = SaveFile.equipedBadges[i];
                DisplayOfEquipedBadge[i].sprite = Badges[tempBadgeCode].GetComponent<BadgeController>().badge.BadgeImage;

            }
            else
            {
                DisplayOfEquipedBadge[i].sprite = null;
                
            }
        }
    }
}
