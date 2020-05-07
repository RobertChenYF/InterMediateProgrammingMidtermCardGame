
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Badge", order = 1)]
public class Badge : ScriptableObject
{
    
    public string BadgeName;
    public Sprite BadgeImage;
    public int badgeCode;
    public int price;
    [TextArea(15, 20)]
    public string description;
}
