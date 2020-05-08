using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BadgeController : MonoBehaviour //control the display of badge inside the store
{
    public Button thisBadge;
    
    public bool purchased = false;
    public bool revealed = false;

    public Badge badge;
    public Image icon;
    
    public TextMeshProUGUI description;
    public TextMeshProUGUI price;
    public TextMeshProUGUI nameOfBadge;

    public GameObject ownedText;
    public GameObject CoverImage;
    
    void Start() //import the scriptable object
    {
        nameOfBadge.text = badge.BadgeName;
        icon.sprite = badge.BadgeImage;
        description.text = badge.description;
        price.text = "$" + badge.price;
    }

    
    void Update()
    {
        //update the display based on whether it is revealed or purchased
        if (purchased)
        {
            thisBadge.interactable = true;
            ownedText.SetActive(true);
            CoverImage.SetActive(false);
        }
        else if (!revealed)
        {
            thisBadge.interactable = false;
            CoverImage.SetActive(true);
        }
        else
        {
            thisBadge.interactable = true;
            CoverImage.SetActive(false);
        }
    }
}
