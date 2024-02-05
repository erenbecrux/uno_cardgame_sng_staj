using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New CardDesign", menuName = "CardDesign")]
public class CardDesign : ScriptableObject
{
    public Color[] CardColors;
    public Sprite[] CardIconsTop;
    public bool HasCardIcon;
    public Sprite FourColorCard;
    public Sprite DrawFourCard;
    public bool IsSpecialCard;
    public Font CardFont;
}
