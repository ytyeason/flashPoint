
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
//using Newtonsoft.Json;


public class GameManager: MonoBehaviour
{
    public GameObject firemanObject;
    public TileType[] tileTypes;
    public WallType[] wallTypes;
    
    
    public JSONObject game_info = StaticInfo.game_info;
    private List<Game> games = new List<Game>();

    public WallManager wallManager;
    private TileMap tileMap;
    private Fireman fireman;
    
    void Start()
    {
        fireman = new Fireman("eason", Colors.Blue, firemanObject, 0, 0);
        wallManager = new WallManager(wallTypes,this);
        tileMap = new TileMap(tileTypes,this, fireman);

		// Sanity check:
		//Debug.Log("Fireman (x,z): (" + fireman.currentX + ", " + fireman.currentZ + ")");
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
