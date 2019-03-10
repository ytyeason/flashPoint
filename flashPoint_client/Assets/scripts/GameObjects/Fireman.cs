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
	public Boolean debugMode = true;		// Toggle this for more descriptive Debug.Log() output

    public String name = "eason";

    public Colors color = Colors.White;//default to white

    public int AP = 5;//whatever the initial value is

    public int FreeAP = 10;

	public Boolean carryingVictim = false;

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

	// Pre-condition via ClickableTiles: in_status != 0
	public int extinguishFire(int in_status)
	{
		if (debugMode) Debug.Log("Running extuinguishFire(" + in_status + ")");

		// AP check
		if (FreeAP < 1)
		{
			if (debugMode) Debug.Log("AP unchanged: " + FreeAP);
			return -1;
		}
		else // Fire -> Smoke || Smoke -> Normal: 1 AP
		{
			FreeAP -= 1;
			if (debugMode) Debug.Log("Changed extFire: AP is now: " + FreeAP);
			return (in_status - 1);
		}
	}

	public Boolean chopWall()
	{
		if (FreeAP >= 2)
		{
			FreeAP -= 2;
			if (debugMode) Debug.Log("AP is now: " + FreeAP);
			return true;
		}
		else
		{
			Debug.Log("No AP left to chop the Wall!");
			return false;
		}
	}

	public void tryMove(int x, int z, int in_status)//int[] ct_key, Dictionary<int[], ClickableTile> ct_table)
    {
		// FreeAP must be positive
		if ( FreeAP > 0) {
			// Validate tile
			if (x >= 0 && z >= 0)
			{
				if (x == currentX - 5 || x == currentX + 5 || x == currentX)
				{
					if (z == currentZ - 5 || z == currentZ + 5 || z == currentZ)
					{
						//ClickableTile cur_ct = ct_table[ct_key];
						//Debug.Log("(DEBUG) tryMove(" + x + ", " + z + ")'s spaceState is: " + in_status);


						// Now that chosen ClickableTile is valid, check AP constraints:
						if ( in_status != 2 && FreeAP >= 1 && !carryingVictim) // Safe
						{
							FreeAP--;
							String condition = (debugMode) ? " - ran with (!CarryVictim, Safe, AP >= 1)" : "";
							Debug.Log("AP is now: " + FreeAP + condition);
							move(x, z);
						}
						else if (in_status == 2 && FreeAP >= 2 && !carryingVictim) // Fire
						{
							FreeAP-=2;
							String condition = (debugMode) ? " - ran with (!CarryVictim, Fire, AP >= 2)" : "";
							Debug.Log("AP is now: " + FreeAP + condition);
							move(x, z);
						}
						else if (in_status != 2 && carryingVictim && FreeAP >= 2)
						{
							FreeAP -= 2;
							String condition = (debugMode) ? " - ran with (CarryVictim, !Fire, AP >= 2)" : "";
							Debug.Log("AP is now: " + FreeAP + condition);
							move(x, z);
						}
						else{
							Debug.Log("Need more than " + FreeAP + " to move to target tile (" + x + ", " + z +")");
						}
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

	// Once move is validated the following, unconditionally succesful, move is called
	public void move(int x, int z)
	{
		currentX = x;
		currentZ = z;
		s.transform.position = new Vector3(x, 0.2f, z);
	}



}
