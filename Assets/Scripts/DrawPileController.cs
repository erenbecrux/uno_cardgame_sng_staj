using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class DrawPileController : MonoBehaviour
{
    public Transform DrawPileSlot;
    public Transform DiscardPileSlot;

    public List<Card> DrawPileDeck = new List<Card>();
    public List<Card> DiscardPileDeck = new List<Card>();

    private int discardPileSortingOrder = 0;
    private int drawPileSortingOrder = 0;

    /// <summary>
    /// Sets draw pile at the start of the game
    /// </summary>
    public void SetDrawPile(List<Card> remainingCards)
    {
        int remainingCardCount = remainingCards.Count;

        for(int i = 0; i < remainingCardCount; i++)
        {
            DrawPileDeck.Add(remainingCards[i]);

            DrawPileDeck[i].IsInDrawPile = true;

            DrawPileDeck[i].transform.parent = DrawPileSlot;

            DrawPileDeck[i].GetComponent<SortingGroup>().sortingOrder = drawPileSortingOrder;
            drawPileSortingOrder++;

            DrawPileDeck[i].MoveCardToDrawPile(Constants.DrawPileSettingTimeMultiplier*i);

            DrawPileDeck[i].transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    /// <summary>
    /// Draws starting card from draw pile at the start of the each round
    /// </summary>
    public void DrawStartingCard()
    {
        Card card = DrawPileDeck[0];
        DrawPileDeck.Remove(card);
        GameManager.Instance.SetCurrentCardAttributes(card);

        // Checks if the drawn card is an action card
        if (card.CardType == CardTypeEnums.ReverseCard)
        {
            GameManager.Instance.ReverseGameplayRotation();
        }
        else if (card.CardType == CardTypeEnums.SkipCard)
        {
            GameManager.Instance.SetNextPlayerTurnState();
        }
        else if (card.CardType == CardTypeEnums.DrawTwoCard)
        {
            // Draws two cards for the first player
            if (GameManager.Instance.TurnState == TurnState.PlayerTurn)
            {
                ControllerReference.Instance.PlayerController.DrawTwoCardsPlayer();
            }
            else if (GameManager.Instance.TurnState == TurnState.RightBotTurn)
            {
                ControllerReference.Instance.RightBotController.DrawTwoCardsBot();
            }
            else if (GameManager.Instance.TurnState == TurnState.OppositeBotTurn)
            {
                ControllerReference.Instance.OppositeBotController.DrawTwoCardsBot();
            }
            else if (GameManager.Instance.TurnState == TurnState.LeftBotTurn)
            {
                ControllerReference.Instance.LeftBotController.DrawTwoCardsBot();
            }

            // Skips turn
            GameManager.Instance.SetNextPlayerTurnState();
        }
        else if (card.CardType == CardTypeEnums.WildCard)
        {
            // EndTurn method is called at buttons
            UIManager.Instance.InGameUIController.ShowWildCardPopUp();
        }
        else if (card.CardType == CardTypeEnums.WildDrawFourCard)
        {
            // If the first card is wild draw four card, put it into the draw pile and draw another card
            DrawPileDeck.Add(card);
            card = DrawPileDeck[0];
            DrawPileDeck.Remove(card);
            GameManager.Instance.SetCurrentCardAttributes(card);
        }

        DiscardPileDeck.Add(card);

        card.IsInDrawPile = false;

        card.transform.parent = DiscardPileSlot;

        StartCoroutine(card.RotateCardFace());

        card.GetComponent<SortingGroup>().sortingOrder = discardPileSortingOrder;
        discardPileSortingOrder++;

    }

    /// <summary>
    /// Sets discarded card's attributes and its position according to given card
    /// </summary>
    public void SetDiscardedCardPosition(Card cardToDiscard)
    {
        cardToDiscard.CardFront.SetActive(true);
        cardToDiscard.CardBack.SetActive(false);

        cardToDiscard.gameObject.transform.parent = DiscardPileSlot;
        cardToDiscard.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);

        cardToDiscard.GetComponent<SortingGroup>().sortingOrder = discardPileSortingOrder;
        discardPileSortingOrder++;

        DiscardPileDeck.Add(cardToDiscard);
    }

    /// <summary>
    /// Shuffles discard pile and puts shuffled deck into draw pile
    /// </summary>
    public void ShuffleDiscardPile()
    {
        int remainingCards = DiscardPileDeck.Count - 1;
        int loopCount = DiscardPileDeck.Count - 1;
        int cardsInDeck = DrawPileDeck.Count;

        for (int i = 0; i < loopCount; i++)
        {
            int randomIndex = Random.Range(0, remainingCards);
            DrawPileDeck.Add(DiscardPileDeck[randomIndex]);
            DiscardPileDeck.RemoveAt(randomIndex);
            remainingCards--;

            DrawPileDeck[i+cardsInDeck].CardFront.SetActive(false);
            DrawPileDeck[i+cardsInDeck].CardBack.SetActive(true);
            DrawPileDeck[i+cardsInDeck].IsInDrawPile = true;
            DrawPileDeck[i+cardsInDeck].transform.parent = DrawPileSlot;
            DrawPileDeck[i+cardsInDeck].transform.position = DrawPileSlot.position;
        }
    }

    /// <summary>
    /// Loads discard pile from saved gamedata file
    /// </summary>
    public void LoadDiscardPile()
    {
        for(int i = 0; i < GameDataSingleton.Instance.DiscardPileIDs.Length; i++)
        {
            DiscardPileDeck.Add(ControllerReference.Instance.DeckController.Deck[GameDataSingleton.Instance.DiscardPileIDs[i] - 1]);

            DiscardPileDeck[i].IsInDrawPile = false;

            DiscardPileDeck[i].CardBack.SetActive(false);
            DiscardPileDeck[i].CardFront.SetActive(true);

            DiscardPileDeck[i].transform.parent = DiscardPileSlot;

            DiscardPileDeck[i].transform.position = DiscardPileSlot.position;

            DiscardPileDeck[i].transform.localEulerAngles = new Vector3(0, 0, 0);

            DiscardPileDeck[i].GetComponent<SortingGroup>().sortingOrder = discardPileSortingOrder;
            discardPileSortingOrder++;
        }

    }

    /// <summary>
    /// Loads draw pile from saved gamedata file
    /// </summary>
    public void LoadDrawPile()
    {
        for (int i = 0; i < GameDataSingleton.Instance.DrawPileIDs.Length; i++)
        {
            DrawPileDeck.Add(ControllerReference.Instance.DeckController.Deck[GameDataSingleton.Instance.DrawPileIDs[i] - 1]);

            DrawPileDeck[i].IsInDrawPile = true;

            DrawPileDeck[i].transform.parent = DrawPileSlot;

            DrawPileDeck[i].transform.position = DrawPileSlot.position;

            DrawPileDeck[i].transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }
}
