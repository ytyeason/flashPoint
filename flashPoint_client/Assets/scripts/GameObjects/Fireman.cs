using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;


public class Fireman
{
    public GameObject s;
    
    public String name = "eason";

    public Colors color = Colors.White;//default to white

    public int AP = 4;//whatever the initial value is

    public int FreeAP = 4;

    public Fireman(String name, Colors color, GameObject s)
    {
        this.name = name;
        this.color = color;
        this.s = s;
    }

    public void setAP(int ap)
    {
        AP = ap;
    }

    public void move(int x, int z)
    {
        Debug.Log("moving");
        s.transform.position = new Vector3(x, 0.2f, z);
    }

        
}
