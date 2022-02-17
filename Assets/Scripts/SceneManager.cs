using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public Button loadSceneButton;
    public int sceneNumber;
    // Start is called before the first frame update
    void Start()
    {
        loadSceneButton.onClick.AddListener(() => LoadNextScene(sceneNumber));
    }

    void LoadNextScene(int sceneNumber)
    {
        Debug.Log("Clicked");
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNumber);
    }
}
