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
        var room=obj.data.ToDictionary()["room"];
        if(!room.Equals(StaticInfo.roomNumber)) return;
        SceneManager.LoadScene("gameSetUp");
    }

    void LOADRoomSucessful(SocketIOEvent obj)
    {
        Debug.Log("load room successful");
        //SceneManager.LoadScene("DragDrop");
        string level=obj.data.ToDictionary()["level"];
        string det=obj.data.ToDictionary()["status"];
        string rand=obj.data.ToDictionary()["rand"];
        string numOfHazmat=obj.data.ToDictionary()["numberOfHazmat"];
        string numOfHotspot=obj.data.ToDictionary()["numberOfHotspot"];
        string numOfPlayer=obj.data.ToDictionary()["numberOfPlayer"];
        var room=obj.data.ToDictionary()["room"];
        if(!room.Equals(StaticInfo.roomNumber)) return;
        if(level.Equals("Family")){
            SceneManager.LoadScene("Room");

        }else{
            SceneManager.LoadScene("DragDrop");
        }
        StaticInfo.level=level;
        StaticInfo.random=Int32.Parse(rand);
        StaticInfo.numberOfPlayer=numOfPlayer;
        StaticInfo.numOfHazmat=numOfHazmat;
        StaticInfo.numOfHotspot=numOfHotspot;
    }

    void LOADGAMESuccessful(SocketIOEvent obj)
    {
        Debug.Log("load room successful");
        //Debug.Log(obj.data);
        //Debug.Log(obj.data[0]);
        Debug.Log("Games:   ");
        Debug.Log(obj.data);
        
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
        var poi = obj.data[1][5];
        var treatedPOI = obj.data[1][6];
        var movingPOI = obj.data[1][7];
        var movingTreatedMemo = obj.data[1][8];
        var HazmatMemo = obj.data[1][9];
        var hotSpotMemo = obj.data[1][10];
        var movingHazmatMemo = obj.data[1][11];
        var room=obj.data.ToDictionary()["roomNumber"];
        if(!room.Equals(StaticInfo.roomNumber)) return;
        var status=obj.data.ToDictionary()["status"];
        if(status.Equals("false")) return;
        
        //Debug.Log(poi);
        //Debug.Log(tiles);
        /*
        Debug.Log(obj.data[2]);
        Debug.Log(obj.data[3].str);
        Debug.Log(obj.data[4].str);
        Debug.Log(obj.data[5].str);
        */
        Debug.Log(obj.data[6].ToString());
        Debug.Log(obj.data[7].ToString());
        Debug.Log(obj.data[8].ToString());
        Debug.Log(obj.data[9].ToString());
        
        Debug.Log(obj.data[10].ToString());
        Debug.Log(obj.data[11].ToString());
        
        Debug.Log(obj.data[12]);
        Debug.Log(obj.data[13]);
        Debug.Log(obj.data[14]);
        Debug.Log(obj.data[15]);
        Debug.Log(obj.data[16]);
        Debug.Log(obj.data[17]);
        
        StaticInfo.name = obj.data[2].str;
        StaticInfo.roomNumber = obj.data[3].str;
        StaticInfo.level = obj.data[4].str;
        
        StaticInfo.numberOfPlayer = obj.data[5].str;
        StaticInfo.numOfHazmat = obj.data[6].str;
        StaticInfo.numOfHotspot = obj.data[7].str;
        
        StaticInfo.selectedRoles = obj.data[8].str;
        StaticInfo.confirmedPosition = obj.data[9].str;
        //StaticInfo.joinedPlayers = obj.data[10].str;
        
        StaticInfo.ambulance = obj.data[10].ToDictionary();
        StaticInfo.engine = obj.data[11].ToDictionary();
        
        StaticInfo.freeAP = Int32.Parse(obj.data[12].str);
        StaticInfo.remainingSpecAp = Int32.Parse(obj.data[13].str);
        
        StaticInfo.damagedWall = Int32.Parse(obj.data[14].str);
        StaticInfo.rescued = Int32.Parse(obj.data[15].str);
        StaticInfo.killed = Int32.Parse(obj.data[16].str);
        StaticInfo.removedHazmat = Int32.Parse(obj.data[17].str);
        
        StaticInfo.riding = Convert.ToBoolean(obj.data[18].str);
        StaticInfo.driving = Convert.ToBoolean(obj.data[19].str);
        StaticInfo.carryingHazmat = Convert.ToBoolean(obj.data[20].str);
        StaticInfo.carryingVictim = Convert.ToBoolean(obj.data[21].str);
        StaticInfo.leadingVictim = Convert.ToBoolean(obj.data[22].str);

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
        
        Dictionary<int[], int> p = new Dictionary<int[], int>();
        foreach (var location in poi.list)
        {
            foreach (KeyValuePair<string, string> entry in location.ToDictionary())
            {
                var key = entry.Key.Split(',').Select(Int32.Parse).ToArray();
                var value = Convert.ToInt32(entry.Value);
                //Debug.Log(key[0] + " "+ key[1] + " "+value);
                p[key] = value;
            }
        }
        StaticInfo.poi = p;
        
        Dictionary<int[], int> mp = new Dictionary<int[], int>();
        foreach (var location in movingPOI.list)
        {
            foreach (KeyValuePair<string, string> entry in location.ToDictionary())
            {
                var key = entry.Key.Split(',').Select(Int32.Parse).ToArray();
                var value = Convert.ToInt32(entry.Value);
                //Debug.Log(key[0] + " "+ key[1] + " "+value);
                mp[key] = value;
            }
        }
        StaticInfo.movingPOI = mp;
        
        Dictionary<int[], int> tp = new Dictionary<int[], int>();
        foreach (var location in treatedPOI.list)
        {
            foreach (KeyValuePair<string, string> entry in location.ToDictionary())
            {
                var key = entry.Key.Split(',').Select(Int32.Parse).ToArray();
                var value = Convert.ToInt32(entry.Value);
                //Debug.Log(key[0] + " "+ key[1] + " "+value);
                tp[key] = value;
            }
        }
        StaticInfo.treatedPOI = tp;
        
        Dictionary<int[], int> mtp = new Dictionary<int[], int>();
        foreach (var location in movingTreatedMemo.list)
        {
            foreach (KeyValuePair<string, string> entry in location.ToDictionary())
            {
                var key = entry.Key.Split(',').Select(Int32.Parse).ToArray();
                var value = Convert.ToInt32(entry.Value);
                //Debug.Log(key[0] + " "+ key[1] + " "+value);
                mtp[key] = value;
            }
        }
        StaticInfo.movingTreatedMemo = mtp;
        
        //hazmat---------
        Dictionary<int[], int> hz = new Dictionary<int[], int>();
        foreach (var location in HazmatMemo.list)
        {
            foreach (KeyValuePair<string, string> entry in location.ToDictionary())
            {
                var key = entry.Key.Split(',').Select(Int32.Parse).ToArray();
                var value = Convert.ToInt32(entry.Value);
                //Debug.Log(key[0] + " "+ key[1] + " "+value);
                hz[key] = value;
            }
        }
        StaticInfo.HazmatMemo = hz;
        
        
        Dictionary<int[], int> hs = new Dictionary<int[], int>();
        foreach (var location in hotSpotMemo.list)
        {
            foreach (KeyValuePair<string, string> entry in location.ToDictionary())
            {
                var key = entry.Key.Split(',').Select(Int32.Parse).ToArray();
                var value = Convert.ToInt32(entry.Value);
                //Debug.Log(key[0] + " "+ key[1] + " "+value);
                hs[key] = value;
            }
        }
        StaticInfo.hotSpotMemo = hs;
        
        
        Dictionary<int[], int> mhs = new Dictionary<int[], int>();
        foreach (var location in movingHazmatMemo.list)
        {
            foreach (KeyValuePair<string, string> entry in location.ToDictionary())
            {
                var key = entry.Key.Split(',').Select(Int32.Parse).ToArray();
                var value = Convert.ToInt32(entry.Value);
                //Debug.Log(key[0] + " "+ key[1] + " "+value);
                mhs[key] = value;
            }
        }
        StaticInfo.movingHazmatMemo = mhs;
        
        //change StartingPosition

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
        room["level"] = StaticInfo.level;
        
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
