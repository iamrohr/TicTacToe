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

	public int maxNumberOfButtons = 5;

	private void Start()
	{
		UpdateGameList();
	}

	private void UpdateGameList()
	{
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

	private void NewGame(string data)
	{
		if (!(data == "" || data == null))
		{
			GameInfo loadedGame = JsonUtility.FromJson<GameInfo>(data);
			if (!(loadedGame.gameID == "" || data == null))
			{
				if (!GameData.Instance.userData.activeGames.Contains(loadedGame.gameID) && loadedGame.openPlayerSlots > 0)
				{
					Debug.Log("Joining existing game");
					JoinGame(loadedGame);
					return;
				}
			}
		}

		CreateGame();
	}
	
	public void JoinGame(GameInfo gameInfo)
	{
		Debug.Log("joining game: " + gameInfo.gameID);
		GameData.Instance.userData.activeGames.Add(gameInfo.gameID);

		//save our user with our new game
		GameData.Instance.userData.activeGames.Add(gameInfo.gameID);
		GameData.Instance.SaveUserData();

		//Update new game name
		gameInfo.displayName = gameInfo.players[0].name + " vs " + GameData.Instance.userData.name;

		string jsonString = JsonUtility.ToJson(gameInfo);

		//Update the game
		SaveManager.Instance.SaveData("games/" + gameInfo.gameID, jsonString);
	}
	
	
	public void CreateGame()
	{
		//Create a new game and start filling out the info.
		var newGameInfo = new GameInfo();

		newGameInfo.seed = Random.Range(0, int.MaxValue);
		newGameInfo.displayName = GameData.Instance.userData.name + "'s game";

		//Add the user as the first player
		newGameInfo.players = new List<UserInfo>();
		newGameInfo.players.Add(PlayerData.data);

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
			if (GameData.Instance.userData.activeGames.Contains(activeGame.gameID) || activeGame.players.Count > 1)
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