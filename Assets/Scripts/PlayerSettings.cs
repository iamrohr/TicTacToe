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
        LoadPlayerColor();
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
            SaveManager.instance.playerSaveData.name = "Player";
            SaveManager.instance.Save();
            playerName.text = SaveManager.instance.playerSaveData.name;
            Debug.Log("Empty");
 
        }
        else
        {
            SaveManager.instance.playerSaveData.name = playerNameInputField.text;
            SaveManager.instance.Save();
            playerName.text = SaveManager.instance.playerSaveData.name;
        }

        //Save Player Color
        SaveManager.instance.playerSaveData.colorHUE = playerColorSlider.value;
        SaveManager.instance.Save();
    }

    private void LoadPlayerColor()
    {
        playerColorSlider.value = SaveManager.instance.playerSaveData.colorHUE;
    }
    
    
}
