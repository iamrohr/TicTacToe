using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseLogin : MonoBehaviour
{
    //Shows user whats happening
    public TextMeshProUGUI outputText;

    //Buttons Loby
    public Button playButton;
    public Button signInButton;
    public Button registerButton;

    //Login Fields
    public TMP_InputField username;
    public TMP_InputField password;

    //Delegate
    public delegate void SignInHandler();
    public SignInHandler OnSignIn;
    
    private FirebaseAuth auth;


    // Start is called before the first frame update
    void Start()
    {
        //Tun first scene
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            //Log if we get any errors from the opperation
            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
            }

            auth = FirebaseAuth.DefaultInstance;
        });

        //Disable button until we have logged in
        playButton.interactable = false;

        signInButton.onClick.AddListener(() => SignIn(username.text, password.text));
        registerButton.onClick.AddListener(() => RegisterNewUser(username.text, password.text));
    }


    private void RegisterNewUser(string email, string password)
    {
        Debug.Log("Starting Registration");
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogWarning(task.Exception);
                outputText.text = task.Exception.InnerExceptions[0].InnerException.Message;
            }
            else
            {
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("User Registerd: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
                SignedIn(newUser);
            }
        });
    }

    private void SignIn(string email, string password)
    {
        Debug.Log("Sign in i FirebaseLogin.cs");
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogWarning(task.Exception);
                outputText.text = task.Exception.InnerExceptions[0].InnerException.Message;
            }
            else
            {
                Debug.Log("I else på FirebaseLogin.cs");
                SignedIn(task.Result);
            }
        });
    }

    void AnonymousSignIn()
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogWarning(task.Exception);
                outputText.text = task.Exception.InnerExceptions[0].InnerException.Message;
            }
            else
            {
                {
                    SignedIn(task.Result);
                }
            }
        });
    }

    void SignedIn(FirebaseUser newUser)
    {
        Debug.LogFormat("User signed in successfully: {0} ({1})",
            newUser.DisplayName, newUser.UserId);

        //Display who logged in
        if (newUser.DisplayName != "")
            outputText.text = "Logged in as: " + newUser.DisplayName + PlayerData.data;
        else if (newUser.Email != "")
            outputText.text = "logged in as: " + newUser.Email + PlayerData.data;
        else
            outputText.text = "Logged in as: Anonymous User " + newUser.UserId.Substring(0, 6);
        
        OnSignIn?.Invoke();
    }

    public void PlayerDataLoaded()
    {
        playButton.interactable = true;
    }


    void SaveToFirebase(string data)
    {
        var db = FirebaseDatabase.DefaultInstance;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        //puts the json data in the "users/userId" part of the database.
        db.RootReference.Child("users").Child(userId).SetRawJsonValueAsync(data);
    }

    void LoadFromFirebase()
    {
        var db = FirebaseDatabase.DefaultInstance;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        db.RootReference.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
            }

            //here we get the result from our database.
            DataSnapshot snap = task.Result;

            outputText.text = snap.GetRawJsonValue();
        });
    }
}
