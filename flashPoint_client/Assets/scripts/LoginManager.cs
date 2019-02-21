using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour {

	public SocketIOComponent socket;

	public Text username;

	void Start ()
	{
		Debug.Log(socket);
		StartCoroutine(ConnectToServer());
		socket.On("USER_CONNECTED", OnUserConnected );

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
