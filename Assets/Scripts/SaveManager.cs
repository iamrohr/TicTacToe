using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;
    public static SaveManager Instance { get { return _instance; } }

    public delegate void OnLoadedDelegate(string jsonData);
    public delegate void OnSaveDelegate();

    FirebaseDatabase db;

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

        db = FirebaseDatabase.DefaultInstance;
    }

   
    //loads the data at "path" then returns json result to the delegate/callback function
    public void LoadData(string path, OnLoadedDelegate onLoadedDelegate)
    {
        //Fråga varför vi ej når hit. 
        Debug.Log("In LoadData");
        db.RootReference.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogWarning(task.Exception);

            onLoadedDelegate(task.Result.GetRawJsonValue());
        });
    }

    //Save the data at the given path
    public void SaveData(string path, string jsonData, OnSaveDelegate onSaveDelegate = null)
    {
        db.RootReference.Child(path).SetRawJsonValueAsync(jsonData).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogWarning(task.Exception);

            onSaveDelegate?.Invoke();
        });
    }

    public void LoadDataMultiple(string path, OnLoadedDelegate onLoadedDelegate)
    {
        db.RootReference.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            string jsonData = task.Result.GetRawJsonValue();

            if (task.Exception != null)
                Debug.LogWarning(task.Exception);

            foreach (var item in task.Result.Children)
            {
                onLoadedDelegate(task.Result.GetRawJsonValue());
            }
        });
    }
}