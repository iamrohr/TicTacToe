using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;

public class SignOut : MonoBehaviour
{
    public Button signOutButton;

    public void Start()
    {
        signOutButton.onClick.AddListener(() => SignOutUser());
    }

    public void SignOutUser()
    {
        GameData.Instance.userData = null;
        GameData.Instance.gameData = null;
        FirebaseAuth.DefaultInstance.SignOut();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
