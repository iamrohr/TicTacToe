using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System;
using System.Runtime.CompilerServices;
using UnityEditor.Build.Content;
public class GameData : MonoBehaviour
{
    private static GameData _instance;
    public static GameData Instance { get { return _instance; } }
    
    public UserInfo userData;
    public GameInfo gameData;
    public GamePlayer gamePlayer;

    public string userID;

    private void Awake()
    {
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

    public void OnSignIn(string userID)
    {
        //Try to load user data
        this.userID = userID;
        SaveAndLoadManager.Instance.LoadData("users/" + userID, OnLoadData);
    }
    
    void OnLoadData(string json)
    {
        if (json != null)
        {
            //Updates data for user, name and active games etc
            userData = JsonUtility.FromJson<UserInfo>(json);
        }

        //If user Empty, Create new user with "base" values.
        userData ??= new UserInfo();
        SaveUserData();
        FindObjectOfType<FirebaseLogin>()?.PlayerDataLoaded();
    }

    public void SaveUserData()
    {
        userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        SaveAndLoadManager.Instance.SaveData("users/" + this.userID, JsonUtility.ToJson(userData));
    }
    
}
    
