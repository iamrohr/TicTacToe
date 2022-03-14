using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Firebase.Database;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //Turn
    public int whoseTurn;
    public int turnCount; 
    public GameObject[] turnIcons; 
    public int[] markedSpaces; 
    
    //Graphics
    public Sprite[] playIcons; 
    public Button[] tictactoeSpaces;

    //Winner states
    public Text winnerTextX;
    public Text winnerTextO;
    public GameObject winnerPanel;
    public GameObject[] winningLines;
    public GameObject drawState;
    
    //ScoreCounter
    public int xPlayerScore;
    public int oPlayerScore;
    public Text xPlayerScoreText;
    public Text oPlayerScoreText;
    public bool setupGame;
    public GameController gameController;
    
    //Wait panel
    public GameObject waitPanel;
    public UnityEngine.UI.Image player1;
    public UnityEngine.UI.Image player2;
    
    void Start()
    {
        if (!GameData.Instance.gameData.setupGameFB)
        {
            GameSetup();
        }
        
        whoseTurn = GameData.Instance.gameData.whosTurnFB;
        turnCount = GameData.Instance.gameData.turnCountFB;
        markedSpaces = GameData.Instance.gameData.markedSpacesFB;
        CheckButtons();
        UpdateLocalPlayerData();
        WhosTurnFunction();
        
        FirebaseDatabase.DefaultInstance.RootReference.Child("games/").Child(GameData.Instance.gameData.gameID).ValueChanged += CheckIfChangesInGameHappens;
    }
    
    void CheckIfChangesInGameHappens(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        
        GameInfo gameInfo = JsonUtility.FromJson<GameInfo>(args.Snapshot.GetRawJsonValue());

        try
        {
            GameData.Instance.gameData = gameInfo;

            whoseTurn = gameInfo.whosTurnFB;
            turnCount = gameInfo.turnCountFB;
            markedSpaces = gameInfo.markedSpacesFB;
            
            CheckButtons();
            WhosTurnFunction();
            PlayerColors();
            
            if (GameData.Instance.gameData.winnerCheckFB)
            {
                WinnerDisplay(GameData.Instance.gameData.winnerNumberFB);
            }
        }
        
        catch
        {
            
        }
    }
    private void WhosTurnFunction()
    {
        if (whoseTurn == 0 && GameData.Instance.gamePlayer.playerNumber == 0)
        {
            waitPanel.SetActive(false);
            turnIcons[0].SetActive(true);
            turnIcons[1].SetActive(false);
        }

        if (whoseTurn == 1 && GameData.Instance.gamePlayer.playerNumber == 1)
        {
            waitPanel.SetActive(false);
            turnIcons[0].SetActive(false);
            turnIcons[1].SetActive(true);
        }
    }
    private static void UpdateLocalPlayerData()
    {
        if (GameData.Instance.gameData.players[0].userID == GameData.Instance.userID)
        {
            GameData.Instance.gamePlayer = GameData.Instance.gameData.players[0];
            GameData.Instance.gameData.playerColor1FB = GameData.Instance.gamePlayer.colorHue;
            string jSon = JsonUtility.ToJson(GameData.Instance.gameData);
            SaveAndLoadManager.Instance.SaveData("games/" + GameData.Instance.gameData.gameID, jSon);
        }
        
        else if (GameData.Instance.gameData.players[1].userID == GameData.Instance.userID)
        {
            GameData.Instance.gamePlayer = GameData.Instance.gameData.players[1];
            GameData.Instance.gameData.playerColor2FB = GameData.Instance.gamePlayer.colorHue;
            string jSon = JsonUtility.ToJson(GameData.Instance.gameData);
            SaveAndLoadManager.Instance.SaveData("games/" + GameData.Instance.gameData.gameID, jSon);
        }
    }
    void GameSetup()
    {
        whoseTurn = 0;
        turnCount = 0;
        turnIcons[0].SetActive(true);
        turnIcons[1].SetActive(false);

        for (int i = 0; i < tictactoeSpaces.Length; i++)
        {
            tictactoeSpaces[i].interactable = true;
            tictactoeSpaces[i].GetComponent<Image>().sprite = null;
        }
        for (int i = 0; i < markedSpaces.Length; i++)
        {
            markedSpaces[i] = -100;
        }

        setupGame = true;
        GameData.Instance.gameData.setupGameFB = setupGame;
        GameData.Instance.gameData.markedSpacesFB = markedSpaces;
        GameData.Instance.gameData.turnCountFB = turnCount;
        GameData.Instance.gameData.whosTurnFB = whoseTurn;
        string jSon = JsonUtility.ToJson(GameData.Instance.gameData);
        SaveAndLoadManager.Instance.SaveData("games/" + GameData.Instance.gameData.gameID, jSon);
    }

    public void TicTacToeButton(int whichNumber)
    {
        tictactoeSpaces[whichNumber].image.sprite = playIcons[whoseTurn];
        tictactoeSpaces[whichNumber].interactable = false;

        markedSpaces[whichNumber] = whoseTurn + 1; //sets 1 or 2 instead of 0 and 1 
        turnCount++;

        if (turnCount > 4)
        {
            bool isWinner = WinnerCheck();
            
            if (turnCount == 9 && isWinner == false)
            {
                Draw();
            }
        }

        if (whoseTurn == 0)
        {
            whoseTurn = 1;
            turnIcons[0].SetActive(false);
            turnIcons[1].SetActive(true);
        }
        else
        {
            whoseTurn = 0;
            turnIcons[0].SetActive(true);
            turnIcons[1].SetActive(false);
        }
        
        //Save FB to local
        GameData.Instance.gameData.markedSpacesFB = markedSpaces;
        GameData.Instance.gameData.turnCountFB = turnCount;
        GameData.Instance.gameData.whosTurnFB = whoseTurn;
        
        //Ta user data från detta gamet. Gör till en json string och skicka in i save manager och spara på FB. 
        string jSon = JsonUtility.ToJson(GameData.Instance.gameData);
        SaveAndLoadManager.Instance.SaveData("games/" + GameData.Instance.gameData.gameID, jSon);
        
        waitPanel.SetActive(true);
    }

    bool WinnerCheck()
    {
        //Hor win state
        int s1 = markedSpaces[0] + markedSpaces[1] + markedSpaces[2];
        int s2 = markedSpaces[3] + markedSpaces[4] + markedSpaces[5];
        int s3 = markedSpaces[6] + markedSpaces[7] + markedSpaces[8];
        
        //Vert win state
        int s4 = markedSpaces[0] + markedSpaces[3] + markedSpaces[6];
        int s5 = markedSpaces[1] + markedSpaces[4] + markedSpaces[7];
        int s6 = markedSpaces[2] + markedSpaces[5] + markedSpaces[8];
        
        //Diagonal win state
        int s7 = markedSpaces[0] + markedSpaces[4] + markedSpaces[8];
        int s8 = markedSpaces[2] + markedSpaces[4] + markedSpaces[6];

        var solutions = new int[]
            {s1, s2, s3, s4, s5, s6, s7, s8};
        for (int i = 0; i < solutions.Length; i++)
        {
            if (solutions[i] == 3 * (whoseTurn + 1))
            {
                WinnerDisplay(i);

                if (whoseTurn == 0 && GameData.Instance.gamePlayer.playerNumber == 0)
                {
                    GameData.Instance.gameData.winnerPlayerNumberFB = 1;
                }

                if (whoseTurn == 1 && GameData.Instance.gamePlayer.playerNumber == 1)
                {
                    GameData.Instance.gameData.winnerPlayerNumberFB = 2;
                }
                
                GameData.Instance.gameData.winnerNumberFB = i;
                GameData.Instance.gameData.winnerCheckFB = true;
                string jSon = JsonUtility.ToJson(GameData.Instance.gameData);
                SaveAndLoadManager.Instance.SaveData("games/" + GameData.Instance.gameData.gameID, jSon);
                return true;
            }
        }
        return false;
    }
    
    void WinnerDisplay(int indexIn)
    {
        if (GameData.Instance.gameData.winnerPlayerNumberFB == 1)
        {
            CheckButtons();
            winnerPanel.gameObject.SetActive(true);
            winnerTextX.gameObject.SetActive(true);
            
        }
        if (GameData.Instance.gameData.winnerPlayerNumberFB == 2)
        {
            CheckButtons();
            winnerPanel.gameObject.SetActive(true);
            winnerTextO.gameObject.SetActive(true);
        }
        
        if (GameData.Instance.gameData.winnerPlayerNumberFB == 1)
        {
            xPlayerScore++;
            xPlayerScoreText.text = xPlayerScore.ToString();
        }

        if (GameData.Instance.gameData.winnerPlayerNumberFB == 2)
        {
            oPlayerScore++;
            oPlayerScoreText.text = oPlayerScore.ToString();
        }
        
        winningLines[indexIn].SetActive(true);
    }
    
    void PlayerColors()
    {
        if(GameData.Instance.gameData.playerColor1FB != 0)
            player2.color = Color.HSVToRGB(GameData.Instance.gameData.playerColor1FB, 1, 1);
        
        if(GameData.Instance.gameData.playerColor2FB != 0)
            player1.color = Color.HSVToRGB(GameData.Instance.gameData.playerColor2FB, 1, 1);
    }
    
    void Draw()
    {
        winnerPanel.SetActive(true);
        drawState.SetActive(true);
    }
    
    void CheckButtons()
    {
        //TL
        if (markedSpaces[0] == 1)
        {
            tictactoeSpaces[0].image.sprite = playIcons[0];
            tictactoeSpaces[0].interactable = false;
        }
        if (markedSpaces[0] == 2)
        {
            tictactoeSpaces[0].image.sprite = playIcons[1];
            tictactoeSpaces[0].interactable = false;
        }
        
        //M
        if (markedSpaces[1] == 1)
        {
            tictactoeSpaces[1].image.sprite = playIcons[0];
            tictactoeSpaces[1].interactable = false;
        }
        if (markedSpaces[1] == 2)
        {
            tictactoeSpaces[1].image.sprite = playIcons[1];
            tictactoeSpaces[1].interactable = false;
        }
        
        //TR
        if (markedSpaces[2] == 1)
        {
            tictactoeSpaces[2].image.sprite = playIcons[0];
            tictactoeSpaces[2].interactable = false;
        }
        if (markedSpaces[2] == 2)
        {
            tictactoeSpaces[2].image.sprite = playIcons[1];
            tictactoeSpaces[2].interactable = false;
        }
        
        //TR
        if (markedSpaces[3] == 1)
        {
            tictactoeSpaces[3].image.sprite = playIcons[0];
            tictactoeSpaces[3].interactable = false;
        }
        if (markedSpaces[3] == 2)
        {
            tictactoeSpaces[3].image.sprite = playIcons[1];
            tictactoeSpaces[3].interactable = false;
        }
        
        //ML
        if (markedSpaces[4] == 1)
        {
            tictactoeSpaces[4].image.sprite = playIcons[0];
            tictactoeSpaces[4].interactable = false;
        }
        if (markedSpaces[4] == 2)
        {
            tictactoeSpaces[4].image.sprite = playIcons[1];
            tictactoeSpaces[4].interactable = false;
        }
        
        //MM
        if (markedSpaces[5] == 1)
        {
            tictactoeSpaces[5].image.sprite = playIcons[0];
            tictactoeSpaces[5].interactable = false;
        }
        if (markedSpaces[5] == 2)
        {
            tictactoeSpaces[5].image.sprite = playIcons[1];
            tictactoeSpaces[5].interactable = false;
        }
        
        //MR
        if (markedSpaces[6] == 1)
        {
            tictactoeSpaces[6].image.sprite = playIcons[0];
            tictactoeSpaces[6].interactable = false;
        }
        if (markedSpaces[6] == 2)
        {
            tictactoeSpaces[6].image.sprite = playIcons[1];
            tictactoeSpaces[6].interactable = false;
        }
        
        //BL
        if (markedSpaces[7] == 1)
        {
            tictactoeSpaces[7].image.sprite = playIcons[0];
            tictactoeSpaces[7].interactable = false;
        }
        if (markedSpaces[7] == 2)
        {
            tictactoeSpaces[7].image.sprite = playIcons[1];
            tictactoeSpaces[7].interactable = false;
        }
        
        //BM
        if (markedSpaces[8] == 1)
        {
            tictactoeSpaces[8].image.sprite = playIcons[0];
            tictactoeSpaces[8].interactable = false;
        }
        if (markedSpaces[8] == 2)
        {
            tictactoeSpaces[8].image.sprite = playIcons[1];
            tictactoeSpaces[8].interactable = false;
        }
    }
}

