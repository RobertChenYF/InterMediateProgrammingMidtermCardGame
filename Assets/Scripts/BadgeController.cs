using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BadgeController : MonoBehaviour
{
    public Button thisBadge;
    
    public bool purchased = false;
    public bool revealed = false;

    public Badge badge;
    public Image icon;
    
    public TextMeshProUGUI description;
    public TextMeshProUGUI price;
    public TextMeshProUGUI nameOfBadge;

    public GameObject CoverImage;
    // Start is called before the first frame update
    void Start()
    {
        nameOfBadge.text = badge.BadgeName;
        icon.sprite = badge.BadgeImage;
        description.text = badge.description;
        price.text = "$" + badge.price;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(EventSystem.current);
        if (purchased)
        {
            thisBadge.interactable = false;
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
