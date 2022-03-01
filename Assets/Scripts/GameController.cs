using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //Turn
    public int whoseTurn; //0 = X  and 1 = O
    public int turnCount; //Number of turns played
    public GameObject[] turnIcons; // Displays whos turn it is
    public int[] markedSpaces; //ID's which space that was marked by which player
    
    //Graphics
    public Sprite[] playIcons; //0 = x and 1 = O
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

    public GameController gameController;
    
    //Wait panel
    public GameObject waitPanel;

    public bool setupGame;

    void Start()
    {
         
        if (setupGame)
        {
            GameSetup();
        }

        //Listener varje gång kommer den uppdatera. Delegate.
       FirebaseDatabase.DefaultInstance.RootReference.Child("games/").Child(GameData.Instance.gameData.gameID).ValueChanged += CheckIfChangesInGameHappens;
    }

    //kollar om ändringar hänt
    void CheckIfChangesInGameHappens(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        //Konverterar till game info.
        GameInfo gameInfo = JsonUtility.FromJson<GameInfo>(args.Snapshot.GetRawJsonValue());

        try
        {
            GameData.Instance.gameData = gameInfo;
            
            markedSpaces = gameInfo.markedSpacesFB;
            turnCount = gameInfo.turnCountFB;
            whoseTurn = gameInfo.whosTurnFB;
            
        }
        catch
        {
            
        }
    }

    private void Update()
    {
        //Metod ladda uppdaterad data och spara lokalt. 
        // Uppdatera gameplayer från FB. Data från spelet och sätt det med player data så att du vet vilken spelare du är. Lokalt. 
        if (GameData.Instance.gameData.players[0].userID == GameData.Instance.userID)
            GameData.Instance.gamePlayer = GameData.Instance.gameData.players[0];
        else if (GameData.Instance.gameData.players[1].userID == GameData.Instance.userID)
            GameData.Instance.gamePlayer = GameData.Instance.gameData.players[1];
        
        //Egen metod som kollar vems tur. 
        //Bryta ut till egen funktion?
        if (whoseTurn == 0 && GameData.Instance.gamePlayer.playerNumber == 0)
        {
            waitPanel.SetActive(false);
        }
        if (whoseTurn == 1 && GameData.Instance.gamePlayer.playerNumber == 1)
        {
            waitPanel.SetActive(false);
        }
        
        CheckButtons();

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
        
        //Markera alla till, bara för starten -1 
        for (int i = 0; i < markedSpaces.Length; i++)
        {
            markedSpaces[i] = -100;
        }

        setupGame = false;
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
        
        //Kanske byta ut till FB direkt och inte ha 2 olika. Arbeta med Singelton istället för ny. 
        GameData.Instance.gameData.markedSpacesFB = markedSpaces;
        GameData.Instance.gameData.turnCountFB = turnCount;
        GameData.Instance.gameData.whosTurnFB = whoseTurn;
        //Ta user detta från detta gamet. Gör till en json string och skicka in i save manager och spara på FB. 
        string jSon = JsonUtility.ToJson(GameData.Instance.gameData);
        SaveAndLoadManager.Instance.SaveData("games/" + GameData.Instance.gameData.gameID, jSon);
        
        //Inactivate game
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
                return true;
            }
        }
        return false;
    }

    void WinnerDisplay(int indexIn)
    {
        if (whoseTurn == 0)
        {
            xPlayerScore++;
            xPlayerScoreText.text = xPlayerScore.ToString();
            
            winnerPanel.gameObject.SetActive(true);
            winnerTextX.gameObject.SetActive(true);
        }
        else
        {
            oPlayerScore++;
            oPlayerScoreText.text = oPlayerScore.ToString();
            
            winnerPanel.gameObject.SetActive(true);
            winnerTextO.gameObject.SetActive(true);
        }

        winningLines[indexIn].SetActive(true);
        
    }

    public void Rematch()
    {
        GameSetup();

        for (int i = 0; i < winningLines.Length; i++)
        {
            winningLines[i].SetActive(false);
        }
        
        winnerPanel.SetActive(false);
        drawState.SetActive(false);
    }

    public void Restart()
    {
        Rematch();
        xPlayerScore = 0;
        oPlayerScore = 0;
        xPlayerScoreText.text = "0";
        oPlayerScoreText.text = "0";
    }

    void Draw()
    {
        winnerPanel.SetActive(true);
        drawState.SetActive(true);
    }


    private static void LoadGameData()
    {
    
    }

    void SaveGameData()
    {
        // SaveManager.Instance.SaveData("users/");
    }

    public void DataToSend()
    {
        GameData.Instance.gameData.markedSpacesFB = markedSpaces;
        GameData.Instance.gameData.turnCountFB = turnCount;
        GameData.Instance.gameData.whosTurnFB = whoseTurn;
    }
    
    void CheckButtons()
    {
        //TL
        if (GameData.Instance.gameData.markedSpacesFB[1] == 1)
        {
            tictactoeSpaces[1].image.sprite = playIcons[1];
            tictactoeSpaces[1].interactable = false;
        }
        if (GameData.Instance.gameData.markedSpacesFB[1] == 2)
        {
            tictactoeSpaces[1].image.sprite = playIcons[2];
            tictactoeSpaces[1].interactable = false;
        }
        
    }

}

