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

    }

    void CreateRoomSucessful(SocketIOEvent obj)//change scene
    {
        Debug.Log("create room successful");
        //SceneManager.LoadScene("DragDrop"); skpped for now
        SceneManager.LoadScene("gameSetUp");
    }

    void LOADRoomSucessful(SocketIOEvent obj)
    {
        Debug.Log("load room successful");
        //SceneManager.LoadScene("DragDrop");
        string level=obj.data.ToDictionary()["level"];
        string det=obj.data.ToDictionary()["status"];
        if(level.Equals("Family")){
            SceneManager.LoadScene("Room");
            
        }else{
            SceneManager.LoadScene("DragDrop");
        }
        StaticInfo.level=level;
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
    
    public void LOAD()
    {
        Debug.Log("LOAD button clicked");
        Debug.Log(roomNumber.text);

        StaticInfo.roomNumber = roomNumber.text;
		
        Dictionary<String, String> room = new Dictionary<string, string>();
        room["room"] = StaticInfo.roomNumber;
        room["name"] = StaticInfo.name;
        socket.Emit("LOAD_ROOM",new JSONObject(room));
    }
    
    public void CreateRoom()
    {
        Debug.Log("create button clicked");
        Debug.Log(roomNumber.text);
        
        StaticInfo.roomNumber = roomNumber.text;
		
        Dictionary<String, String> room = new Dictionary<string, string>();
        room["room"] = StaticInfo.roomNumber;
        room["name"] = StaticInfo.name;
        socket.Emit("CREATE_ROOM",new JSONObject(room));
    }


}
