using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
using System.Linq;

//using Newtonsoft.Json;
//using System.Web.Script.Serialization;


public class GameManager: MonoBehaviour
{
    public SocketIOComponent socket;
    public GameObject firemanObject;
    public TileType[] tileTypes;
    public WallType[] wallTypes;

    
    
    public JSONObject game_info = StaticInfo.game_info;

    public WallManager wallManager;
    private TileMap tileMap;

    private JSONObject room;
    private JSONObject participants;
    private String level;
    private String numberOfPlayer;
    private Dictionary<String, JSONObject> players = new Dictionary<string, JSONObject>();

    public Fireman fireman;

    
    void Start()
    {
        StartCoroutine(ConnectToServer());
        socket.On("LocationUpdate_SUCCESS", LocationUpdate_SUCCESS);

        if (game_info != null) 
        {
            room = game_info[StaticInfo.roomNumber];
            participants = room["participants"];
            level = room["level"].ToString();
            numberOfPlayer = room["numberOfPlayer"].ToString();

            List<string> p = participants.keys;
            foreach (var v in p)
            {
                //Debug.Log(participants[v]);
                var o = participants[v];
                players[v] = o;
                Debug.Log(players[v]);
            }
        }

        fireman = initializeFireman();
        wallManager = new WallManager(wallTypes,this);
        tileMap = new TileMap(tileTypes,this, fireman);

    }

    void LocationUpdate_SUCCESS(SocketIOEvent obj)
    {
        Debug.Log("Location update successful");
    }

    IEnumerator ConnectToServer()
    {
        yield return new WaitForSeconds(0.5f);

        socket.Emit("USER_CONNECT");

        yield return new WaitForSeconds(0.5f);

    }

    public Fireman initializeFireman()
    {
        var location = players[StaticInfo.name]["Location"].ToString();
        location = location.Substring(1, location.Length - 2);
        Debug.Log(location);

        var cord = location.Split(',');
        int x = Convert.ToInt32(0);
        int z = Convert.ToInt32(5);

        int ap = Convert.ToInt32(players[StaticInfo.name]["AP"].ToString());
        return new Fireman(StaticInfo.name, Colors.Blue, firemanObject, x, z, 100000, this);

    }

    public GameObject instantiateObject(GameObject w, Vector3 v, Quaternion q)
    {
        GameObject objectW = (GameObject)Instantiate(w, v, q);
        return objectW;
    }

    public void DestroyObject(GameObject w)
    {
        Destroy(w);
    }

    public void UpdateLocation(int x, int z)
    {
        Debug.Log("Update Location");
        StaticInfo.Location = new int[] { x, z };
        Dictionary<String, String> update = new Dictionary<string, string>();
        update["room"] = StaticInfo.roomNumber;
        update["name"] = StaticInfo.name;
        update["Location"] = StaticInfo.Location[0] + "," + StaticInfo.Location[1];

        socket.Emit("Location", new JSONObject(update));
    }
}
