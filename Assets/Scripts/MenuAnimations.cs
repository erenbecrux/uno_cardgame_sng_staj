using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimations : MonoBehaviour
{
    /// <summary>
    /// Used for closing ui pop ups in the animator events except store animations
    /// </summary>
    public void CloseCanvasAfterAnimation()
    {
        this.GetComponentInParent<Canvas>().gameObject.SetActive(false);
    }

    /// <summary>
    /// Used for closing store pop ups in the animator events
    /// </summary>
    public void CloseThisObjectAfterAnimation()
    {
        this.gameObject.SetActive(false);
    }
}
