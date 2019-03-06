using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
using UnityEngine.SceneManagement;
//using GameObjects;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour {

	public SocketIOComponent socket;

	public Text username;

	void Start ()
	{

		StartCoroutine(ConnectToServer());
		socket.On("USER_CONNECTED", OnUserConnected );
		socket.On("LoginSucessful", LoginSucessful );
		
		//----------------------just for testing, probably should create game in other scenes
		Game new_game = new Game(3,DifficultyLevel.Easy);
		Dictionary<String, Fireman> fireManManager = new_game.getFiremanManager();

		foreach(KeyValuePair<string, Fireman> entry in fireManManager)
		{
			Debug.Log(entry.Value.name);
		}
		//-------------------------------------------------------

	}

	void LoginSucessful(SocketIOEvent obj)//change scene
	{
		Debug.Log("login successful");
		SceneManager.LoadScene("Lobby");
	}

	IEnumerator ConnectToServer()
	{
		yield return new WaitForSeconds(0.5f);

		socket.Emit("USER_CONNECT");
		
		yield return new WaitForSeconds(0.5f);
		
	}

	void OnUserConnected (SocketIOEvent obj)
	{
		Debug.Log( "all user born on this client" );
	}

	public void LoginClick()
	{
		Debug.Log("login button clicked");
		Debug.Log(username.text);

		StaticInfo.name = username.text;
		
		Dictionary<String, String> user = new Dictionary<string, string>();
		user["name"] = StaticInfo.name;
		socket.Emit("Login",new JSONObject(user));
	}

	public void SignUpClick()
	{
		Debug.Log("Sign up button clicked");
		Debug.Log(username.text);
		
		StaticInfo.name = username.text;
		
		Dictionary<String, String> user = new Dictionary<string, string>();
		user["name"] = StaticInfo.name;
		user["password"] = "666";
		socket.Emit("Signup", new JSONObject(user));
	}

}
