using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
using System;

[Serializable]
public class GamePlayer
{
    public String username;
    private String password;
    public PlayerStatus status;
    Fireman gameChar;

    public GamePlayer(String username, String password, PlayerStatus status, Fireman gameChar)
    {
        this.username = username;
        this.password = password;
        this.status = status;
        this.gameChar = gameChar;
    }
    
}
