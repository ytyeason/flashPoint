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
        socket.On("LocationSetUp_SUCCESS", LocationSetUp_SUCCESS);

    }

    void startGame_SUCCESS(SocketIOEvent obj)//change scene
    {
        Debug.Log("start Game successful");
        Debug.Log(obj.data);
        StaticInfo.game_info = obj.data[0];
        var room=obj.data.ToDictionary()["room"];
        if(!room.Equals(StaticInfo.roomNumber)) return;
        SceneManager.LoadScene("FlashpointUIDemo");
    }

    void LocationSetUp_SUCCESS(SocketIOEvent obj)
    {
        Debug.Log("set Location successful");
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

    public void RightClicked()
    {
        Debug.Log("Right Clicked");
        StaticInfo.Location = new int[] { 54, 18 };
        Dictionary<String, String> Right = new Dictionary<string, string>();
        Right["room"] = StaticInfo.roomNumber;
        Right["name"] = StaticInfo.name;
        Right["Location"] = StaticInfo.Location[0] + "," + StaticInfo.Location[1];

        socket.Emit("Location", new JSONObject(Right));
    }

    public void LeftClicked()
    {
        Debug.Log("Left Clicked");
        StaticInfo.Location = new int[] { 0, 18 }; 
        Dictionary<String, String> Left = new Dictionary<string, string>();
        Left["room"] = StaticInfo.roomNumber;
        Left["name"] = StaticInfo.name;
        Left["Location"] = StaticInfo.Location[0] + "," + StaticInfo.Location[1];
        //Debug.Log(Left["Location"]);

        socket.Emit("Location", new JSONObject(Left));
    }

    public void TopClicked()
    {
        Debug.Log("Top Clicked");
        StaticInfo.Location = new int[] { 30, 42 };
        Dictionary<String, String> Top = new Dictionary<string, string>();
        Top["room"] = StaticInfo.roomNumber;
        Top["name"] = StaticInfo.name;
        Top["Location"] = StaticInfo.Location[0] + "," + StaticInfo.Location[1];

        socket.Emit("Location", new JSONObject(Top));
    }

    public void BottomClicked()
    {
        Debug.Log("Bottom Clicked");
        StaticInfo.Location = new int[] { 30, 0 };
        Dictionary<String, String> Bottom = new Dictionary<string, string>();
        Bottom["room"] = StaticInfo.roomNumber;
        Bottom["name"] = StaticInfo.name;
        Bottom["Location"] = StaticInfo.Location[0] + ","+StaticInfo.Location[1];

        socket.Emit("Location", new JSONObject(Bottom));
    }

}
