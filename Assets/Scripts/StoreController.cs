using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class StoreController : MonoBehaviour
{
    public ThemeItem ItemPrefab;
    public GameObject StoreBackground;

    public ThemeDesign[] ThemeDesigns;
    public List<ThemeItem> Items = new List<ThemeItem>();

    public TextMeshProUGUI MoneyText;

    public GameObject ErrorPopUp;
    public GameObject BuyPopUp;
    public TextMeshProUGUI PriceText;

    private ThemeItem itemToBuy;

    private void Start()
    {
        // Initialize store
        InitializeStore();

        // Set Player money text
        MoneyText.text = GameDataSingleton.Instance.PlayerMoney.ToString();

        // Initialize current theme
        SetCurrentTheme(GameDataSingleton.Instance.CurrentThemeID);
    }

    /// <summary>
    /// Initializes store by instantiating items in store and sorting them
    /// </summary>
    public void InitializeStore()
    {
        for(int i = 0; i < ThemeDesigns.Length; i++)
        {
            ThemeItem newItem;

            newItem = Instantiate(ItemPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            Items.Add(newItem);

            newItem.ItemID = ThemeDesigns[i].ThemeID;
            newItem.ItemPrice = ThemeDesigns[i].ThemePrice;
            newItem.BackgroundTheme.sprite = ThemeDesigns[i].BackgroundSprite;

            // Initialize item description
            if(GameDataSingleton.Instance.OwnedThemeIDs.Contains(newItem.ItemID))
            {
                if(newItem.ItemID == GameDataSingleton.Instance.CurrentThemeID)
                {
                    newItem.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "CURRENT THEME";
                }
                else
                {
                    newItem.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "OWNED";
                }
            }
            else
            {
                newItem.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "PRICE: " + newItem.ItemPrice;
            }

            newItem.transform.SetParent(StoreBackground.transform, false);
        }

        // Sort market
        Vector2 posVector = (Constants.MarketOffSetVector / (Items.Count+1));

        for(int i = 0; i < Items.Count; i++)
        {
            Items[i].GetComponent<RectTransform>().anchoredPosition = Items[i].GetComponent<RectTransform>().anchoredPosition + posVector;
            posVector = posVector + (Constants.MarketOffSetVector / (Items.Count+1));
        }
    }

    /// <summary>
    /// Changes current theme to given item and modifies item descriptions
    /// </summary>
    public void SetCurrentTheme(int itemID)
    {
        // Find the selected item and set it as current theme, then set other owned items' descriptions
        for(int i = 0; i < Items.Count; i++)
        {
            if(itemID == ThemeDesigns[i].ThemeID)
            {
                GameDataSingleton.Instance.CurrentTheme = ThemeDesigns[i];
                GameDataSingleton.Instance.CurrentThemeID = itemID;
                Items[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "CURRENT THEME";
            }
            else
            {
                if (GameDataSingleton.Instance.OwnedThemeIDs.Contains(Items[i].ItemID))
                {
                    Items[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "OWNED";
                }         
            }
        }
    }

    /// <summary>
    /// Updates money text to current money value of player
    /// </summary>
    public void UpdateMoneyText()
    {
        MoneyText.text = GameDataSingleton.Instance.PlayerMoney.ToString();
    }

    /// <summary>
    /// Shows buy pop up and sets itemToBuy to selected item
    /// </summary>
    public void ShowBuyPopUp(ThemeItem item)
    {
        BuyPopUp.gameObject.SetActive(true);
        PriceText.text = "PRICE: " + item.ItemPrice;
        itemToBuy = item;
    }

    /// <summary>
    /// Buys the previously clicked item and sets its description
    /// </summary>
    public void OnClickBuyButton()
    {
        GameDataSingleton.Instance.OwnedThemeIDs.Add(itemToBuy.ItemID);
        GameDataSingleton.Instance.PlayerMoney = GameDataSingleton.Instance.PlayerMoney - itemToBuy.ItemPrice;

        itemToBuy.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "OWNED";

        // Update money text in main menu
        ControllerReference.Instance.StoreController.UpdateMoneyText();

        // Close buy pop up is activated by the animator
    }

}
