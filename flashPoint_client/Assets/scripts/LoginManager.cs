using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;

public class LoginManager : MonoBehaviour {

	public SocketIOComponent socket;


	void Start ()
	{
		Debug.Log(socket);
		StartCoroutine(ConnectToServer());

	}

	IEnumerator ConnectToServer()
	{
		yield return new WaitForSeconds(0.5f);

		socket.Emit("USER_CONNECT");
	}

	void Update()
	{
		
	}

}
