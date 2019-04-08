using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
using System.Linq;
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
        socket.On("LOAD_GAME_SUCCESS", LOADGAMESuccessful);

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

    void LOADGAMESuccessful(SocketIOEvent obj)
    {
        Debug.Log("load room successful");
        //Debug.Log(obj.data);
        //Debug.Log(obj.data[0]);
        Debug.Log(obj.data[0]);

        StaticInfo.LoadGame = true;
        StaticInfo.game_info = obj.data[0];
        //var status_info = obj.data[1];
        //Debug.Log(obj.data[1][0]);
        //Debug.Log(obj.data[1][1]);
        var hWall = obj.data[1][0];
        var vWall = obj.data[1][1];
        var tiles = obj.data[1][2];
        var hDoor = obj.data[1][3];
        var vDoor = obj.data[1][4];
        //Debug.Log(tiles);

        Dictionary<int[], int> h = new Dictionary<int[], int>();

        foreach (var location in hWall.list)
        {
            //Debug.Log(location);
            foreach (KeyValuePair<string, string> entry in location.ToDictionary())
            {
                var key = entry.Key.Split(',').Select(Int32.Parse).ToArray();
                var value = Convert.ToInt32(entry.Value);
                h[key] = value;
            }
        }
        StaticInfo.hWallMemo = h;

        Dictionary<int[], int> v = new Dictionary<int[], int>();

        foreach (var location in vWall.list)
        {
            //Debug.Log(location);
            foreach (KeyValuePair<string, string> entry in location.ToDictionary())
            {
                var key = entry.Key.Split(',').Select(Int32.Parse).ToArray();
                var value = Convert.ToInt32(entry.Value);
                v[key] = value;
            }
        }
        StaticInfo.vWallMemo = v;

        int[,] t = new int[10,8];
        foreach (var location in tiles.list)
        {
            foreach (KeyValuePair<string, string> entry in location.ToDictionary())
            {
                var key = entry.Key.Split(',').Select(Int32.Parse).ToArray();
                var value = Convert.ToInt32(entry.Value);
                //Debug.Log(key[0] + " "+ key[1] + " "+value);
                t[key[0], key[1]] = value;
            }
        }
        StaticInfo.tiles = t;

        Dictionary<int[], int> hD = new Dictionary<int[], int>();
        foreach (var location in hDoor.list)
        {
            foreach (KeyValuePair<string, string> entry in location.ToDictionary())
            {
                var key = entry.Key.Split(',').Select(Int32.Parse).ToArray();
                var value = Convert.ToInt32(entry.Value);
                //Debug.Log(key[0] + " "+ key[1] + " "+value);
                hD[key] = value;
            }
        }
        StaticInfo.defaultHorizontalDoors = hD;

        Dictionary<int[], int> vD = new Dictionary<int[], int>();
        foreach (var location in vDoor.list)
        {
            foreach (KeyValuePair<string, string> entry in location.ToDictionary())
            {
                var key = entry.Key.Split(',').Select(Int32.Parse).ToArray();
                var value = Convert.ToInt32(entry.Value);
                //Debug.Log(key[0] + " "+ key[1] + " "+value);
                vD[key] = value;
            }
        }
        StaticInfo.defaultVerticalDoors = vD;


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

    public void LOADRoom()
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

    public void LOADGame()
    {
        Debug.Log("LOAD game button clicked");
        Debug.Log(roomNumber.text);

        StaticInfo.roomNumber = roomNumber.text;

        Dictionary<String, String> room = new Dictionary<string, string>();
        room["room"] = StaticInfo.roomNumber;
        room["name"] = StaticInfo.name;
        socket.Emit("LOAD_GAME",new JSONObject(room));
    }


}
