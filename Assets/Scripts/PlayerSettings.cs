using UnityEngine;
using TMPro;

public class PlayerSettings : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public TMP_InputField playerNameInputField;
    
    public UnityEngine.UI.Slider playerColorSlider;
    public UnityEngine.UI.Image player;

    public UserInfo userInfo;
    public GameData gameData;
    
    void Start()
    {
        LoadPlayerSettings();
        playerColorSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        gameData = GameData.Instance;
    }

    public void ValueChangeCheck()
    {
        //Change Player color when slider is used 
        player.color = Color.HSVToRGB(playerColorSlider.value, 1, 1);
    }

    public void SavePlayerSettings()
    {
        //Save Player Name And Update On Screen
        if (string.IsNullOrWhiteSpace(playerNameInputField.text))
        {
            userInfo.name = "Player";
            // playerName.text = userInfo.name;
        }
        else
        {
            userInfo.name = playerNameInputField.text;
            playerName.text =  userInfo.name;
        }

        //Save Player Color
        userInfo.colorHue = playerColorSlider.value;

        GameData.Instance.userData = userInfo;
        
        GameData.Instance.SaveUserData();
    }

    private void LoadPlayerSettings()
    {
        playerColorSlider.value =  GameData.Instance.userData.colorHue;
        player.color = Color.HSVToRGB(GameData.Instance.userData.colorHue, 1, 1);
        playerName.text = playerNameInputField.text = GameData.Instance.userData.name;
    }

}
