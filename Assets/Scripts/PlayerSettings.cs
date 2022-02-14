using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

[Serializable]
public class PlayerSaveData
{
    public string name;
    public float colorHue;
}

public class PlayerSettings : MonoBehaviour
{
    private const string key_sliderColor = "key_sliderColor";
    
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerNameInputField;
    
    public UnityEngine.UI.Slider playerColorSlider;
    public UnityEngine.UI.Image player;
    
    //Save JSON
    public PlayerSaveData mySaveData;
    public string jsonString;

    void Start()
    {
        // MyLoadedData();
        playerColorSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        LoadSettings();
        MySaveJsonData();

    }

    public void SavePlayerSettings()
    {
        // //Save Player Name And Update On Screen
        // if (!string.IsNullOrEmpty(playerNameInputField.text))
        // {
        //     PlayerPrefs.SetString("pName", "Player");
        //     playerName.text = PlayerPrefs.GetString("pName", "Player");
        // }
        // else
        // {
            PlayerPrefs.SetString("pName", playerNameInputField.text);
            playerName.text = PlayerPrefs.GetString("pName", "Player");
        // }

        //Save Player Color
        PlayerPrefs.SetFloat(key_sliderColor, playerColorSlider.value);
      
        //Save all to JSON & PlayerPrefs
        //Create JSON
        mySaveData = new PlayerSaveData();
        MySaveJsonData();
        jsonString = JsonUtility.ToJson(mySaveData);

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

    public void MySaveJsonData()
    {
        //Data to be saved
        mySaveData.name = PlayerPrefs.GetString("pName");
        mySaveData.colorHue = PlayerPrefs.GetFloat(key_sliderColor);
  
        //Save to string with PlayerPrefs
        PlayerPrefs.SetString("PlayerSaveData", jsonString);
        
        //Save to json.file
        SaveToFile("SaveData.json", jsonString);
        
        Debug.Log("Data saved " + jsonString);
    }

    public void MyLoadedData()
    {
        PlayerSaveData myLoadedData;

        try
        {
            myLoadedData = JsonUtility.FromJson<PlayerSaveData>(jsonString);
            
            Debug.Log("Data loaded JSON." + jsonString);
        }
        catch (Exception)
        {
            Debug.Log("Data failed to load, setting default values.");
            
            myLoadedData = new PlayerSaveData();
            myLoadedData.name = "Player";
            myLoadedData.colorHue = 0f;
        }
    }
    
    public void SaveToFile(string fileName, string jsonString)
    {
        // Open a file in write mode. This will create the file if it's missing.
        // It is assumed that the path already exists.
        using (var stream = File.OpenWrite(fileName))
        {
            // Truncate the file if it exists (we want to overwrite the file)
            stream.SetLength(0);

            // Convert the string into bytes. Assume that the character-encoding is UTF8.
            // Do you not know what encoding you have? Then you have UTF-8
            var bytes = Encoding.UTF8.GetBytes(jsonString);

            // Write the bytes to the hard-drive
            stream.Write(bytes, 0, bytes.Length);

            // The "using" statement will automatically close the stream after we leave
            // the scope - this is VERY important
        }
    }
    
    
}
