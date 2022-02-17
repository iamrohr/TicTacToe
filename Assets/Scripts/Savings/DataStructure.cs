using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserInfo
{
    public string name;
    public float colorHue;
}

[Serializable]
public class GameInfo
{
    public string displayName;
    public string gameID;
    public int numberOfPlayers = 2;
    public int seed;
    // public List<PlayerInfo> players;
}

[Serializable]
public class PlayerGameInfo
{
    public string displayName;
    // public Vector3 position;
    public bool hidden;
}