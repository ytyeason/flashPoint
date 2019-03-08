
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
//using Newtonsoft.Json;


public class GameManager: MonoBehaviour
{
    public JSONObject game_info = StaticInfo.game_info;
    private List<Game> games = new List<Game>();
    
    void Start()
    {
        
        
        
    }
}
