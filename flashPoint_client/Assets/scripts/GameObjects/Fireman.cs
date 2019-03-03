using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;


public class Fireman
{
    public String name = "undefined";

    public Colors color = Colors.White;//default to white

    public int AP = 4;//whatever the initial value is

    public int FreeAP = 4;

    public Fireman(String name, Colors color)
    {
        this.name = name;
        this.color = color;
    }

    public void setAP(int ap)
    {
        AP = ap;
    }

        
}
