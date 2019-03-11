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

    public Fireman initializeFireman()
    {
        var location = players[StaticInfo.name]["Location"].ToString();
        location = location.Substring(1, location.Length - 2);
        Debug.Log(location);
        if (location.Equals("Top"))
        {
            int ap = Convert.ToInt32(players[StaticInfo.name]["AP"].ToString());
            return new Fireman(StaticInfo.name, Colors.Blue, firemanObject, 0, 0, 1000000);
        }
        else if (location.Equals("Left"))
        {
            int ap = Convert.ToInt32(players[StaticInfo.name]["AP"].ToString());
            return new Fireman(StaticInfo.name, Colors.Blue, firemanObject, 0, 15, ap);
        }
        else if (location.Equals("Right"))
        {
            int ap = Convert.ToInt32(players[StaticInfo.name]["AP"].ToString());
            return new Fireman(StaticInfo.name, Colors.Blue, firemanObject, 45, 15, ap);
        }
        else
        {
            int ap = Convert.ToInt32(players[StaticInfo.name]["AP"].ToString());
            return new Fireman(StaticInfo.name, Colors.Blue, firemanObject, 25, 0, ap);
        }
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
}
