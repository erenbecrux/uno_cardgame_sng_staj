using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeItem : MonoBehaviour
{
    public int ItemID;
    public int ItemPrice;
    public Image BackgroundTheme;

    /// <summary>
    /// Set the selected item as current theme if clicked item is owned. Otherwise check player's money and show required pop up
    /// </summary>
    public void OnClickItem()
    {
        if (GameDataSingleton.Instance.OwnedThemeIDs.Contains(this.ItemID))
        {
            // Change current theme
            ControllerReference.Instance.StoreController.SetCurrentTheme(ItemID);
        }
        else
        {
            if (GameDataSingleton.Instance.PlayerMoney >= ItemPrice)
            {
                // Show buy pop up
                ControllerReference.Instance.StoreController.ShowBuyPopUp(this);                
            }
            else
            {
                // Show error pop up
                ControllerReference.Instance.StoreController.ErrorPopUp.SetActive(true);
            }
        }
    }    

}
