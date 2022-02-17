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
    public List<GamePlayer> players;
}

[Serializable]
public class GamePlayer
{
    public string userID;
    public string name;
    public float colorHue;
}