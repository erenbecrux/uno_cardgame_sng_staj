using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    public Image SoundEffectsToggleBackground;
    public Sprite SoundEffectsOff;
    public Sprite SoundEffectsOn;

    private void Start()
    {
        // check the current status of sound effects
        if(GameDataSingleton.Instance.IsSoundEffectsOn)
        {
            SoundEffectsToggleBackground.sprite = SoundEffectsOn;
        }
        else if(!GameDataSingleton.Instance.IsSoundEffectsOn)
        {
            SoundEffectsToggleBackground.sprite= SoundEffectsOff;
        }
        
    }

    /// <summary>
    /// If the sound effects toggle is clicked, change current mode
    /// </summary>
    public void OnClickSoundEffectsToggle()
    {
        if (GameDataSingleton.Instance.IsSoundEffectsOn)
        {
            SoundEffectsToggleBackground.sprite = SoundEffectsOff;
            GameDataSingleton.Instance.IsSoundEffectsOn = false;
        }
        else if (!GameDataSingleton.Instance.IsSoundEffectsOn)
        {
            SoundEffectsToggleBackground.sprite = SoundEffectsOn;
            GameDataSingleton.Instance.IsSoundEffectsOn = true;
        }
    }
}
