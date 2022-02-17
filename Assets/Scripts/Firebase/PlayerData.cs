using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System;

    [Serializable]
    public class UserInfo
    {
        public string name;
        public float colorHue;
    }

public class PlayerData : MonoBehaviour
{
    public static UserInfo data; 
    static string userPath;
    
    void Start()
    {
        FindObjectOfType<FirebaseLogin>().OnSignIn += OnSignIn;
        // userPath = "users/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    }

    void OnSignIn()
    {
        Debug.Log("On Sign In Triggered");
        userPath = "users/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        SaveManager.Instance.LoadData(userPath, OnLoadData);
    }

    void OnLoadData(string json)
    {
        if (json != null)
        {
            data = JsonUtility.FromJson<UserInfo>(json);
        }
        else
        {
            data = new UserInfo();
            SaveData();
        }
        
        FindObjectOfType<FirebaseLogin>()?.PlayerDataLoaded();
        
    }

    public static void SaveData()
    {
        SaveManager.Instance.SaveData(userPath, JsonUtility.ToJson(data));
    }
    
}
