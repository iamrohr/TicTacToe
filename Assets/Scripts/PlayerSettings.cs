using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class PlayerSettings : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public TMP_InputField playerNameInputField;
    
    public UnityEngine.UI.Slider playerColorSlider;
    public UnityEngine.UI.Image player;

    void Start()
    {
        LoadPlayerSettings();
        playerColorSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

    }

    public void ValueChangeCheck()
    {
        //Change Player color when slider is used 
        player.color = Color.HSVToRGB(playerColorSlider.value, 1, 1);
    }

    public void SavePlayerSettings()
    {
        //Save Player Name And Update On Screen
        if (string.IsNullOrWhiteSpace(playerNameInputField.text))
        {
            PlayerData.data.name = "Player";
            PlayerData.SaveData();
            // SaveManager.Instance.playerSaveData.name = "Player";
            // SaveManager.Instance.Save();
            // playerName.text = SaveManager.Instance.playerSaveData.name;
            playerName.text = PlayerData.data.name;
        }
        else
        {
            PlayerData.data.name = playerNameInputField.text;
            PlayerData.SaveData();
            // SaveManager.Instance.playerDa.name = playerNameInputField.text;
            // SaveManager.Instance.Save();
            // playerName.text = SaveManager.Instance.playerSaveData.name;
            playerName.text = PlayerData.data.name;
        }

        //Save Player Color
        PlayerData.data.colorHue = playerColorSlider.value;
        PlayerData.SaveData();
        // SaveManager.Instance.playerSaveData.colorHUE = playerColorSlider.value;
        // SaveManager.Instance.Save();
    }

    private void LoadPlayerSettings()
    {
        playerColorSlider.value = PlayerData.data.colorHue;
        PlayerData.data.name = playerNameInputField.text;
        // playerColorSlider.value = SaveManager.Instance.playerSaveData.colorHUE;
    }
    
    
    
    
}
