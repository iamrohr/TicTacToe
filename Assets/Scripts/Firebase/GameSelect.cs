using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using UnityEngine.Events;

public class GameSelect : MonoBehaviour
{
	//Editor Connections
	public Transform gameListHolder;
	public GameObject gameButtonPrefab;

	private void Start()
	{
		UpdateGameList();
	}

	private void UpdateGameList()
	{
		Debug.Log("I am in update");
		//clear/remove the old list, If we have one.
		foreach (Transform child in gameListHolder)
			Destroy(child.gameObject);

		//create new list, load each of the users active games
		foreach (string gameID in GameData.Instance.userData.activeGames)
		{
			SaveManager.Instance.LoadData("games/" + gameID, LoadGameInfo);
		}

		//We have to few games, create a create game button
		if (GameData.Instance.userData.activeGames.Count < 5)
		{
			Debug.Log("I Have created a button");
		    CreateButton("New Game", () => SaveManager.Instance.LoadData("games/", NewGame));
		}
	}

	//Create button for the games, and add onclick events with the corresponding game info.
	public void LoadGameInfo(string json)
	{
		var gameInfo = JsonUtility.FromJson<GameInfo>(json);

		var newButton = Instantiate(gameButtonPrefab, gameListHolder).GetComponent<Button>();
		newButton.GetComponentInChildren<TextMeshProUGUI>().text = gameInfo.displayName;
		//TODO: display more game status on each button.

		newButton.onClick.AddListener(() => SceneController.Instance.StartGame(gameInfo));
	}

	private void NewGame(List<string> data)
	{

		List<GameInfo> games = new List<GameInfo>();

		foreach (var gameJson in data)
		{
			games.Add(JsonUtility.FromJson<GameInfo>(gameJson));
		}

		foreach (var game in games)
		{
			if (!GameData.Instance.userData.activeGames.Contains(game.gameID) && game.openPlayerSlots > 0)
			{
				Debug.Log("Joining existing game");
				JoinGame(game);
				return;
			}
		}
		
		CreateGame();
	}
	
	// public void JoinGame(GameInfo gameInfo)
	// {
	// 	Debug.Log("joining game: " + gameInfo.gameID);
	// 	GameData.Instance.userData.activeGames.Add(gameInfo.gameID);
	//
	// 	//save our user with our new game
	// 	GameData.Instance.SaveUserData();
	//
	// 	//Update new game name
	// 	gameInfo.displayName = gameInfo.players[0].name + " vs " + GameData.Instance.userData.name;
	// 	string jsonString = JsonUtility.ToJson(gameInfo);
	//
	// 	//Update the game
	// 	SaveManager.Instance.SaveData("games/" + gameInfo.gameID, jsonString);
	// }
	
	void JoinGame(GameInfo gameInfo)
	{
		//Add this game to our player and save.
		GameData.Instance.userData.activeGames.Add(gameInfo.gameID);
		GameData.Instance.SaveUserData();
			//Add our player to the game, update the game name and save.
		GamePlayer player = new GamePlayer
		{
			colorHue = GameData.Instance.userData.colorHue,
			name = GameData.Instance.userData.name,
			playerNumber = GameData.Instance.gamePlayerIngame.playerNumber,
			userID = GameData.Instance.userID
		};

		player.playerNumber = 1;

		gameInfo.playersFirebase.Add(player);
		//Att göra: Create a better naming convention.
		gameInfo.displayName = gameInfo.playersFirebase[0].name + " vs " + GameData.Instance.userData.name;
        
		gameInfo.openPlayerSlots--;
		//if (gameInfo.openPlayerSlots == 0)
		//    gameInfo.gameStatus = GameStatus.closed;

		//Save gameInfo so we can get it in the callback.
		// tmpGameInfo = gameInfo;

		//Update the game, we have joined
		string json = JsonUtility.ToJson(gameInfo);
		SaveManager.Instance.SaveData("games/" + gameInfo.gameID, json);
		
		//Att göra load game
		
	}
	
	
	public void CreateGame()
	{
		//Create a new game and start filling out the info.
		var newGameInfo = new GameInfo();
		
		newGameInfo.seed = Random.Range(0, int.MaxValue);
		newGameInfo.displayName = GameData.Instance.userData.name + "'s game";

		//Add the user as the first player
		newGameInfo.playersFirebase = new List<GamePlayer>();
		
		GamePlayer player = new GamePlayer
		{
			colorHue = GameData.Instance.userData.colorHue,
			name = GameData.Instance.userData.name,
			playerNumber = GameData.Instance.gamePlayerIngame.playerNumber,
			userID = GameData.Instance.userID
		};

		player.playerNumber = 0;
		
		newGameInfo.playersFirebase.Add(player);
		
		newGameInfo.openPlayerSlots = 2;

		//get a unique ID for the game
		string key = SaveManager.Instance.GetKey("games/");
		newGameInfo.gameID = key;

		//convert to json
		string data = JsonUtility.ToJson(newGameInfo);

		//Save our new game
		string path = "games/" + key;
		SaveManager.Instance.SaveData(path, data);

		//add the key to our active games
		GameCreated(key, newGameInfo);
	}

	public void GameCreated(string gameKey, GameInfo gameInfo)
	{
		//If we dont have any active games, create the list.
		GameData.Instance.userData.activeGames ??= new List<string>();
		GameData.Instance.userData.activeGames.Add(gameKey);

		//save our user with our new game
		GameData.Instance.SaveUserData();

		//Start the game
		SceneController.Instance.StartGame(gameInfo);
	}

	//We will try to join a random game, if we can't we create a new game.
	public void JoinRandomGame(List<string> data)
	{
		List<GameInfo> games = new List<GameInfo>();

		foreach (var item in data)
			games.Add(JsonUtility.FromJson<GameInfo>(item));

		foreach (var activeGame in games)
		{
			//Don't list our own games or full games.
			if (GameData.Instance.userData.activeGames.Contains(activeGame.gameID) || activeGame.playersFirebase.Count > 1)
				continue;
			
			JoinGame(activeGame);
			return;
		}

		//No random games to join, create a new game.
		CreateGame();
	}
	
	private void CreateButton(string buttonText, UnityAction onClickAction)
	{
		var newButton = Instantiate(gameButtonPrefab, gameListHolder.transform).GetComponent<Button>();
		newButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
		newButton.onClick.AddListener(onClickAction);
	}
}