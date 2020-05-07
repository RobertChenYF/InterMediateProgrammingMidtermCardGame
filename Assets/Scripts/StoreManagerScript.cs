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
    // Start is called before the first frame update
    void Start()
    {
        CurrentMoneyAmount = SaveFile.currentScore;
        currentMoney.text = "Money: " + CurrentMoneyAmount;
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
        currentSelectGameObject.GetComponent<BadgeController>().purchased = true;
        }
        
    }
}
