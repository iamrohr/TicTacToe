using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    //Singleton structure variables
    private static SceneController _instance;
    public static SceneController Instance { get { return _instance; } }

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

    // Update is called once per frame
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    internal void StartGame(GameInfo gameInfo)
    {
        GameData.Instance.gameData = gameInfo;
        SceneManager.LoadScene("Game");
    }
}