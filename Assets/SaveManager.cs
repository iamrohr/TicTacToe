using System;
using System.IO;
using System.Text;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    //Singelton instantiation
    private static SaveManager _instance;
    public static SaveManager instance
    {
        get { return _instance; }
    }
    
    private void Awake()
    {
        //Singelton
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    
    
    
    public PlayerSaveData playerSaveData;
    private Transform player;
    private const string saveFileName = "SaveData.json";
    
    private void Start()
    {
        playerSaveData = new PlayerSaveData();
        LoadData();
    }

    
    public void Save()
    {
        string jsonString = JsonUtility.ToJson(playerSaveData);
        
        SaveToFile(saveFileName, jsonString);
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
    
    
    private void LoadData()
    {
        try
        {
            string loadedString = LoadFromFile(saveFileName);
            playerSaveData = JsonUtility.FromJson<PlayerSaveData>(loadedString);
        }
        catch (Exception)
        {
            Debug.Log("No data found");
            playerSaveData.name = "Player";
            playerSaveData.colorHUE = 0;
        }
    }

    public string LoadFromFile(string fileName)
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

