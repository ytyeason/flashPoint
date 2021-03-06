﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FireManager : MonoBehaviour
{

	//private Random rng;
	private int mapSizeX; // Should be 10
	private int mapSizeZ; // Should be 8
	public GameManager gm;
	public TileMap tileMap;
	public Boolean debugMode = true;        // Toggle this for more descriptive Debug.Log() output

    public LinkedList<int[]> hazametList = new LinkedList<int[]>();



	// Use this for initialization
	public FireManager(GameManager in_gm, TileMap in_tileMap, int in_X, int in_Z)
	{
		this.gm = in_gm;
		this.tileMap = in_tileMap;  //initFireTileStores(in_tileMap);
		this.mapSizeX = in_X;
		this.mapSizeZ = in_Z;
		hazametList=new LinkedList<int[]>();
	}

	public void explosion(){
		while (hazametList.Count != 0)
		{
			int[] tmp = hazametList.First.Value;
			hazametList.RemoveFirst();
			Debug.Log("Check Location");
			Debug.Log(tmp[0]+","+tmp[1]);
			gm.hazmatManager.explodeHazmat(tmp[0], tmp[1]);
			gm.explodeHazmat(tmp[0],tmp[1]);
			keepGoingUp(tmp[0],tmp[1]);
			keepGoingDown(tmp[0],tmp[1]);
			keepGoingLeft(tmp[0],tmp[1]);
			keepGoingRight(tmp[0],tmp[1]);
		}
	}

	// Pseudo-controller function called by GameManager when turn is changed
	public void advanceFire(int in_x, int in_z, bool isATest)
	{
		// Places the new Smoke marker (n.b. might be 'more' than Smoke due to rule interactions)
		if( debugMode ) Debug.Log("NewFire running:");
		newFire(in_x, in_z, isATest);

		// Flashover step:
		if (debugMode) Debug.Log("flashover:");
		flashover();

		Debug.Log("List Count");
		Debug.Log(hazametList.Count);
		
		explosion();
		
		// Final step
		if (debugMode) Debug.Log("extOutFire:");
		extOutFire();

		// Recursively resolve flareups
		if(gm.hazmatManager.containsKey(in_x,in_z,gm.hazmatManager.placedHotspot)){
			System.Random rand=new System.Random();
			int x=rand.Next(1,8);
			int z=rand.Next(1,6);
			advanceFire(x,z,isATest);
		}
		
	}


	// Called to "Remove any Fire markers that were placed outside of the building"
	public void extOutFire()
	{
		// Extinguish fires above and below the house
		for (int x_elem = 0; x_elem < mapSizeX; x_elem++)
		{
			if (tileMap.tiles[x_elem, 0] == 2)	// Bottom row is on Fire
			{
				Debug.Log("Extinguishing fire on (" + x_elem + ",0)");
                if(x_elem==3||x_elem==4){
                    tileMap.buildNewTile(x_elem, 0, 4);
                    tileMap.gm.UpdateTile(x_elem, 0, 4);
                }
                else if(x_elem==1||x_elem==2){
                    tileMap.buildNewTile(x_elem, 0, 3);
                    tileMap.gm.UpdateTile(x_elem, 0, 3);
                }
                else{
                    tileMap.buildNewTile(x_elem, 0, 0);
                    tileMap.gm.UpdateTile(x_elem, 0, 0);
                }
			}
			if(tileMap.tiles[x_elem, mapSizeZ - 1] == 2) // Top row is on fire
			{
				Debug.Log("Extinguishing fire on (" + x_elem + "," + (mapSizeZ - 1) + ")");
                if(x_elem==5||x_elem==6){
                    tileMap.buildNewTile(x_elem, mapSizeZ - 1, 4);
                    tileMap.gm.UpdateTile(x_elem, mapSizeZ - 1, 4);
                }
                else if(x_elem==7||x_elem==8){
                    tileMap.buildNewTile(x_elem, mapSizeZ - 1, 3);
                    tileMap.gm.UpdateTile(x_elem, mapSizeZ - 1, 3);
                }
                else{
                    tileMap.buildNewTile(x_elem, mapSizeZ - 1, 0);
                    tileMap.gm.UpdateTile(x_elem, mapSizeZ - 1, 0);
                }
			}
		}

		// Extinguish fires on the left and right of the house
		for (int z_elem = 0; z_elem < mapSizeZ; z_elem++)
		{
			if (tileMap.tiles[0, z_elem] == 2)  
			{
				Debug.Log("Extinguishing fire on (0," + z_elem + ")");
                if(z_elem==3||z_elem==4){
                    tileMap.buildNewTile(0, z_elem, 4);
                    tileMap.gm.UpdateTile(0, z_elem, 4);
                }
                else if(z_elem==5||z_elem==6){
                    tileMap.buildNewTile(0, z_elem, 3);
                    tileMap.gm.UpdateTile(0, z_elem, 3);
                }
                else{
                    tileMap.buildNewTile(0, z_elem, 0);
                    tileMap.gm.UpdateTile(0, z_elem, 0);
                }

			}
			if (tileMap.tiles[mapSizeX - 1, z_elem] == 2) 
			{
				Debug.Log("Extinguishing fire on (" + (mapSizeX - 1) + "," + z_elem + ")");
                if(z_elem==3||z_elem==4){
                    tileMap.buildNewTile(mapSizeX - 1, z_elem, 4);
                    tileMap.gm.UpdateTile(mapSizeX - 1, z_elem, 4);
                }
                else if(z_elem==1||z_elem==2){
                    tileMap.buildNewTile(mapSizeX - 1, z_elem, 3);
                    tileMap.gm.UpdateTile(mapSizeX - 1, z_elem, 3);
                }
                else{
                    tileMap.buildNewTile(mapSizeX - 1, z_elem, 0);
                    tileMap.gm.UpdateTile(mapSizeX - 1, z_elem, 0);
                }
			}
		}
	}


	// Called when flashover is triggered
	public void flashover()
	{
		bool canLeft = false;
		bool canUp = false;
		bool canDown = false;
		bool canRight = false;
		DoorManager dm = gm.doorManager;
		WallManager wm = gm.wallManager;
		bool adjOnFire = false;
		bool flashoverTriggered = false;

		// Until nothing triggers we keep looping (i.e. until no smoke can be transformed into a Fire marker
		while(true) {
		// Want to flip the smoke tile if a neighbouring tile can ignite
			for (int x_elem = 0; x_elem < mapSizeX; x_elem++) 
			{
				for (int z_elem = 0; z_elem < mapSizeZ; z_elem++)
				{
					if (tileMap.tiles[x_elem, z_elem] != 1) continue;
				
					// Prep for adjacent Fire check:
					adjOnFire = false;
					if (x_elem >= 1) { canLeft = true; }
					else { canLeft = false; }

					if (x_elem < mapSizeX - 1) { canRight = true; }
					else { canRight = false; }

					if (z_elem >= 1) { canDown = true; }
					else { canDown = false; }
				
					if (z_elem < mapSizeZ - 1) { canUp = true; }
					else { canUp = false; }

				
					// If the tile is next to a Fire marker; if there's an obstacle in the way then Fire isn't 'adjacent'
					if(canUp && tileMap.tiles[x_elem, z_elem + 1] == 2 && !adjOnFire &&
								!(dm.checkIfHDoor(x_elem, z_elem + 1)&&!dm.checkIfOpenHDoor(x_elem, z_elem + 1) || wm.checkIfHWall(x_elem, z_elem + 1)))
					{
							adjOnFire = true;
					}
				
					if (canDown && tileMap.tiles[x_elem, z_elem - 1] == 2 && !adjOnFire &&
								!(dm.checkIfHDoor(x_elem, z_elem)&&!dm.checkIfOpenHDoor(x_elem, z_elem) || wm.checkIfHWall(x_elem, z_elem)))
					{
							adjOnFire = true;
					}
	
					if(canLeft && tileMap.tiles[x_elem - 1, z_elem] == 2 && !adjOnFire &&
								!(dm.checkIfVDoor(x_elem, z_elem)&&!dm.checkIfOpenVDoor(x_elem, z_elem) || wm.checkIfVWall(x_elem, z_elem)))
					{
							adjOnFire = true;
					}
				
					if(canRight && tileMap.tiles[x_elem + 1, z_elem] == 2 && !adjOnFire &&
								!(dm.checkIfVDoor(x_elem + 1, z_elem)&&!dm.checkIfOpenVDoor(x_elem + 1, z_elem) || wm.checkIfVWall(x_elem + 1, z_elem)))
					{
							adjOnFire = true;
					}


					// If any adjacent tiles were on Fire ('legitimately') then we change our tile from Smoke to Fire
					if (adjOnFire == true)
					{
						// Sanity check:
						if (debugMode)
						{
							Debug.Log("");
							Debug.Log("canLeft: " + canLeft);
							Debug.Log("canUp: " + canUp);
							Debug.Log("canDown: " + canDown);
							Debug.Log("canRight: " + canRight);
							Debug.Log("(x,z):  " + x_elem + ", " + z_elem);
							Debug.Log("");
						}

						tileMap.buildNewTile(x_elem, z_elem, 2);
						tileMap.gm.UpdateTile(x_elem, z_elem, 2);
						flashoverTriggered = true;
						// if(gm.hazmatManager.isHazamet(x_elem,z_elem)){
						// 	int[] tmp = new int[2];
                		//     tmp[0] = x_elem;
                		//     tmp[1] = z_elem;
					    // 	hazametList.AddLast(tmp);
						// }
						
					}
					
				}
			}

			// We reach here if, at no point in the loop, there was a flashover
			if(flashoverTriggered == true) {
				flashoverTriggered = false;
				continue;
			}
			else
			{
				break;
			}
			
		}
	}

	// Recursive function to propagate shockwave upwards
	public void keepGoingUp(int rng_X, int rng_Z)
    {
        
		Debug.Log("Going up");
        if (rng_X >= 1 && rng_X <= 8 && rng_Z >= 1 && rng_Z <= 6)
        {
            //TOP 

            //if this is wall
            if (gm.wallManager.checkIfHWall(rng_X, rng_Z+1))
            {
                int type = gm.wallManager.HorizontalWall(rng_X, rng_Z+1);
                type += 2;
                if (type == 2 || type == 4)

                {
                    gm.wallManager.BreakWall(rng_X, rng_Z+1, type, 1, true);
                    gm.UpdateWall(rng_X, rng_Z+1, type, 1,true); // horizontal
                }


            }
            //if this is door 
            else if (gm.doorManager.checkIfHDoor(rng_X, rng_Z+1)&&!gm.doorManager.checkIfOpenHDoor(rng_X, rng_Z+1))
                {
                int type = gm.doorManager.HorizontalDoor(rng_X, rng_Z+1);
                type += 4;
                if (type == 4)

                {
                    gm.doorManager.ChangeDoor(rng_X, rng_Z+1, type, 2,true);
                    gm.UpdateDoor(rng_X, rng_Z+1, type, 2,true);// horizontal
                }

            }
            //nothing or smoke
            else if (tileMap.tiles[rng_X, rng_Z + 1] == 0 || tileMap.tiles[rng_X, rng_Z + 1] == 1||gm.doorManager.checkIfOpenHDoor(rng_X, rng_Z+1))
            {
                tileMap.buildNewTile(rng_X, rng_Z+1, 2);
                tileMap.gm.UpdateTile(rng_X, rng_Z+1, 2);
				if(gm.doorManager.checkIfOpenHDoor(rng_X, rng_Z+1)){
					int type = gm.doorManager.HorizontalDoor(rng_X, rng_Z+1);
					gm.doorManager.ChangeDoor(rng_X, rng_Z+1, type+2, type,true);
                    gm.UpdateDoor(rng_X, rng_Z+1, type+2, type,true);// horizontal
				}
            }
            // else if (gm.hazmatManager.containsKey(rng_X, rng_Z,gm.hazmatManager.placedHazmat)||gm.hazmatManager.containsKey(rng_X, rng_Z,gm.hazmatManager.movingHazmat)){ 

            // int[] tmp = new int[2];
            //     tmp[0] = rng_X;
            //     tmp[1] = rng_Z;
            //     Boolean t = true;
            //     for (LinkedListNode<int[]> node = hazametList.First; node != null; node = node.Next)
            //     {
            //         if(node.Value[0]==rng_X && node.Value[1] == rng_Z)
            //         {
            //             t = false;
            //         }
            //     }
            //     if (t )
            //     {
                    
            //         hazametList.AddLast(tmp);
            //     }
               
            // }

            else
            {
                //recursion function
				// tileMap.buildNewTile(rng_X, rng_Z+1, 2);
        		// tileMap.gm.UpdateTile(rng_X, rng_Z+1, 2);
                keepGoingUp(rng_X, rng_Z + 1);
            }
        }
    }

	// Recursive function to propagate shockwave leftwards
	public void keepGoingLeft(int rng_X, int rng_Z)
    {
        
		Debug.Log("Going left");
        if (rng_X >= 1 && rng_X <= 8 && rng_Z >= 1 && rng_Z <= 6)
        {
            //TOP 

            //if this is wall
            if (gm.wallManager.checkIfVWall(rng_X, rng_Z))
            {
                int type = gm.wallManager.VerticalWall(rng_X, rng_Z);
                type += 2;
                if (type == 3 || type == 5)

                {
                    gm.wallManager.BreakWall(rng_X, rng_Z, type, 0, true);
                    gm.UpdateWall(rng_X, rng_Z, type, 0, true); // 
                }


            }
            //if this is door 
            else if (gm.doorManager.checkIfVDoor(rng_X, rng_Z)&&!gm.doorManager.checkIfOpenVDoor(rng_X, rng_Z))
                {
                int type = gm.doorManager.VerticalDoor(rng_X, rng_Z);
                type += 4;
                if (type == 5)

                {
                    gm.doorManager.ChangeDoor(rng_X, rng_Z, type, 3,true);
                    gm.UpdateDoor(rng_X, rng_Z, type, 3,true);// horizontal
                }

            }
            //nothing or smoke
            else if (tileMap.tiles[rng_X-1, rng_Z ] == 0 || tileMap.tiles[rng_X-1, rng_Z ] == 1||gm.doorManager.checkIfOpenVDoor(rng_X, rng_Z))
            {
                tileMap.buildNewTile(rng_X-1, rng_Z, 2);
                tileMap.gm.UpdateTile(rng_X-1, rng_Z, 2);
				if(gm.doorManager.checkIfOpenVDoor(rng_X, rng_Z)){
					int type = gm.doorManager.VerticalDoor(rng_X, rng_Z);
					gm.doorManager.ChangeDoor(rng_X, rng_Z, type+2, type,true);
                    gm.UpdateDoor(rng_X, rng_Z, type+2, type,true);// horizontal
				}
            }
            // else if (gm.hazmatManager.containsKey(rng_X, rng_Z,gm.hazmatManager.placedHazmat)||gm.hazmatManager.containsKey(rng_X, rng_Z,gm.hazmatManager.movingHazmat))
            // {

            //     int[] tmp = new int[2];
            //     tmp[0] = rng_X;
            //     tmp[1] = rng_Z;
            //     Boolean t = true;
            //     for (LinkedListNode<int[]> node = hazametList.First; node != null; node = node.Next)
            //     {
            //         if (node.Value[0] == rng_X && node.Value[1] == rng_Z)
            //         {
            //             t = false;
            //         }
            //     }
            //     if (t)
            //     {
                    
            //         hazametList.AddLast(tmp);
            //     }
            // }
            else
            {
                //recursion function 
				// tileMap.buildNewTile(rng_X-1, rng_Z, 2);
        		// tileMap.gm.UpdateTile(rng_X-1, rng_Z, 2);
                keepGoingLeft(rng_X-1, rng_Z );
            }



        }
    }

	// Recursive function to propagate shockwave rightwards
	public void keepGoingRight(int rng_X, int rng_Z)
    {
        Debug.Log("Going right");
        if (rng_X >= 1 && rng_X <= 8 && rng_Z >= 1 && rng_Z <= 6)
        {
            //TOP 

            //if this is wall
            if (gm.wallManager.checkIfVWall(rng_X+1, rng_Z))
            {
                int type = gm.wallManager.VerticalWall(rng_X+1, rng_Z);
                type += 2;
                if (type == 3 || type == 5)

                {
                    gm.wallManager.BreakWall(rng_X+1, rng_Z, type, 0, true);
                    gm.UpdateWall(rng_X+1, rng_Z, type, 0,true); // 
                }


            }
            //if this is door 
            else if (gm.doorManager.checkIfVDoor(rng_X+1, rng_Z)&&!gm.doorManager.checkIfOpenVDoor(rng_X+1, rng_Z))
                {
                int type = gm.doorManager.VerticalDoor(rng_X+1, rng_Z);
                type += 4;
                if (type == 5)

                {
                    gm.doorManager.ChangeDoor(rng_X+1, rng_Z, type, 3,true);
                    gm.UpdateDoor(rng_X+1, rng_Z, type, 3,true);// horizontal
                }

            }
            //nothing or smoke
            else if (tileMap.tiles[rng_X+1, rng_Z ] == 0 || tileMap.tiles[rng_X+1, rng_Z ] == 1||gm.doorManager.checkIfOpenVDoor(rng_X+1, rng_Z))
            {
                tileMap.buildNewTile(rng_X+1, rng_Z, 2);
                tileMap.gm.UpdateTile(rng_X+1, rng_Z, 2);
				if(gm.doorManager.checkIfOpenVDoor(rng_X+1, rng_Z)){
					int type = gm.doorManager.VerticalDoor(rng_X+1, rng_Z);
					gm.doorManager.ChangeDoor(rng_X+1, rng_Z, type+2, type,true);
                    gm.UpdateDoor(rng_X+1, rng_Z, type+2, type,true);// horizontal
				}
            }
            // else if (gm.hazmatManager.containsKey(rng_X, rng_Z,gm.hazmatManager.placedHazmat)||gm.hazmatManager.containsKey(rng_X, rng_Z,gm.hazmatManager.movingHazmat))
            // {

            //     int[] tmp = new int[2];
            //     tmp[0] = rng_X;
            //     tmp[1] = rng_Z;
            //     Boolean t = true;
            //     for (LinkedListNode<int[]> node = hazametList.First; node != null; node = node.Next)
            //     {
            //         if (node.Value[0] == rng_X && node.Value[1] == rng_Z)
            //         {
            //             t = false;
            //         }
            //     }
            //     if (t )
            //     {
                   
            //         hazametList.AddLast(tmp);
            //     }
            // }
            else
            {
				//recursion function 
				// tileMap.buildNewTile(rng_X+1, rng_Z, 2);
                // tileMap.gm.UpdateTile(rng_X+1, rng_Z, 2);
				keepGoingRight(rng_X +1, rng_Z);
            }



        }
    }

	// Recursive function to propagate shockwave downwards
    public void keepGoingDown(int rng_X, int rng_Z)
    {
        Debug.Log("Going down");
        if (rng_X >= 1 && rng_X <= 8 && rng_Z >= 1 && rng_Z <= 6)
        {
            //TOP 

            //if this is wall
            if (gm.wallManager.checkIfHWall(rng_X , rng_Z))
            {
                int type = gm.wallManager.HorizontalWall(rng_X , rng_Z);
                type += 2;
                if (type == 2 || type == 4)

                {
                    gm.wallManager.BreakWall(rng_X, rng_Z, type, 1, true);
                    gm.UpdateWall(rng_X, rng_Z, type, 1,true); // 
                }


            }
            //if this is door 
            else if (gm.doorManager.checkIfHDoor(rng_X , rng_Z)&&!gm.doorManager.checkIfOpenHDoor(rng_X,rng_Z))
                {
                int type = gm.doorManager.HorizontalDoor(rng_X, rng_Z);
                type += 4;
                if (type == 4)

                {
                    gm.doorManager.ChangeDoor(rng_X, rng_Z, type, 2,true);
                    gm.UpdateDoor(rng_X, rng_Z, type, 2,true);// horizontal
                }
				// if(type==6){
				// 	gm.doorManager.ChangeDoor(rng_X, rng_Z, type-2, 1);
                //     gm.UpdateDoor(rng_X, rng_Z, type-2, 1);// horizontal
				// }

            }
            //nothing or smoke or open door
            else if (tileMap.tiles[rng_X , rng_Z-1] == 0 || tileMap.tiles[rng_X , rng_Z-1] == 1||gm.doorManager.checkIfOpenHDoor(rng_X,rng_Z))
            {
                tileMap.buildNewTile(rng_X, rng_Z-1, 2);
                tileMap.gm.UpdateTile(rng_X, rng_Z-1, 2);
				if(gm.doorManager.checkIfOpenHDoor(rng_X,rng_Z)){
					int type = gm.doorManager.HorizontalDoor(rng_X, rng_Z);
					gm.doorManager.ChangeDoor(rng_X, rng_Z, type+2, type,true);
                    gm.UpdateDoor(rng_X, rng_Z, type+2, type,true);// horizontal
				}
            }
            // else if (gm.hazmatManager.containsKey(rng_X, rng_Z,gm.hazmatManager.placedHazmat)||gm.hazmatManager.containsKey(rng_X, rng_Z,gm.hazmatManager.movingHazmat))
            // {

            //     int[] tmp = new int[2];
            //     tmp[0] = rng_X;
            //     tmp[1] = rng_Z;
            //     Boolean t = true;
            //     for (LinkedListNode<int[]> node = hazametList.First; node != null; node = node.Next)
            //     {
            //         if (node.Value[0] == rng_X && node.Value[1] == rng_Z)
            //         {
            //             t = false;
            //         }
            //     }
            //     if (t )
            //     {
                   
            //         hazametList.AddLast(tmp);
            //     }
            // }
            else
            {
				//recursion function 
				// tileMap.buildNewTile(rng_X, rng_Z-1, 2);
                // tileMap.gm.UpdateTile(rng_X, rng_Z-1, 2);

				keepGoingDown(rng_X , rng_Z-1);
            }

        }
    }

	// Begin phase 1 of advanceFire: placing the new Smoke marker
	public void newFire(int in_x, int in_z, bool isATest)
	{
		// Coordinate to place the new Smoke marker
		int rng_X = (isATest) ? in_x : UnityEngine.Random.Range(1, mapSizeX - 2);
		int rng_Z = (isATest) ? in_z : UnityEngine.Random.Range(1, mapSizeZ - 2);
		int current_type = tileMap.tiles[rng_X, rng_Z];
		if (debugMode) Debug.Log("Type is: " + current_type);
		
		Boolean canLeft = false;
		Boolean canUp = false;
		Boolean canDown = false;
		Boolean canRight = false;
		
		DoorManager dm = gm.doorManager;
		WallManager wm = gm.wallManager;
		Boolean adjOnFire = false;

		// If the tile has only a Smoke
		if (current_type == 1)
		{
			// Flip Smoke marker to Fire
			if (debugMode) Debug.Log("Flipping Smoke -> Fire");
			tileMap.buildNewTile(rng_X, rng_Z, 2);
			tileMap.gm.UpdateTile(rng_X, rng_Z, 2);
			return;
		}
		else if (current_type == 2)
		{
			if (debugMode) Debug.Log("Explosion happens at:" + rng_X + rng_Z);
			// Trigger explosion in all four directions
            keepGoingUp(rng_X, rng_Z);
            keepGoingDown(rng_X, rng_Z);
            keepGoingLeft(rng_X, rng_Z);
            keepGoingRight(rng_X, rng_Z);

            // tileMap.buildNewTile(rng_X, rng_Z, 2);
            // tileMap.gm.UpdateTile(rng_X, rng_Z, 2);

           

            return;
		}

		// Prep for adjacent Fire check:
		if (rng_X >= 1)
		{
			canLeft = true;
		}
		if (rng_X < mapSizeX - 1)
		{
			canRight = true;
		}
		if (rng_Z >= 1)
		{
			canDown = true;
		}
		if (rng_Z < mapSizeZ - 1)
		{
			canUp = true;
		}
		// Sanity check:
		if (debugMode)
		{
			Debug.Log("canLeft: " + canLeft);
			Debug.Log("canUp: " + canUp);
			Debug.Log("canDown: " + canDown);
			Debug.Log("canRight: " + canRight);
		}


		// If the tile is next to a Fire marker; if there's an obstacle in the way then Fire isn't 'adjacent'
		if (canUp && tileMap.tiles[rng_X, rng_Z + 1] == 2 && !adjOnFire &&
				!(dm.checkIfHDoor(rng_X, rng_Z + 1)&&!dm.checkIfOpenHDoor(rng_X, rng_Z + 1) || wm.checkIfHWall(rng_X, rng_Z + 1)))
		{
			adjOnFire = true;
		}
		if (canDown && tileMap.tiles[rng_X, rng_Z - 1] == 2 && !adjOnFire &&
				!(dm.checkIfHDoor(rng_X, rng_Z)&&!dm.checkIfOpenHDoor(rng_X, rng_Z) || wm.checkIfHWall(rng_X, rng_Z)))
		{
			adjOnFire = true;
		}
		if (canLeft && tileMap.tiles[rng_X - 1, rng_Z] == 2 && !adjOnFire &&
				!(dm.checkIfVDoor(rng_X, rng_Z)&&!dm.checkIfOpenVDoor(rng_X, rng_Z) || wm.checkIfVWall(rng_X, rng_Z)))
		{
			adjOnFire = true;
		}
		if (canRight && tileMap.tiles[rng_X + 1, rng_Z] == 2 && !adjOnFire &&
				!(dm.checkIfVDoor(rng_X + 1, rng_Z)&&!dm.checkIfOpenVDoor(rng_X + 1, rng_Z) || wm.checkIfVWall(rng_X + 1, rng_Z)))
		{
			adjOnFire = true;
		}

		// If any adjacent tiles were on Fire ('legitimately') then we change our tile from Smoke to Fire
		if (adjOnFire == true)
		{
			tileMap.buildNewTile(rng_X, rng_Z, 2);
			tileMap.gm.UpdateTile(rng_X, rng_Z, 2);
			return;
		}

		// We reach here if the tile should simply have a Smoke marker
		tileMap.buildNewTile(rng_X, rng_Z, 1);
		tileMap.gm.UpdateTile(rng_X, rng_Z, 1);
	}
}