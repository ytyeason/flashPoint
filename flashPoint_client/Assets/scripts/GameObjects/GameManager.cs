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
    private Fireman fireman;

    private JSONObject room;
    private String participants;
    private JSONObject level;
    private JSONObject numberOfPlayer;
    
    void Start()
    {
        if (game_info != null) 
        {
            room = game_info[StaticInfo.roomNumber];
            participants = room["participants"].ToString();
            participants = participants.Substring(1, participants.Length - 2);//remove [ and ]
            level = room["level"];
            numberOfPlayer = room["numberOfPlayer"];
            List<string> p = participants.Split(',').ToList();
            foreach (var v in p)
            {
                Debug.Log(v.Substring(1, v.Length - 2));
            }

        }
        /*
        fireman = new Fireman("eason", Colors.Blue, firemanObject);
        wallManager = new WallManager(wallTypes,this);
        tileMap = new TileMap(tileTypes,this, fireman);
        */

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
