using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
using UnityEngine.SceneManagement;
//using GameObjects;
using UnityEngine.UI;

public class gameSetUpManager : MonoBehaviour {

    public SocketIOComponent socket;

    public Text numberOfPlayer;

    public Text level;

    void Start ()
    {

        StartCoroutine(ConnectToServer());
        socket.On("USER_CONNECTED", OnUserConnected );
        socket.On("gameSetUp_SUCCESS", gameSetUpSucessful );

    }

    void gameSetUpSucessful(SocketIOEvent obj)//change scene
    {
        var level = obj.data.ToDictionary()["level"];
        Debug.Log("gameSetUp successful");
        if(level=="Family"){
            SceneManager.LoadScene("Room");
            // Debug.Log("Family board");
        }else{
            SceneManager.LoadScene("DragDrop");
            // Debug.Log("Not family");
        }
        
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

    public void Next()
    {
        StaticInfo.level = level.text;
        StaticInfo.numberOfPlayer = numberOfPlayer.text;
        
        Debug.Log(StaticInfo.level);
        Debug.Log(StaticInfo.numberOfPlayer);
        
        Dictionary<String, String> gameSetUp = new Dictionary<string, string>();
        gameSetUp["room"] = StaticInfo.roomNumber;
        gameSetUp["name"] = StaticInfo.name;
        gameSetUp["level"] = StaticInfo.level;
        gameSetUp["numberOfPlayer"] = StaticInfo.numberOfPlayer;
        socket.Emit("gameSetUp",new JSONObject(gameSetUp));
    }


}
