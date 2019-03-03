using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
//using GameObjects;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour {

	public SocketIOComponent socket;

	public Text username;

	void Start ()
	{

		StartCoroutine(ConnectToServer());
		socket.On("USER_CONNECTED", OnUserConnected );
		
		//----------------------just for testing, probably should create game in other scenes
		Game new_game = new Game(3,DifficultyLevel.Easy);
		Dictionary<String, Fireman> fireManManager = new_game.getFiremanManager();

		foreach(KeyValuePair<string, Fireman> entry in fireManManager)
		{
			Debug.Log(entry.Value.name);
		}
		//-------------------------------------------------------

	}

	IEnumerator ConnectToServer()
	{
		yield return new WaitForSeconds(0.5f);

		socket.Emit("USER_CONNECT");
		
		yield return new WaitForSeconds(0.5f);
		
		Dictionary<string, string> data = new Dictionary<string,string>();
		//data['name'] = 'eason';
		
		//socket.Emit("PLAY");
	}

	void OnUserConnected (SocketIOEvent obj)
	{
		Debug.Log( "all user born on this client" );
	}

	public void LoginClick()
	{
		Debug.Log("login button clicked");
		Debug.Log(username.text);
	}

	public void SignUpClick()
	{
		Debug.Log("Sign up button clicked");
		Debug.Log(username.text);
	}

}
