using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;


public class Fireman
{
    public GameObject s;

	public int currentX;
	public int currentZ;

    public String name = "eason";

    public Colors color = Colors.White;//default to white

    public int AP = 4;//whatever the initial value is

    public int FreeAP = 4;

    public Fireman(String name, Colors color, GameObject s, int in_x, int in_z, int AP)
    {
        this.name = name;
        this.color = color;
        this.s = s;
		this.currentX = in_x;
		this.currentZ = in_z;
        this.AP = AP;
        this.FreeAP = AP;
    }

    public void setAP(int ap)
    {
        AP = ap;
    }

    public void move(int x, int z)
    {
		// Debug:
		/*
		Debug.Log("Input: " + x + ", " + z);
		Debug.Log("selectedUnit: " + currentX + ", " + currentZ);
		*/

		if( FreeAP > 0) {
			if (x >= 0 && z >= 0)
			{
				if (x == currentX - 5 || x == currentX + 5 || x == currentX)
				{
					if (z == currentZ - 5 || z == currentZ + 5 || z == currentZ)
					{
						FreeAP--;
						Debug.Log("AP is now: " + FreeAP);
						currentX = x;
						currentZ = z;
						s.transform.position = new Vector3(x, 0.2f, z);
					}
					else
						Debug.Log("MoveSelectedUnitTo(z): Fireman can move at most one z-unit at a time.");
				}
				else
					Debug.Log("MoveSelectedUnitTo(x): Fireman can move at most one x-unit at a time.");
			}
			else
			{
				Debug.Log("MoveSelectedUnitTo(" + x + ", " + z + "): x & z have to be non-negative.");
			}
		}
		else
		{
			Debug.Log("Fireman.move(" + x + ", " + z + "): FreeAP must be positive to move.");
		}
	}

        
}
