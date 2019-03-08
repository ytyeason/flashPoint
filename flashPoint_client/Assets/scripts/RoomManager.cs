using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
using UnityEngine.SceneManagement;
//using GameObjects;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour {

    public SocketIOComponent socket;

    void Start ()
    {

        StartCoroutine(ConnectToServer());
        socket.On("USER_CONNECTED", OnUserConnected );
        socket.On("startGame_SUCCESS", startGame_SUCCESS );

    }

    void startGame_SUCCESS(SocketIOEvent obj)//change scene
    {
        Debug.Log("start Game successful");
        Debug.Log(obj.data);
        StaticInfo.game_info = obj.data;
        SceneManager.LoadScene("FlashpointUIDemo");
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

    public void StartGame()
    {
        Debug.Log("Start Game");
        Dictionary<String, String> startGame = new Dictionary<string, string>();
        startGame["room"] = StaticInfo.roomNumber;
        startGame["name"] = StaticInfo.name;
        
        socket.Emit("startGame",new JSONObject(startGame));
    }

}
