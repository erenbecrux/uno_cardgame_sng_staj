using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public int CardID;
    public int CardNumber;

    public Color SpriteColor;

    public bool HasCardIcon;
    public bool IsWildCard;

    public GameObject MainCardColor;

    public GameObject CardIconTop;
    public GameObject CardIconMain;
    public GameObject CardIconBottom;

    public TextMeshPro CardTextTop;
    public TextMeshPro CardTextMain;
    public TextMeshPro CardTextBottom;

    public GameObject CardFront;
    public GameObject CardBack;
    public GameObject CardPlayableIcon;

    public CardTypeEnums CardType;
    public CardColorEnums CardColorGameplay;

    public bool IsPlayable = false;
    public bool IsInDrawPile = false;

    /// <summary>
    /// Moves card to the discard pile position
    /// </summary>
    public void MoveCardToDiscardPile()
    {
        this.gameObject.transform.DOMove(ControllerReference.Instance.DrawPileController.DiscardPileSlot.position, Constants.CardMoveTime);
    }

    /// <summary>
    /// Moves card to the given hand position in card move time plus given time. Also plays card movement sound fx
    /// </summary>
    public void MoveCardToHand(Vector3 hand, float time)
    {
        this.gameObject.transform.DOLocalMove(hand, Constants.CardMoveTime + time);
        AudioManager.Instance.PlayCardClip();
    }

    /// <summary>
    /// Moves card to the draw pile position in given time. Also plays card movement fx
    /// </summary>
    public void MoveCardToDrawPile(float time)
    {
        this.gameObject.transform.DOMove(ControllerReference.Instance.DrawPileController.DrawPileSlot.position, time);
        AudioManager.Instance.PlayCardClip();
    }

    /// <summary>
    /// Moves card to discard pile then rotates it to open its face
    /// </summary>
    public IEnumerator RotateCardFace()
    {
        MoveCardToDiscardPile();
        yield return new WaitForSeconds(Constants.CardMoveTime);

        this.gameObject.transform.DORotate(new Vector2(0, -90), Constants.CardMoveTime);
        yield return new WaitForSeconds(Constants.CardMoveTime);

        this.CardFront.SetActive(true);
        this.CardBack.SetActive(false);

        this.gameObject.transform.DORotate(new Vector2(0, 0), Constants.CardMoveTime);
    }
}

