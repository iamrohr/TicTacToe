using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class PlayerLoadData
{
    public string name;
    public float colorHue;
}

public class PlayerSettingsLoad : MonoBehaviour
{
    //Save JSON
    public PlayerLoadData myLoadData;
    public string loadJasonString;
    
    void Start()
    {
        // myLoadData = PlayerData.data;
        string loadedString = Load("SaveData.json");
        myLoadData = JsonUtility.FromJson<PlayerLoadData>(loadedString);
    }
    
    public string Load(string fileName)
    {
        // Open a stream for the supplied file name as a text file
        using (var stream = File.OpenText(fileName))
        {
            // Read the entire file and return the result. This assumes that we've written the
            // file in UTF-8
            return stream.ReadToEnd();
        }
    }
}
