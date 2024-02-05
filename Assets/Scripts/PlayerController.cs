using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngineInternal;

public class PlayerController : MonoBehaviour
{
    public Transform PlayerHandSlot;

    private Vector3 OffSetOne = Constants.OffSetValueOne + new Vector3(0, 0, -0.01f);
    private Vector3 OffSetTwo = Constants.OffSetValueTwo + new Vector3(0, 0, -0.01f);
    private Vector3 OffSetThree = Constants.OffSetValueThree + new Vector3(0, 0, -0.01f);

    public List<Card> PlayerDeck = new List<Card>();

    public bool isClickedUno = false;

    public float PlayerTimer;

    /// <summary>
    /// Adds cards to the player deck in the given list of seven cards
    /// </summary>
    public void DrawSevenCards(List<Card> sevenCards)
    {
        for (int i = 0; i < 7; i++)
        {
            PlayerDeck.Add(sevenCards[i]);
            PlayerDeck[i].transform.parent = PlayerHandSlot;

            PlayerDeck[i].CardBack.SetActive(false);
            PlayerDeck[i].CardFront.SetActive(true);
        }

        SortPlayerHand();
    }

    /// <summary>
    /// Draws one card from the draw pile and adds to the player deck
    /// </summary>
    private void DrawOneCardPlayer()
    {
        Card cardToDraw = ControllerReference.Instance.DrawPileController.DrawPileDeck[0];

        PlayerDeck.Add(cardToDraw);
        cardToDraw.transform.parent = PlayerHandSlot;
        cardToDraw.IsInDrawPile = false;
        cardToDraw.CardFront.SetActive(true);
        cardToDraw.CardBack.SetActive(false);

        ControllerReference.Instance.DrawPileController.DrawPileDeck.Remove(cardToDraw);

        SortPlayerHand();
    }

    /// <summary>
    /// Sorts player hand and sets cards' position according the number of cards in the player deck
    /// </summary>
    private void SortPlayerHand()
    {
        Vector3 currentCardPosition = new Vector3(0, 0, 0);
        Vector3 marginVector = new Vector3(0, 0, 0);

        if (PlayerDeck.Count <= Constants.NumOfCardOffSetOne)
        {
            marginVector = OffSetOne;
        }
        else if (PlayerDeck.Count <= Constants.NumOfCardOffSetTwo)
        {
            marginVector = OffSetTwo;
        }
        else if (PlayerDeck.Count <= Constants.NumOfCardOffSetThree)
        {
            marginVector = OffSetThree;
        }

        // Calculates start of the hand position
        currentCardPosition = currentCardPosition - (((PlayerDeck.Count - 1) * marginVector) / 2); 

        // Sets each card's position
        for (int i = 0; i < PlayerDeck.Count; i++)
        {
            PlayerDeck[i].GetComponent<SortingGroup>().sortingOrder = i;

            PlayerDeck[i].transform.localEulerAngles = new Vector3(0, 0, 0);
            PlayerDeck[i].MoveCardToHand(currentCardPosition, i * Constants.HandSortingTimeMultiplier);
            currentCardPosition = currentCardPosition + marginVector;
        }
    }

    /// <summary>
    /// Scans each card in the player hand and decides if it is playable
    /// </summary>
    public void FindPlayableCards()
    {
        int numOfPlayableCards = 0;

        for(int i = 0; i < PlayerDeck.Count; i++)
        {
            Card card = PlayerDeck[i];

            if(card.CardColorGameplay == GameManager.Instance.CurrentCardColor)
            {
                card.IsPlayable = true;
                numOfPlayableCards++;
            }
            else if(card.CardNumber == GameManager.Instance.CurrentCardNumber)
            {
                card.IsPlayable = true;
                numOfPlayableCards++;
            }
            else if(card.CardType == CardTypeEnums.WildCard)
            {
                card.IsPlayable = true;
                numOfPlayableCards++;
            }
            else
            {
                card.IsPlayable = false;
            }

            card.CardPlayableIcon.gameObject.SetActive(card.IsPlayable);
        }

        // Checks if wild 4 card can be playable
        if(numOfPlayableCards == 0)
        {
            for(int i = 0; i < PlayerDeck.Count; i++)
            {
                Card card = PlayerDeck[i];

                if(card.CardType == CardTypeEnums.WildDrawFourCard)
                {
                       card.IsPlayable = true;
                       card.CardPlayableIcon.gameObject.SetActive(true);
                }
            }
        }

    }

    /// <summary>
    /// Plays the given card and sets current card attributes in the game manager
    /// </summary>
    private void PlayClickedCard(Card cardToPlay)
    {
        Debug.Log("You played:" + cardToPlay.CardColorGameplay + cardToPlay.CardNumber);

        ControllerReference.Instance.DrawPileController.SetDiscardedCardPosition(cardToPlay);
        GameManager.Instance.SetCurrentCardAttributes(cardToPlay);

        cardToPlay.CardPlayableIcon.gameObject.SetActive(false);
        cardToPlay.IsPlayable = false;
        cardToPlay.MoveCardToDiscardPile();
        PlayerDeck.Remove(cardToPlay);

        SortPlayerHand();

        // Checks if the played card is a wild card
        if (cardToPlay.CardType == CardTypeEnums.WildCard || cardToPlay.CardType == CardTypeEnums.WildDrawFourCard)
        {
            // EndTurn method is called at buttons
            UIManager.Instance.InGameUIController.ShowWildCardPopUp();
        }
        else
        {
            GameManager.Instance.EndTurn();
        }
    }

    /// <summary>
    /// If the mouse button is clicked send a Raycast and check the clicked card
    /// </summary>
    public void CheckClickedCard()
    {
#if UNITY_EDITOR
        if(Input.GetMouseButtonDown(0))
        {
            CheckRaycast(Input.mousePosition);
        }
#endif

#if UNITY_ANDROID
        

        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckRaycast(Input.GetTouch(0).position);
        }
#endif

    }

    /// <summary>
    /// Draws two cards for player
    /// </summary>
    public void DrawTwoCardsPlayer()
    {
        DrawOneCardPlayer();
        DrawOneCardPlayer();
    }

    /// <summary>
    /// Calculates player hand's score and returns it
    /// </summary>
    public int CalculatePlayerHand()
    {
        int scoreAccumuator = 0;
        for (int i = 0; i < PlayerDeck.Count; i++)
        {
            Card card = PlayerDeck[i];

            if (card.CardNumber <= 9)
            {
                scoreAccumuator += card.CardNumber;
            }
            else if (card.CardNumber <= 12)
            {
                scoreAccumuator += Constants.ActionCardPoints;
            }
            else if (card.CardNumber <= 14)
            {
                scoreAccumuator += Constants.WildCardPoints;
            }
        }

        return scoreAccumuator;
    }

    /// <summary>
    /// Plays the drawn card which is the last element of the player deck
    /// </summary>
    public void PlayDrawnCard()
    {
        PlayClickedCard(PlayerDeck[PlayerDeck.Count - 1]);
    }

    /// <summary>
    /// Call raycast to given vector and check if it hit a playable card
    /// </summary>
    private void CheckRaycast(Vector3 positionVector)
    {

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(positionVector), Vector2.zero);

        if (hit.collider != null)
        {

#if UNITY_EDITOR
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                ControlHitRaycast(hit);
            }
#endif

#if UNITY_ANDROID
            if(!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                ControlHitRaycast(hit);
            }
#endif

        }
    }

    /// <summary>
    /// Controls the successfull hit if it is playable or drawable
    /// </summary>
    private void ControlHitRaycast(RaycastHit2D hit)
    {
        Card clickedCard = hit.collider.GetComponent<Card>();

        if (clickedCard.IsPlayable)
        {
            // If the clicked card is playable, plays it
            PlayClickedCard(clickedCard);
        }
        else if (clickedCard.IsInDrawPile)
        {
            // If the clicked card is in draw pile, draw a card and check if it is playable
            DrawOneCardPlayer();
            FindPlayableCards();
            if (PlayerDeck[PlayerDeck.Count - 1].IsPlayable)
            {
                UIManager.Instance.InGameUIController.ShowDrawnCardPopUp();
            }
            else
            {
                GameManager.Instance.EndTurn();
            }
        }
    }

    /// <summary>
    /// Loads player's hand from the saved gamedata file
    /// </summary>
    public void LoadPlayerHand()
    {
        for (int i = 0; i < GameDataSingleton.Instance.PlayerCardIDs.Length; i++)
        {
            PlayerDeck.Add(ControllerReference.Instance.DeckController.Deck[GameDataSingleton.Instance.PlayerCardIDs[i] - 1]);

            PlayerDeck[i].IsInDrawPile = false;

            PlayerDeck[i].transform.parent = PlayerHandSlot;

            PlayerDeck[i].CardBack.SetActive(false);
            PlayerDeck[i].CardFront.SetActive(true);
        }

        SortPlayerHand();
    }
}
