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
    public int whosTurnFirebase;
    public int[] markedSpacesFirebase;
    public List<GamePlayer> playersFirebase;
}

[Serializable]
public class GamePlayer
{
    // public int whosTurn; //0 starts, 1 second
    public string userID;
    public string name;
    public float colorHue;
    public int playerNumber;
}