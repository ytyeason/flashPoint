using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
using UnityEngine.SceneManagement;
//using GameObjects;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour {

    public SocketIOComponent socket;

    public Text roomNumber;

    public String user;

    void Start ()
    {

        StartCoroutine(ConnectToServer());
        socket.On("USER_CONNECTED", OnUserConnected );
        socket.On("CREATE_ROOM_SUCCESS", CreateRoomSucessful );
        socket.On("LOAD_ROOM_SUCCESS", LOADRoomSucessful );
		
        //----------------------just for testing, probably should create game in other scenes
        Game new_game = new Game(3,DifficultyLevel.Easy);
        Dictionary<String, Fireman> fireManManager = new_game.getFiremanManager();

        foreach(KeyValuePair<string, Fireman> entry in fireManManager)
        {
            Debug.Log(entry.Value.name);
        }
        //-------------------------------------------------------

    }

    void CreateRoomSucessful(SocketIOEvent obj)//change scene
    {
        Debug.Log("create room successful");
        SceneManager.LoadScene("DragDrop");
    }

    void LOADRoomSucessful(SocketIOEvent obj)
    {
        Debug.Log("load room successful");
        SceneManager.LoadScene("DragDrop");
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
    
    public void LOAD()
    {
        Debug.Log("LOAD button clicked");
        Debug.Log(roomNumber.text);
		
        Dictionary<String, String> room = new Dictionary<string, string>();
        room["room"] = roomNumber.text;
        socket.Emit("LOAD_ROOM",new JSONObject(room));
    }
    
    public void CreateRoom()
    {
        Debug.Log("create button clicked");
        Debug.Log(roomNumber.text);
		
        Dictionary<String, String> room = new Dictionary<string, string>();
        room["room"] = roomNumber.text;
        socket.Emit("CREATE_ROOM",new JSONObject(room));
    }


}
