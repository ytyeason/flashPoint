﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;


public class Game {
    
    private Dictionary<String, Fireman> fireman_manager = new Dictionary<string, Fireman>();
    
    private Dictionary<String, GamePlayer> player_manager = new Dictionary<string, GamePlayer>();
    
    private ArrayList VictimManager = new ArrayList();
    
    private ArrayList FalseAlarmManager = new ArrayList();
    
    private Dictionary<int, Wall> WallManager = new Dictionary<int, Wall>();

    private Dictionary<int, Space> SpaceManager = new Dictionary<int, Space>();

    public GameStatus status = GameStatus.ReadyToJoin;

    public int redDice = 8;//temp

    public int blackDice = 8;//temp

    public int numOfDesiredPlayer;

    public DifficultyLevel level = DifficultyLevel.Easy;
    
    public Game(int num, DifficultyLevel level)
    {
        numOfDesiredPlayer = num;
        this.level = level;
        
        Debug.Log("new Game created!");
        Fireman f1 = new Fireman("f1",Colors.Blue);
        Fireman f2 = new Fireman("f2", Colors.Red);
        Fireman f3 = new Fireman("f3",Colors.Yellow);

        fireman_manager["f1"] = f1;
        fireman_manager["f2"] = f2;
        fireman_manager["f3"] = f3;
        
        GamePlayer p1 = new GamePlayer("p1" ,"666", PlayerStatus.Available, f1);
        GamePlayer p2 = new GamePlayer("p2" ,"666", PlayerStatus.Available, f2);
        GamePlayer p3 = new GamePlayer("p3" ,"666", PlayerStatus.Available, f3);
    }

    public Dictionary<String,Fireman> getFiremanManager()
    {
        return fireman_manager;
    }
    
    public Dictionary<String,GamePlayer> getPlayerManager()
    {
        return player_manager;
    }
    
    //todo: a lot of methods left to be added in M5


}