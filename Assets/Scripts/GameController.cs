using System.Collections;
using System.Collections.Generic;
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

    public int playerNumber;
    void Start()
    {
        GameSetup();
        playerNumber = GameData.Instance.gamePlayerIngame.playerNumber;
        whoseTurn = GameData.Instance.gameData.whosTurnFirebase;
    }

    void SaveFirebaseGameData()
    {
        SaveManager.Instance.SaveData("games/" + GameData.Instance.gameData.gameID, JsonUtility.ToJson(GameData.Instance.gameData));
    }

    void SaveLocalPlayerDataToFirebase()
    {
        if (playerNumber == 0)
        {
            
        }

        if (playerNumber == 1)
        {
            
        }
    }

    void LoadFirebaseGameData()
    {
        
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
    }

    public void TicTacToeButton(int whichNumber)
    {
        whoseTurn = GameData.Instance.gameData.whosTurnFirebase;
        
        if (whoseTurn == playerNumber)
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
            

            if (whoseTurn == 0 && whoseTurn == playerNumber)
            {
                whoseTurn = 1;
                
                turnIcons[0].SetActive(false);
                turnIcons[1].SetActive(true);
            }
            else if (whoseTurn == 1 && whoseTurn == playerNumber)
            {
                whoseTurn = 0;
                
                turnIcons[0].SetActive(true);
                turnIcons[1].SetActive(false);
            }
        }

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
    
}

