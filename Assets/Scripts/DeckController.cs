using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    public CardDesign cardDesign;

    public Card CardPrefab;

    public List<Card> Deck = new List<Card>();
    public List<Card> ShuffledDeck = new List<Card>();

    private int numOfCards = 108;
    private int colorIndex = 0;


    /// <summary>
    /// Instantiates 108 cards and generates their attributes
    /// </summary>
    private void InstantiateDeck()
    {
        for (int i = 1; i <= numOfCards ; i++)
        {
            Card NewCard;
            NewCard = Instantiate(CardPrefab, new Vector3(0,0,0) , Quaternion.identity);

            NewCard.CardID = i;

            GenerateCardFace(NewCard);
            Deck.Add(NewCard);

            NewCard.CardFront.SetActive(false);
            NewCard.CardBack.SetActive(true);
        }
    }

    /// <summary>
    /// According to card's ID, generate its attributes
    /// </summary>
    private void GenerateCardFace(Card Card)
    {
        // cardID's should start from 1.
        colorIndex = (Card.CardID - 1) / 12; 

        if(colorIndex > 3 && colorIndex < 8)
        {
            // Resets color array access
            colorIndex = colorIndex - 4; 
        }
        else if(colorIndex >= 8)
        {
            // Set color index to 4 for special cards
            colorIndex = 4; 
        }


        Card.CardNumber = Card.CardID % 12;

        if (Card.CardNumber > 9)
        {
            Card.HasCardIcon = true;
        }
        else if(Card.CardNumber == 0) 
        {
            // Assigns number 12 to avoid mod operation's drawback
            Card.HasCardIcon = true;
            Card.CardNumber = 12;
        }

        // Checks if the card is one of the 0's or a wild card
        if(Card.CardID > 96)
        {
            Card.IsWildCard = true;
        }


        if(Card.IsWildCard)
        {
            if(Card.CardID <= 100 ) // 0's
            {
                if (Card.CardNumber == 1) // red 0
                {
                    colorIndex = 0;
                }
                else if (Card.CardNumber == 2) // yellow 0
                {
                    colorIndex = 1;
                }
                else if (Card.CardNumber == 3) // green 0
                {
                    colorIndex = 2;
                }
                else if (Card.CardNumber == 4) // blue 0
                {
                    colorIndex = 3;
                }

                Card.IsWildCard = false;

                Card.CardNumber = 0;

                Card.CardIconTop.SetActive(false);
                Card.CardTextTop.text = Card.CardNumber.ToString();

                Card.CardIconMain.SetActive(false);
                Card.CardTextMain.text = Card.CardNumber.ToString();

                Card.CardIconBottom.SetActive(false);
                Card.CardTextBottom.text = Card.CardNumber.ToString();

                
            }
            else if(Card.CardID <= 104) // wild card
            {
                Card.CardNumber = 13;

                Card.CardType = CardTypeEnums.WildCard;

                Card.CardIconTop.SetActive(false);
                Card.CardTextTop.enabled = false;

                Card.CardIconMain.SetActive(true);
                Card.CardIconMain.GetComponent<SpriteRenderer>().sprite = cardDesign.FourColorCard;
                Card.CardTextMain.enabled = false;

                Card.CardIconBottom.SetActive(false);
                Card.CardTextBottom.enabled = false;
            }
            else if(Card.CardID <= 108) // wild draw 4 card
            {
                Card.CardNumber = 14;

                Card.CardType = CardTypeEnums.WildDrawFourCard;

                Card.CardIconTop.SetActive(false);
                Card.CardTextTop.text = "+4";

                Card.CardIconMain.SetActive(true);
                Card.CardIconMain.GetComponent<SpriteRenderer>().sprite = cardDesign.DrawFourCard;
                Card.CardTextMain.enabled = false;

                Card.CardIconBottom.SetActive(false);
                Card.CardTextBottom.text = "+4";
            }
        }
        else if (Card.HasCardIcon)
        {
            // top,icon
            Card.CardIconTop.SetActive(true);
            Card.CardIconTop.GetComponent<SpriteRenderer>().sprite = cardDesign.CardIconsTop[Card.CardNumber - 10]; // 10-block ; 11-reverse ; 12-addtwo
            Card.CardTextTop.enabled = false;

            // main,icon
            Card.CardIconMain.SetActive(true);
            Card.CardIconMain.GetComponent<SpriteRenderer>().sprite = cardDesign.CardIconsTop[Card.CardNumber - 10]; // 10-block ; 11-reverse ; 12-addtwo
            Card.CardTextMain.enabled = false;

            // bottom,icon
            Card.CardIconBottom.SetActive(true);
            Card.CardIconBottom.GetComponent<SpriteRenderer>().sprite = cardDesign.CardIconsTop[Card.CardNumber - 10]; // 10-block ; 11-reverse ; 12-addtwo
            Card.CardTextBottom.enabled = false;

            // if the card is +2 modify corner texts
            if(Card.CardNumber == 12)
            {
                Card.CardIconTop.SetActive(false);
                Card.CardTextTop.enabled = true;
                Card.CardTextTop.text = "+2";

                Card.CardIconBottom.SetActive(false);
                Card.CardTextBottom.enabled = true;
                Card.CardTextBottom.text = "+2";
            }

            if(Card.CardNumber == 10)
            {
                Card.CardType = CardTypeEnums.SkipCard;
            }
            else if(Card.CardNumber == 11)
            {
                Card.CardType = CardTypeEnums.ReverseCard;
            }
            else if(Card.CardNumber == 12)
            {
                Card.CardType = CardTypeEnums.DrawTwoCard;
            }
        }
        else
        {
            // top,number
            Card.CardIconTop.SetActive(false);
            Card.CardTextTop.text = Card.CardNumber.ToString();

            // main,number
            Card.CardIconMain.SetActive(false);
            Card.CardTextMain.text = Card.CardNumber.ToString();

            // bottom,number
            Card.CardIconBottom.SetActive(false);
            Card.CardTextBottom.text = Card.CardNumber.ToString();
        }

        // arrange card colors for both sprite and gameplay
        Card.SpriteColor = cardDesign.CardColors[colorIndex];
        if(colorIndex == 4)
        {
            Card.MainCardColor.GetComponent<SpriteRenderer>().color = Card.SpriteColor; // for draw 4 cards and 4 color cards
            Card.CardColorGameplay = CardColorEnums.Black;
        }
        else
        {
            Card.MainCardColor.GetComponent<SpriteRenderer>().color = Card.SpriteColor;
            Card.CardTextMain.color = Card.SpriteColor;
            Card.CardIconMain.GetComponent<SpriteRenderer>().color = Card.SpriteColor;

            if(colorIndex == 0)
            {
                Card.CardColorGameplay = CardColorEnums.Red;
            }
            else if(colorIndex == 1)
            {
                Card.CardColorGameplay = CardColorEnums.Yellow;
            }
            else if (colorIndex == 2)
            {
                Card.CardColorGameplay = CardColorEnums.Green;
            }
            else if (colorIndex == 3)
            {
                Card.CardColorGameplay = CardColorEnums.Blue;
            }
        }
        
    }

    /// <summary>
    /// Shuffles the deck and add each card to shuffled deck list
    /// </summary>
    private void ShuffleDeck()
    {
        List<int> indexList = new List<int>();
        int remainingCards = numOfCards;

        for(int i = 0; i < numOfCards; i++)
        {
            indexList.Add(i);
        }

        for(int i = 0; i < numOfCards; i++)
        {
            int randomIndex = Random.Range(0, remainingCards);
            ShuffledDeck.Add(Deck[indexList[randomIndex]]);
            indexList.Remove(indexList[randomIndex]);
            remainingCards--;
        }
    }

    /// <summary>
    /// Returns a card list containing seven cards from shuffled deck
    /// </summary>
    private List<Card> TakeTopSevenCards()
    {
        List<Card> cardsToTake = new List<Card>();

        for(int i = 0; i < 7; i++)
        {
            cardsToTake.Add(ShuffledDeck[0]);
            ShuffledDeck.RemoveAt(0);
        }

        return cardsToTake;
    }

    /// <summary>
    /// Returns a card list containing remaining cards for draw pile
    /// </summary>
    private List<Card> TakeRemainingCards()
    {
        List<Card> cardsToTake = new List<Card>();

        for (int i = 0; i < 80; i++)
        {
            cardsToTake.Add(ShuffledDeck[0]);
            ShuffledDeck.RemoveAt(0);
        }

        return cardsToTake;
    }

    /// <summary>
    /// Sets card table by instantiating cards, shuffling the deck, drawing seven cards for players and setting starting card. Then starts turn
    /// </summary>
    public IEnumerator SetCardTable()
    {
        GameDataSingleton.Instance.IsTableSet = false;
        UIManager.Instance.InGameUIController.MenuButton.interactable = false;

        InstantiateDeck();
        ShuffleDeck();

        yield return new WaitForSeconds(Constants.TableSettingTime);
        ControllerReference.Instance.PlayerController.DrawSevenCards(TakeTopSevenCards());

        yield return new WaitForSeconds(Constants.TableSettingTime);
        ControllerReference.Instance.RightBotController.DrawSevenCardForBot(TakeTopSevenCards());

        yield return new WaitForSeconds(Constants.TableSettingTime);
        ControllerReference.Instance.OppositeBotController.DrawSevenCardForBot(TakeTopSevenCards());

        yield return new WaitForSeconds(Constants.TableSettingTime);
        ControllerReference.Instance.LeftBotController.DrawSevenCardForBot(TakeTopSevenCards());

        yield return new WaitForSeconds(Constants.TableSettingTime);
        ControllerReference.Instance.DrawPileController.SetDrawPile(TakeRemainingCards());

        yield return new WaitForSeconds(Constants.TableSettingTime);
        ControllerReference.Instance.DrawPileController.DrawStartingCard();

        yield return new WaitForSeconds(Constants.TableSettingTime*2);
        GameDataSingleton.Instance.IsTableSet = true;
        UIManager.Instance.InGameUIController.MenuButton.interactable = true;
        GameManager.Instance.StartTurn();
    }

    /// <summary>
    /// Loads previous game session from gamedata file
    /// </summary>
    public void LoadCardTable()
    {
        InstantiateDeck();

        ControllerReference.Instance.PlayerController.LoadPlayerHand();
        ControllerReference.Instance.RightBotController.LoadBotHand(GameDataSingleton.Instance.RightBotCardIDs);
        ControllerReference.Instance.OppositeBotController.LoadBotHand(GameDataSingleton.Instance.OppositeBotCardIDs);
        ControllerReference.Instance.LeftBotController.LoadBotHand(GameDataSingleton.Instance.LeftBotCardIDs);
        ControllerReference.Instance.DrawPileController.LoadDrawPile();
        ControllerReference.Instance.DrawPileController.LoadDiscardPile();
    }

}
