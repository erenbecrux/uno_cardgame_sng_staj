using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource SFXSource;

    public AudioClip CardClip;
    public AudioClip WinClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Plays card sfx if sound effects are on
    /// </summary>
    public void PlayCardClip()
    {
        if(GameDataSingleton.Instance.IsSoundEffectsOn)
        {
            SFXSource.PlayOneShot(CardClip, 1f);
        }       
    }

    /// <summary>
    /// Plays win sfx if sound effects are on
    /// </summary>
    public void PlayWinClip()
    {
        if(GameDataSingleton.Instance.IsSoundEffectsOn)
        {
            SFXSource.PlayOneShot(WinClip);
        }    
    }
}
