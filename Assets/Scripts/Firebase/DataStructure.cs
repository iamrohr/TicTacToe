using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserInfo
{
    public string name;
    public float colorHue;
    public List<string> activeGames;
}

[Serializable]
public class GameInfo
{
    public string displayName;
    public string gameID;
    public int seed;
    public int openPlayerSlots;
    public List<GamePlayer> players;

    //Game playing info
    public int whosTurnFB;
    public int turnCountFB;
    public int[] markedSpacesFB;
}

[Serializable]
public class GamePlayer
{
 //Loaded color etc 
    public string userID;
    public string name;
    public float colorHue;
    
//Game Specific Data for player      
    public int playerNumber;
    
}