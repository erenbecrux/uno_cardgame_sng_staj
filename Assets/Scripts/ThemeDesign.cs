using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New ThemeDesign", menuName = "ThemeDesign")]
public class ThemeDesign : ScriptableObject
{
    public int ThemeID;
    public int ThemePrice;
    public Sprite BackgroundSprite;
}
