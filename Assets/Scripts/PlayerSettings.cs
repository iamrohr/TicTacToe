using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class PlayerSettings : MonoBehaviour
{
    private const string key_sliderColor = "key_sliderColor";
    
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerNameInputField;
    
    public UnityEngine.UI.Slider playerColorSlider;
    public UnityEngine.UI.Image player;

    void Start()
    {
        playerColorSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        LoadSettings();
    }

    public void SavePlayerSettings()
    {
        //Save Player Name And Update On Screen
        if (!string.IsNullOrEmpty(playerNameInputField.text))
        {
            PlayerPrefs.SetString("pName", "Player");
            playerName.text = PlayerPrefs.GetString("pName", "Player");
        }
        else
        {
            PlayerPrefs.SetString("pName", playerNameInputField.text);
            playerName.text = PlayerPrefs.GetString("pName", "Player");
        }
        
        
        //Save Player Color
        PlayerPrefs.SetFloat(key_sliderColor, playerColorSlider.value);
    }

    public void ValueChangeCheck()
    {
        //Change Player color when slider is used 
        player.color = Color.HSVToRGB(playerColorSlider.value, 1, 1);
    }

    public void LoadSettings()
    {
        //Load Player Name
        playerName.text = PlayerPrefs.GetString("pName", "Player");
        
        //Load Player Color 
        playerColorSlider.value = PlayerPrefs.GetFloat(key_sliderColor);
        player.color = Color.HSVToRGB(playerColorSlider.value, 1, 1);
    }

}
