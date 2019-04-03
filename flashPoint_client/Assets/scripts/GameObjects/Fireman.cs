﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;


public class Fireman
{
    public GameObject s;
    public GameObject fireandvictim;
    public TileMap map;
    public TileType[] tileTypes;
	public int currentX;
	public int currentZ;
	public Boolean debugMode = true;        // Toggle this for more descriptive Debug.Log() output
	private static int REFRESH_AP = 4;

    public String name = "eason";

    public Colors color = Colors.White;//default to white

    public int AP = REFRESH_AP;	//whatever the initial value is

    public int FreeAP = REFRESH_AP;

    public int savedAP = 0;

    public int specialtyAP = 0;

    public int remainingSpecAp = 0;

	public bool carryingVictim = false;
    public POI carriedPOI;
    public Hazmat carriedHazmat;

    public bool driving = false;

    public bool riding = false;

    public GameManager gm;

    public POIManager pOIManager;
    public HazmatManager hazamatManager;



    //public int freeExtinguishAp = 3;

    //public int freeMovementAp = 3;

    public Role role = StaticInfo.role;

    public String level = StaticInfo.level;

    public Fireman(String name, Colors color, GameObject s,GameObject firemanplusvictim, int in_x, int in_z, int AP, GameManager gm, Role role,POIManager 
    pOIManager, HazmatManager hazmatManager)
    {
        this.name = name;
        this.color = color;
        this.s=s;
        this.fireandvictim = firemanplusvictim;
		this.currentX = in_x;
		this.currentZ = in_z;
        this.gm = gm;
        setRole(role);
        this.FreeAP = AP + savedAP;
        this.remainingSpecAp = this.specialtyAP;

        this.pOIManager = pOIManager;
        this.hazamatManager = hazmatManager;


        //if (string.Equals(level, "Experienced"))
        //{
        //    setRole(role);
        //}
    }


    public void setAP(int in_ap)
    {
        FreeAP = in_ap;
		gm.displayAP();
    }

    public void setSpecAP(int specAP)
    {
        remainingSpecAp = specAP;
        gm.displayAP();
    }

    // Refresh AP while rolling over unused AP (a maximum of 4)
    public void refreshAP() 
	{
		Debug.Log("EOT AP: " + FreeAP);
        int rollover_AP = Math.Min(FreeAP, 4);

		// Had AP from a previous turn
		//if (FreeAP > 4)
		//{
		//	// Stores values between 0 and 4 which are allowed to be rolled-over
		//	rollover_AP = 4;
		//}
		//// No rollover (i.e. FreeAP < 4
		//else
		//{
		//	rollover_AP = FreeAP % 5;
		//}

		// Change AP
		setAP(AP + rollover_AP);
        savedAP = rollover_AP;
        setSpecAP(specialtyAP);
		Debug.Log("Rolling over: " + rollover_AP);
		Debug.Log("Total AP for new turn is: " + FreeAP);
	}


	// Fireman's method call from Door.cs
	public Boolean changeDoor(int doorX, int doorZ)
	{
		// If debugMode is enabled, better reporting is enabled
		if (debugMode) Debug.Log("Running extuinguishFire(" + doorX + ", " + doorZ + ")");

		// AP check
		if (FreeAP < 1)
		{
			if (debugMode) Debug.Log("ToggleDoor() failed, AP unchanged: " + FreeAP);
			return false;
		}
		// toggleDoor()
		else
		{
			setAP(FreeAP - 1);
			if (debugMode) Debug.Log("Changed (toggleDoor): AP is now: " + FreeAP);
			return true;
		}
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
			setAP(FreeAP - 1);
			if (debugMode) Debug.Log("Changed extFire: AP is now: " + FreeAP);
			return (in_status - 1);
		}
	}

	public Boolean chopWall()
	{
		if (FreeAP >= 2)
		{
			setAP(FreeAP - 2);
			if (debugMode) Debug.Log("AP is now: " + FreeAP);
			return true;
		}
		else
		{
			if (debugMode) Debug.Log("No AP left to chop the Wall!");
			return false;
		}
	}
	
	public void tryMove(int x, int z, int in_status, GameObject gmo) //int[] ct_key, Dictionary<int[], ClickableTile> ct_table)
    {
		// FreeAP must be positive
		if ( FreeAP > 0) {
			// Validate tile
			if (x >= 0 && z >= 0)
			{
				if (x == currentX - 6 || x == currentX + 6 || x == currentX)
				{
					if (z == currentZ - 6 || z == currentZ + 6 || z == currentZ)
					{
						//ClickableTile cur_ct = ct_table[ct_key];
						//Debug.Log("(DEBUG) tryMove(" + x + ", " + z + ")'s spaceState is: " + in_status);


						// Now that chosen ClickableTile is valid, check AP constraints:
						if ( in_status != 2 && FreeAP >= 1 && !carryingVictim) // Safe
						{
							setAP(FreeAP - 1);
							String condition = (debugMode) ? " - ran with (!CarryVictim, Safe, AP >= 1)" : "";
							Debug.Log("AP is now: " + FreeAP + condition);
							move(x, z, gmo);
                            gm.UpdateLocation(x,z);
						}
						else if (in_status == 2 && FreeAP >= 2 && !carryingVictim) // Fire
						{
							setAP(FreeAP - 2);
							String condition = (debugMode) ? " - ran with (!CarryVictim, Fire, AP >= 2)" : "";
							Debug.Log("AP is now: " + FreeAP + condition);
							move(x, z, gmo);
                            gm.UpdateLocation(x,z);
                        }
						else if (in_status != 2 && carryingVictim && FreeAP >= 2)
						{
							setAP(FreeAP - 2);
							String condition = (debugMode) ? " - ran with (CarryVictim, !Fire, AP >= 2)" : "";
							Debug.Log("AP is now: " + FreeAP + condition);
							move(x, z, gmo);
                            gm.UpdateLocation(x,z);
                        }
                        else if (in_status != 2 && carryingVictim && FreeAP >= 2)
                        {
                            setAP(FreeAP - 2);
                            String condition = (debugMode) ? " - ran with (CarryVictim, !Fire, AP >= 2)" : "";
                            //Debug.Log("AP is now: " + FreeAP + condition);
                            move(x, z, gmo);
                            gm.UpdateLocation(x, z);
                        }
                        else
                        {
                            //Debug.Log("Need more than " + FreeAP + " to move to target tile (" + x + ", " + z +")");
                        }
                    }
                    else
                    {
                        //Debug.Log("MoveSelectedUnitTo(z): Fireman can move at most one z-unit at a time.");
                    }
                }
                else
                {
                    //Debug.Log("MoveSelectedUnitTo(x): Fireman can move at most one x-unit at a time.");

                }
            }
			else
			{
				//Debug.Log("MoveSelectedUnitTo(" + x + ", " + z + "): x & z have to be non-negative.");
			}
		}
		else
		{
			//Debug.Log("Fireman.move(" + x + ", " + z + "): FreeAP must be positive to move.");
		}
	}

	// Check if x & z coordinates place fireman outside
	public bool checkOutside(int x_coord, int z_coord)
	{
		if(	(x_coord == 0 || (x_coord / 6) == map.gm.mapSizeX - 1) &&
			(z_coord == 0 || (z_coord / 6) == map.gm.mapSizeZ - 1))
		{
			Debug.Log("x, z : " + x_coord + ", " + z_coord);
			return true;
		}

		return false;
	}


	// Once move is validated the following, unconditionally succesful, move is called
	public void move(int x, int z, GameObject gmo)
	{
		currentX = x;
		currentZ = z;
		s.transform.position = new Vector3(x, 0.2f, z);
		//Debug.Log("x, y is outside: " + checkOutside(x, z));

   //     if(x==5 && z == 5)
   //     {
			////firemanplusvictim firemanandvictim = new firemanplusvictim(name, FreeAP, color, fireandvictim, currentX, currentZ);
			//Debug.Log("You are now carrying the victim!");
			//carryingVictim = true;
			////GameManager gmm = new GameManager();
        //    gm.DestroyObject(gmo);
            
        //    //s = gmm.instantiateObject(firemanandvictim.s, new Vector3(5, 0, 5), Quaternion.identity);
        //}

		// Check if fireman is outside & carrying a victim 
		if(carryingVictim && checkOutside(x, z) == true)
		{
			// Rescue the victim
			gm.rescued_vict_num++;

			Debug.Log("The victim has been rescued!");
		}
    }

    // For operations-----------------------------------------
    public void move(int x, int z)
    {
        int requiredAP = 1;
        if (gm.tileMap.tiles[x, z] == 2||carryingVictim)
        {
            requiredAP = 2;
        }
        if (this.role == Role.RescueSpec) // Rescue Specialist
        {
            if (remainingSpecAp >= requiredAP)
            {
                setSpecAP(remainingSpecAp - requiredAP);
            }else if (remainingSpecAp + FreeAP >= requiredAP)
            {

                setAP(FreeAP - requiredAP + remainingSpecAp);
                setSpecAP(0);
            }
        }
        else
        {
            if (FreeAP >= requiredAP)
            {
                setAP(FreeAP - requiredAP);
            }
        }
        currentX = x * 6;
        currentZ = z * 6;
        s.transform.position = new Vector3(currentX, 0.2f, currentZ);
        gm.UpdateLocation(currentX, currentZ);

        int[] key = new int[] { x, z };
        if (gm.pOIManager.placedPOI.ContainsKey(key)&& gm.pOIManager.placedPOI[key].status==POIStatus.Hidden)
        {
            gm.pOIManager.reveal(x, z);
            pOIManager.gm.updateRevealPOI(x, z);

        }
    }

    public void extingFire(int x, int z)
    {

    }


    public void setRole(Role role)
    {
        this.role = role;

        if (role.Equals(Role.Generalist)){
            AP = 5;
            // role = "Generalist";
        }

        if (role.Equals(Role.ImagingTech))
        {
            AP = 4;
            // role = "imagingTech";
        }

        if (role.Equals(Role.CAFS))
        {
            AP = 3;
            specialtyAP = 3;
            // role = "CAFSfighter";
        }

        if (role == Role.Captain)
        {
            AP = 4;
            specialtyAP = 2;
        }

        if (role == Role.Paramedic)
        {
            AP = 4;
        }

        if (role == Role.HazmatTech)
        {
            AP = 4;
        }

        if (role == Role.RescueSpec)
        {
            AP = 4;
            specialtyAP = 3;
        }

        if (role == Role.Driver)
        {
            AP = 4;
        }

        if (role == Role.Dog)
        {
            
        }

        if (role == Role.Veteran)
        {

        }

        //setAP(AP + savedAP);
        FreeAP = AP + savedAP;

    }

    public void flipPOI(int x, int z)
    {
        pOIManager.reveal(x, z);
        gm.updateRevealPOI(x, z);

    }


    public void removeHazmet(int x, int z)
    {

        hazamatManager.removeHazmat(x, z);

    }

    public void extinFireForFirefighter(int x, int z)
    {

    }



}
