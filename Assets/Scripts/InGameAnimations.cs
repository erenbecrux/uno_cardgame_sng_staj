using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameAnimations : MonoBehaviour
{
    public void CloseUnoPanel()
    {
        // Attach this script to uno panel
        this.gameObject.SetActive(false);
    }
    
}
