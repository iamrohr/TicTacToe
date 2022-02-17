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
            GameData.Instance.userData.name = "Player";
            GameData.Instance.SaveUserData();
            // SaveManager.Instance.playerSaveData.name = "Player";
            // SaveManager.Instance.Save();
            // playerName.text = SaveManager.Instance.playerSaveData.name;
            playerName.text = GameData.Instance.userData.name;
        }
        else
        {
            GameData.Instance.userData.name = playerNameInputField.text;
            GameData.Instance.SaveUserData();
            // SaveManager.Instance.playerDa.name = playerNameInputField.text;
            // SaveManager.Instance.Save();
            // playerName.text = SaveManager.Instance.playerSaveData.name;
            playerName.text =  GameData.Instance.userData.name;
        }

        //Save Player Color
        GameData.Instance.userData.colorHue = playerColorSlider.value;
        GameData.Instance.SaveUserData();
        // SaveManager.Instance.playerSaveData.colorHUE = playerColorSlider.value;
        // SaveManager.Instance.Save();
    }

    private void LoadPlayerSettings()
    {
        playerColorSlider.value =  GameData.Instance.userData.colorHue;
        player.color = Color.HSVToRGB( GameData.Instance.userData.colorHue, 1, 1);
        playerName.text =  GameData.Instance.userData.name;
        // playerColorSlider.value = SaveManager.Instance.playerSaveData.colorHUE;
    }
    
    
    
    
}
