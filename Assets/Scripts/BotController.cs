using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BotController : MonoBehaviour
{
    public Transform BotHandSlot;
    public List<Card> BotDeck = new List<Card>();
    private List<Card> PlayableBotCards = new List<Card>();
    private int[] ColorCounter = { 0, 0, 0, 0 }; // 1: red, 2: yellow, 3: green, 4: blue
    private int[] PointCounter = { 0, 0, 0, 0 }; // 1: red, 2: yellow, 3: green, 4: blue

    private Vector3 OffSetOne = Constants.OffSetValueOne;
    private Vector3 OffSetTwo = Constants.OffSetValueTwo;
    private Vector3 OffSetThree = Constants.OffSetValueThree;

    public bool isClickedUno = false;

    /// <summary>
    /// Adds cards to the bot deck in the given list of seven cards
    /// </summary>
    public void DrawSevenCardForBot(List<Card> sevenCards)
    {
        for(int i = 0; i < 7; i++)
        {
            BotDeck.Add(sevenCards[i]);
            BotDeck[i].transform.parent = BotHandSlot;
        }

        SortBotHand();
    }

    /// <summary>
    /// Sorts bot hand and sets cards' position according the number of cards in the bot deck
    /// </summary>
    private void SortBotHand()
    {
        Vector3 currentCardPosition = new Vector3(0, 0, 0);
        Vector3 offSetValue = new Vector3(0, 0, 0);

        if (BotDeck.Count <= Constants.NumOfCardOffSetOne)
        {
            offSetValue = OffSetOne;
        }
        else if (BotDeck.Count <= Constants.NumOfCardOffSetTwo)
        {
            offSetValue = OffSetTwo;
        }
        else if (BotDeck.Count <= Constants.NumOfCardOffSetThree)
        {
            offSetValue = OffSetThree;
        }

        // Calculates beginning of the hand
        currentCardPosition = currentCardPosition - (((BotDeck.Count - 1) * offSetValue) / 2);

        // Set each card's position
        for (int i = 0; i < BotDeck.Count; i++)
        {
            BotDeck[i].GetComponent<SortingGroup>().sortingOrder = i;

            BotDeck[i].transform.localEulerAngles = new Vector3(0, 0, 0);
            BotDeck[i].MoveCardToHand(currentCardPosition, i * Constants.HandSortingTimeMultiplier);          
            currentCardPosition = currentCardPosition + offSetValue;
        }
    }

    // This method can be deleted. Plays first card in the bot deck for testing
    public IEnumerator PlayFirstCard()
    {
        yield return new WaitForSeconds(2);

        if (BotDeck.Count == 2)
        {
            RandomlyClickUnoButton();
        }

        Card cardToPlay = BotDeck[0];

        Debug.Log("Bot played:" + cardToPlay.CardColorGameplay + cardToPlay.CardNumber);
        ControllerReference.Instance.DrawPileController.SetDiscardedCardPosition(cardToPlay);
        GameManager.Instance.SetCurrentCardAttributes(cardToPlay);

        if (cardToPlay.CardType == CardTypeEnums.WildCard || cardToPlay.CardType == CardTypeEnums.WildDrawFourCard)
        {
            // default value: red
            GameManager.Instance.CurrentCardColor = CardColorEnums.Red;
        }

        BotDeck.Remove(cardToPlay);
        SortBotHand();

        GameManager.Instance.EndTurn();
    }

    /// <summary>
    /// Plays a card from bot deck after findind playable cards. Also, if it is required, clicks uno button
    /// </summary>
    public IEnumerator PlayOneTurnBot()
    {
        yield return new WaitForSeconds(Constants.BotWaitingTime);

        // Finds playable cards
        FindPlayableCardsBot();

        // Checks if there are playable cards
        if(PlayableBotCards.Count == 0)
        {
            // If there are no playable cards, draw a card
            DrawOneCardBot();
            yield return new WaitForSeconds(Constants.BotWaitingTimeAfterDrawing);

            // Checks if the drawn card is playable
            FindPlayableCardsBot();
            if (PlayableBotCards.Count != 0)
            {
                Card drawnCard = BotDeck[BotDeck.Count - 1];
                PlayCardBot(drawnCard);
            }
        }
        else
        {
            // Randomly clicks uno button when there are two cards in the bot deck
            if (BotDeck.Count == 2)
            {
                RandomlyClickUnoButton();
            }

            // Finds max valued card in the playable cards
            int maxValuedCardIndex = 0;
            for(int i = 0; i < PlayableBotCards.Count; i++)
            {
                if (PlayableBotCards[i].CardNumber > PlayableBotCards[maxValuedCardIndex].CardNumber)
                {
                    maxValuedCardIndex = i;
                }
            }

            Card cardMaxValued = PlayableBotCards[maxValuedCardIndex];
            PlayCardBot(cardMaxValued);
        }


        // Calls EndTurn method at the end
        GameManager.Instance.EndTurn();
    }

    /// <summary>
    /// Draws one card from the draw pile for bot
    /// </summary>
    private void DrawOneCardBot()
    {
        Card cardToDraw = ControllerReference.Instance.DrawPileController.DrawPileDeck[0];

        BotDeck.Add(cardToDraw);
        cardToDraw.transform.parent = BotHandSlot;
        cardToDraw.IsInDrawPile = false;

        ControllerReference.Instance.DrawPileController.DrawPileDeck.Remove(cardToDraw);

        SortBotHand();
    }

    /// <summary>
    /// Draws two cards from the draw pile for bot
    /// </summary>
    public void DrawTwoCardsBot()
    {
        DrawOneCardBot();
        DrawOneCardBot();
    }

    /// <summary>
    /// Calculates bot hand's score and returns it
    /// </summary>
    public int CalculateBotHand()
    {
        int scoreAccumuator = 0;
        for (int i = 0; i < BotDeck.Count; i++)
        {
            Card card = BotDeck[i];

            if(card.CardNumber <= 9)
            {
                scoreAccumuator += card.CardNumber;
            }
            else if(card.CardNumber <= 12)
            {
                scoreAccumuator += Constants.ActionCardPoints;
            }
            else if(card.CardNumber <= 14)
            {
                scoreAccumuator += Constants.WildCardPoints;
            }
        }

        return scoreAccumuator;
    }

    /// <summary>
    /// Randomly click uno button according to a probability constant
    /// </summary>
    public void RandomlyClickUnoButton()
    {
        float prob = Random.Range(0f, 1f);

        if( prob <= Constants.BotUnoProbability )
        {
            isClickedUno = true;
            UIManager.Instance.InGameUIController.OpenUnoPanel();
        }
    }

    /// <summary>
    /// Scans each card in the bot deck and checks if it is playable
    /// </summary>
    private void FindPlayableCardsBot()
    {
        int numOfPlayableCards = 0;

        // Resets card color counter in each turn
        ResetCardColorCounter();

        // Checks each card if it is playable
        for (int i = 0; i < BotDeck.Count; i++)
        {
            Card card = BotDeck[i];

            if (card.CardColorGameplay == GameManager.Instance.CurrentCardColor)
            {
                PlayableBotCards.Add(card);
                numOfPlayableCards++;
            }
            else if (card.CardNumber == GameManager.Instance.CurrentCardNumber)
            {
                PlayableBotCards.Add(card);
                numOfPlayableCards++;
            }

            // Increment card color counter's corresponding value
            CountCardColors(card);
        
        }

        // Checks if wild cards can be playable
        if (numOfPlayableCards == 0)
        {
            for(int i = 0; i < BotDeck.Count; i++)
            {
                Card card = BotDeck[i];

                if(card.CardType == CardTypeEnums.WildCard)
                {
                    PlayableBotCards.Add(card);
                    numOfPlayableCards++;
                }
            }
        }

        // Checks if wild 4 card can be playable
        if (numOfPlayableCards == 0)
        {
            for(int i = 0; i < BotDeck.Count; i++)
            {
                Card card = BotDeck[i];

                if(card.CardType == CardTypeEnums.WildDrawFourCard)
                {
                    PlayableBotCards.Add(card);
                    numOfPlayableCards++;
                }
            }
        }
    }

    /// <summary>
    /// Resets playable cards list
    /// </summary>
    private void ResetPlayableCards()
    {
        for(int i = (PlayableBotCards.Count - 1); i >= 0; i--)
        {
            PlayableBotCards.RemoveAt(i);
        }
    }

    /// <summary>
    /// Increments given card's color type in the color counter and point counter
    /// </summary>
    private void CountCardColors(Card cardToCount)
    {
        for (int j = 0; j < 4; j++)
        {
            if (cardToCount.CardColorGameplay == GameManager.Instance.CardColors[j])
            {
                ColorCounter[j]++;
                PointCounter[j] = PointCounter[j] + cardToCount.CardNumber;
            }
        }
    }

    /// <summary>
    /// Resets card color counter and point counter
    /// </summary>
    private void ResetCardColorCounter()
    {
        for(int i = 0; i < 4; i++)
        {
            ColorCounter[i] = 0;
            PointCounter[i] = 0;
        }
    }

    /// <summary>
    /// Plays given card and sets current card attributes in the game manager
    /// </summary>
    private void PlayCardBot(Card cardToPlay)
    {
        Debug.Log("Bot played:" + cardToPlay.CardColorGameplay + cardToPlay.CardNumber);
        ControllerReference.Instance.DrawPileController.SetDiscardedCardPosition(cardToPlay);
        GameManager.Instance.SetCurrentCardAttributes(cardToPlay);

        // If the played card is wild card, choose a color by looking at the maximum number of cards in the each color type
        if (cardToPlay.CardType == CardTypeEnums.WildCard || cardToPlay.CardType == CardTypeEnums.WildDrawFourCard)
        {
            int maxIndex = 0;
            for (int i = 1; i < 4; i++)
            {
                if (ColorCounter[i] > ColorCounter[maxIndex])
                {
                    maxIndex = i;
                }
                else if (ColorCounter[i] == ColorCounter[maxIndex])
                {
                    // If there are equal number of cards for two different colors, check total card values
                    if (PointCounter[i] > PointCounter[maxIndex])
                    {
                        maxIndex = i;
                    }
                }
            }

            // Set the new color according to previous calculations
            GameManager.Instance.CurrentCardColor = GameManager.Instance.CardColors[maxIndex];
        }

        ResetPlayableCards();
        cardToPlay.MoveCardToDiscardPile();
        BotDeck.Remove(cardToPlay);
        SortBotHand();
    }

    /// <summary>
    /// Loads bot's hand from saved gamedata file
    /// </summary>
    public void LoadBotHand(int[] cardIDs)
    {
        for (int i = 0; i < cardIDs.Length; i++)
        {
            BotDeck.Add(ControllerReference.Instance.DeckController.Deck[cardIDs[i] - 1]);

            BotDeck[i].IsInDrawPile = false;

            BotDeck[i].transform.parent = BotHandSlot;

            BotDeck[i].transform.position = BotHandSlot.position;

            BotDeck[i].transform.localEulerAngles = new Vector3(0, 0, 0);
        }

        SortBotHand();
    }
}