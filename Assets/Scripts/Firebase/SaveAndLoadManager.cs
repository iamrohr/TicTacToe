using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoadManager : MonoBehaviour
{
    private static SaveAndLoadManager _instance;
    public static SaveAndLoadManager Instance { get { return _instance; } }

	public delegate void OnLoadedDelegateMultiple(List<string> jsonData);
	public delegate void OnLoadedDelegate(string json);
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
        db.RootReference.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogWarning(task.Exception);

            onLoadedDelegate(task.Result.GetRawJsonValue());
        });
    }
    
    public void LoadData(string path, OnLoadedDelegateMultiple onLoadedDelegates)
    {
        db.RootReference.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            List<string> loadedJson = new List<string>();

            foreach (var item in task.Result.Children)
            {
                loadedJson.Add(item.GetRawJsonValue());
            }

            onLoadedDelegates(loadedJson);
        });
    }

    //Save the data at the given path
    public void SaveData(string path, string data, OnSaveDelegate onSaveDelegate = null)
	{
        db.RootReference.Child(path).SetRawJsonValueAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogWarning(task.Exception);

			onSaveDelegate?.Invoke();
		});
	}

	public string GetKey(string path)
	{
        return db.RootReference.Child(path).Push().Key;
    }
}