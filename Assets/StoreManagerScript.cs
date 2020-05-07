using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoreManagerScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentMoney;
    [SerializeField] private int CurrentMoneyAmount;
    // Start is called before the first frame update
    void Start()
    {
        CurrentMoneyAmount = SaveFile.currentScore;
        currentMoney.text = "Money: " + CurrentMoneyAmount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
