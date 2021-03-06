﻿//	WALL MANAGER

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;
using System.Linq;
using System;

[Serializable]
public static class hasComponent
{
	public static bool HasComponent<T>(this GameObject flag) where T : Component
	{
		if (flag != null)
		{
			Debug.Log("flag wasn't null");
			return flag.GetComponent<T>() != null;
		}
		Debug.Log("flag was null");
		return false;
	}
}
[Serializable]
public class WallManager
{

	readonly int wallMapSizeX = 10;
	readonly int wallMapSizeZ = 8;

	public Dictionary<int[], GameObject> hwallStores = new Dictionary<int[], GameObject>();
	public Dictionary<int[], GameObject> vwallStores = new Dictionary<int[], GameObject>();

	public List<int[]> defaultHorizontalWalls = new List<int[]>();
	public List<int[]> defaultVerticalWalls = new List<int[]>();
	
	//public Dictionary<int[],int> defaultHorizontalWallsMemo = new Dictionary<int[], int>();
	//public Dictionary<int[],int> defaultVerticalWallsMemo = new Dictionary<int[], int>();

	public WallType[] wallTypes;

	public GameManager gm;

    public int damagedWalls = 0;

    void StartWallManager()
	{
		PopulateWalls();

		GenerateMapVisual();

	}

	public int HorizontalWall(int x, int z)
    {
        List<int[]> keyList = new List<int[]>(hwallStores.Keys);

        foreach (var key in keyList)
        {
            if (key[0] == x && key[1] == z)
            {
                return hwallStores[key].GetComponent<Wall>().type;
            } } return -1;
    }

    public int VerticalWall(int x, int z)
    {
        List<int[]> keyList = new List<int[]>(vwallStores.Keys);

        foreach (var key in keyList)
        {
            if (key[0] == x && key[1] == z)
            {
                return vwallStores[key].GetComponent<Wall>().type;
            }
        }
        return -1;
    }

    public bool isHorizontalWall(int x,int z)
    {
        List<int[]> keyList = new List<int[]>(hwallStores.Keys);

        foreach (var key in keyList)
        {
            if (key[0] == x && key[1] == z&& hwallStores[key].GetComponent<Wall>().type!=4)
            {
                return true;
            }
        }
        return false;
    }

    public bool isVeritcalWall(int x, int z)
    {
        List<int[]> keyList = new List<int[]>(vwallStores.Keys );

        foreach (var key in keyList)
        {
            if (key[0] == x && key[1] == z && vwallStores[key].GetComponent<Wall>().type != 5)
            {
                return true;
            }
        }
        return false;
    }


	public WallManager(WallType[] wallTypes, GameManager gm, int loadGame)
	{
		this.wallTypes = wallTypes;
		this.gm = gm;
		if (loadGame == 0)//we're creating a new game
		{
			StartWallManager();
		}
		else
		{
			loadWallVisual();
		}
		
	}

	void PopulateWalls()
	{
        if (!StaticInfo.level.Equals("Random"))
        {
            //horizontal
            //outer wall
            defaultHorizontalWalls.Add(new int[] { 1, 1 });
            defaultHorizontalWalls.Add(new int[] { 2, 1 });
            defaultHorizontalWalls.Add(new int[] { 4, 1 });
            defaultHorizontalWalls.Add(new int[] { 5, 1 });
            defaultHorizontalWalls.Add(new int[] { 6, 1 });
            defaultHorizontalWalls.Add(new int[] { 7, 1 });
            defaultHorizontalWalls.Add(new int[] { 8, 1 });
            //----------------------------------------------------
            defaultHorizontalWalls.Add(new int[] { 1, 7 });
            defaultHorizontalWalls.Add(new int[] { 2, 7 });
            defaultHorizontalWalls.Add(new int[] { 3, 7 });
            defaultHorizontalWalls.Add(new int[] { 4, 7 });
            defaultHorizontalWalls.Add(new int[] { 5, 7 });
            defaultHorizontalWalls.Add(new int[] { 6, 7 });
            defaultHorizontalWalls.Add(new int[] { 8, 7 });

            //Specific Horizontal for base
            defaultHorizontalWalls.Add(new int[] { 3, 5 });
            defaultHorizontalWalls.Add(new int[] { 4, 5 });
            defaultHorizontalWalls.Add(new int[] { 5, 5 });
            defaultHorizontalWalls.Add(new int[] { 6, 5 });
            defaultHorizontalWalls.Add(new int[] { 7, 5 });
            defaultHorizontalWalls.Add(new int[] { 1, 3 });
            defaultHorizontalWalls.Add(new int[] { 2, 3 });
            defaultHorizontalWalls.Add(new int[] { 3, 3 });
            defaultHorizontalWalls.Add(new int[] { 5, 3 });
            defaultHorizontalWalls.Add(new int[] { 6, 3 });
            defaultHorizontalWalls.Add(new int[] { 7, 3 });
            defaultHorizontalWalls.Add(new int[] { 8, 3 });
            //---------------------------------------------------
            //vertical
            //outer wall
            defaultVerticalWalls.Add(new int[] { 1, 1 });
            defaultVerticalWalls.Add(new int[] { 1, 2 });
            defaultVerticalWalls.Add(new int[] { 1, 3 });
            defaultVerticalWalls.Add(new int[] { 1, 5 });
            defaultVerticalWalls.Add(new int[] { 1, 6 });
            //---------------------------------------------
            defaultVerticalWalls.Add(new int[] { 9, 1 });
            defaultVerticalWalls.Add(new int[] { 9, 2 });
            defaultVerticalWalls.Add(new int[] { 9, 4 });
            defaultVerticalWalls.Add(new int[] { 9, 5 });
            defaultVerticalWalls.Add(new int[] { 9, 6 });

            //Specific Vertical for base
            defaultVerticalWalls.Add(new int[] { 3, 3 });
            defaultVerticalWalls.Add(new int[] { 4, 5 });
            defaultVerticalWalls.Add(new int[] { 6, 6 });
            defaultVerticalWalls.Add(new int[] { 7, 4 });
            defaultVerticalWalls.Add(new int[] { 6, 2 });
            defaultVerticalWalls.Add(new int[] { 8, 2 });
        }
        else
        {
            Debug.Log("Random board number: " + DoorManager.randomBoard);
            if (DoorManager.randomBoard == 1)
            {
                randomWall1();
            }
            else if (DoorManager.randomBoard == 2)
            {
                randomWall2();
            }
            else if (DoorManager.randomBoard == 3)
            {
                randomWall3();
            }
            else if (DoorManager.randomBoard == 4)
            {
                randomWall4();
            }
            else if (DoorManager.randomBoard == 5)
            {
                randomWall5();
            }
        }        

        /* im so stupid....
		
		defaultHorizontalWallsMemo[new int[] {1, 1}] = 0;
		defaultHorizontalWallsMemo[new int[] {2, 1}] = 0;
		defaultHorizontalWallsMemo[new int[] {4, 1}] = 0;
		defaultHorizontalWallsMemo[new int[] {3, 1}] = 0;
		defaultHorizontalWallsMemo[new int[] {6, 1}] = 0;
		defaultHorizontalWallsMemo[new int[] {7, 1}] = 0;
		defaultHorizontalWallsMemo[new int[] {8, 1}] = 0;
		defaultHorizontalWallsMemo[new int[] {1, 3}] = 0;
		defaultHorizontalWallsMemo[new int[] {2, 3}] = 0;
		defaultHorizontalWallsMemo[new int[] {4, 3}] = 0;
		defaultHorizontalWallsMemo[new int[] {5, 3}] = 0;
		defaultHorizontalWallsMemo[new int[] {4, 4}] = 0;
		defaultHorizontalWallsMemo[new int[] {6, 4}] = 0;
		
		defaultHorizontalWallsMemo[new int[] {7, 4}] = 0;
		defaultHorizontalWallsMemo[new int[] {8, 4}] = 0;
		defaultHorizontalWallsMemo[new int[] {1, 5}] = 0;
		defaultHorizontalWallsMemo[new int[] {2, 5}] = 0;
		defaultHorizontalWallsMemo[new int[] {4, 6}] = 0;
		defaultHorizontalWallsMemo[new int[] {5, 6}] = 0;
		defaultHorizontalWallsMemo[new int[] {1, 7}] = 0;
		defaultHorizontalWallsMemo[new int[] {2, 7}] = 0;
		defaultHorizontalWallsMemo[new int[] {3, 7}] = 0;
		defaultHorizontalWallsMemo[new int[] {4, 7}] = 0;
		defaultHorizontalWallsMemo[new int[] {5, 7}] = 0;
		defaultHorizontalWallsMemo[new int[] {7, 7}] = 0;
		defaultHorizontalWallsMemo[new int[] {8, 7}] = 0;
		//------------------------------------------
		defaultVerticalWallsMemo[new int[] {1, 1}] = 0;
		defaultVerticalWallsMemo[new int[] {1, 2}] = 0;
		defaultVerticalWallsMemo[new int[] {1, 4}] = 0;
		defaultVerticalWallsMemo[new int[] {1, 5}] = 0;
		defaultVerticalWallsMemo[new int[] {1, 6}] = 0;
		defaultVerticalWallsMemo[new int[] {4, 1}] = 0;
		defaultVerticalWallsMemo[new int[] {4, 2}] = 0;
		defaultVerticalWallsMemo[new int[] {4, 4}] = 0;
		defaultVerticalWallsMemo[new int[] {4, 5}] = 0;
		defaultVerticalWallsMemo[new int[] {4, 6}] = 0;
		defaultVerticalWallsMemo[new int[] {6, 5}] = 0;
		defaultVerticalWallsMemo[new int[] {6, 6}] = 0;
		
		defaultVerticalWallsMemo[new int[] {7, 1}] = 0;
		defaultVerticalWallsMemo[new int[] {7, 2}] = 0;
		defaultVerticalWallsMemo[new int[] {7, 3}] = 0;
		defaultVerticalWallsMemo[new int[] {9, 1}] = 0;
		defaultVerticalWallsMemo[new int[] {9, 2}] = 0;
		defaultVerticalWallsMemo[new int[] {9, 4}] = 0;
		defaultVerticalWallsMemo[new int[] {9, 5}] = 0;
		defaultVerticalWallsMemo[new int[] {9, 6}] = 0;*/

    }

    void randomWall1()
    {
        //horizontal
        //outer wall
        defaultHorizontalWalls.Add(new int[] { 1, 1 });
        defaultHorizontalWalls.Add(new int[] { 2, 1 });
        defaultHorizontalWalls.Add(new int[] { 4, 1 });
        defaultHorizontalWalls.Add(new int[] { 3, 1 });
        defaultHorizontalWalls.Add(new int[] { 6, 1 });
        defaultHorizontalWalls.Add(new int[] { 7, 1 });
        defaultHorizontalWalls.Add(new int[] { 8, 1 });
        //----------------------------------------------------
        defaultHorizontalWalls.Add(new int[] { 1, 7 });
        defaultHorizontalWalls.Add(new int[] { 2, 7 });
        defaultHorizontalWalls.Add(new int[] { 3, 7 });
        defaultHorizontalWalls.Add(new int[] { 4, 7 });
        defaultHorizontalWalls.Add(new int[] { 5, 7 });
        defaultHorizontalWalls.Add(new int[] { 7, 7 });
        defaultHorizontalWalls.Add(new int[] { 8, 7 });

        //Specific Horizontal for random1
        defaultHorizontalWalls.Add(new int[] { 1, 4 });
        defaultHorizontalWalls.Add(new int[] { 1, 3 });
        defaultHorizontalWalls.Add(new int[] { 2, 3 });
        defaultHorizontalWalls.Add(new int[] { 3, 4 });
        defaultHorizontalWalls.Add(new int[] { 3, 3 });
        defaultHorizontalWalls.Add(new int[] { 4, 4 });
        defaultHorizontalWalls.Add(new int[] { 4, 3 });
        defaultHorizontalWalls.Add(new int[] { 5, 4 });
        defaultHorizontalWalls.Add(new int[] { 6, 3 });
        defaultHorizontalWalls.Add(new int[] { 7, 3 });
        //---------------------------------------------------
        //vertical
        //outer wall
        defaultVerticalWalls.Add(new int[] { 1, 1 });
        defaultVerticalWalls.Add(new int[] { 1, 2 });
        defaultVerticalWalls.Add(new int[] { 1, 4 });
        defaultVerticalWalls.Add(new int[] { 1, 5 });
        defaultVerticalWalls.Add(new int[] { 1, 6 });
        //---------------------------------------------
        defaultVerticalWalls.Add(new int[] { 9, 1 });
        defaultVerticalWalls.Add(new int[] { 9, 2 });
        defaultVerticalWalls.Add(new int[] { 9, 4 });
        defaultVerticalWalls.Add(new int[] { 9, 5 });
        defaultVerticalWalls.Add(new int[] { 9, 6 });

        //Specific Vertical for random1
        defaultVerticalWalls.Add(new int[] { 4, 6 });
        defaultVerticalWalls.Add(new int[] { 5, 6 });
        defaultVerticalWalls.Add(new int[] { 5, 5 });
        defaultVerticalWalls.Add(new int[] { 5, 4 });
        defaultVerticalWalls.Add(new int[] { 5, 1 });
        defaultVerticalWalls.Add(new int[] { 6, 2 });
        defaultVerticalWalls.Add(new int[] { 6, 1 });
        defaultVerticalWalls.Add(new int[] { 7, 6 });
        defaultVerticalWalls.Add(new int[] { 7, 5 });
        defaultVerticalWalls.Add(new int[] { 7, 4 });
        defaultVerticalWalls.Add(new int[] { 8, 6 });
        defaultVerticalWalls.Add(new int[] { 8, 4 });
        defaultVerticalWalls.Add(new int[] { 8, 3 });
    }

    void randomWall2()
    {
        //horizontal
        //outer wall
        defaultHorizontalWalls.Add(new int[] { 1, 1 });
        defaultHorizontalWalls.Add(new int[] { 2, 1 });
        defaultHorizontalWalls.Add(new int[] { 4, 1 });
        defaultHorizontalWalls.Add(new int[] { 3, 1 });
        defaultHorizontalWalls.Add(new int[] { 6, 1 });
        defaultHorizontalWalls.Add(new int[] { 7, 1 });
        defaultHorizontalWalls.Add(new int[] { 8, 1 });
        //----------------------------------------------------
        defaultHorizontalWalls.Add(new int[] { 1, 7 });
        defaultHorizontalWalls.Add(new int[] { 2, 7 });
        defaultHorizontalWalls.Add(new int[] { 3, 7 });
        defaultHorizontalWalls.Add(new int[] { 4, 7 });
        defaultHorizontalWalls.Add(new int[] { 5, 7 });
        defaultHorizontalWalls.Add(new int[] { 7, 7 });
        defaultHorizontalWalls.Add(new int[] { 8, 7 });

        //Specific Horizontal for random2
        defaultHorizontalWalls.Add(new int[] { 1, 5 });
        defaultHorizontalWalls.Add(new int[] { 2, 5 });
        defaultHorizontalWalls.Add(new int[] { 3, 5 });
        defaultHorizontalWalls.Add(new int[] { 5, 5 });
        defaultHorizontalWalls.Add(new int[] { 7, 5 });
        defaultHorizontalWalls.Add(new int[] { 8, 5 });
        defaultHorizontalWalls.Add(new int[] { 5, 4 });
        defaultHorizontalWalls.Add(new int[] { 7, 4 });
        defaultHorizontalWalls.Add(new int[] { 5, 3 });
        defaultHorizontalWalls.Add(new int[] { 7, 3 });
        defaultHorizontalWalls.Add(new int[] { 6, 2 });
        //---------------------------------------------------
        //vertical
        //outer wall
        defaultVerticalWalls.Add(new int[] { 1, 1 });
        defaultVerticalWalls.Add(new int[] { 1, 2 });
        defaultVerticalWalls.Add(new int[] { 1, 4 });
        defaultVerticalWalls.Add(new int[] { 1, 5 });
        defaultVerticalWalls.Add(new int[] { 1, 6 });
        //---------------------------------------------
        defaultVerticalWalls.Add(new int[] { 9, 1 });
        defaultVerticalWalls.Add(new int[] { 9, 2 });
        defaultVerticalWalls.Add(new int[] { 9, 4 });
        defaultVerticalWalls.Add(new int[] { 9, 5 });
        defaultVerticalWalls.Add(new int[] { 9, 6 });

        //Specific Vertical for random2
        defaultVerticalWalls.Add(new int[] { 5, 6 });
        defaultVerticalWalls.Add(new int[] { 5, 5 });
        defaultVerticalWalls.Add(new int[] { 6, 4 });
        defaultVerticalWalls.Add(new int[] { 7, 4 });
        defaultVerticalWalls.Add(new int[] { 4, 3 });
        defaultVerticalWalls.Add(new int[] { 5, 3 });
        defaultVerticalWalls.Add(new int[] { 8, 3 });
        defaultVerticalWalls.Add(new int[] { 4, 2 });
        defaultVerticalWalls.Add(new int[] { 4, 1 });
        defaultVerticalWalls.Add(new int[] { 6, 1 });
        defaultVerticalWalls.Add(new int[] { 8, 1 });
    }

    void randomWall3()
    {
        defaultHorizontalWalls.Add(new int[] { 1, 1 });

        defaultHorizontalWalls.Add(new int[] { 2, 1 });
        //
        defaultHorizontalWalls.Add(new int[] { 4, 1 });
        defaultHorizontalWalls.Add(new int[] { 3, 1 });
        defaultHorizontalWalls.Add(new int[] { 6, 1 });
        defaultHorizontalWalls.Add(new int[] { 7, 1 });
        defaultHorizontalWalls.Add(new int[] { 8, 1 });

        defaultHorizontalWalls.Add(new int[] { 1, 3 });
        defaultHorizontalWalls.Add(new int[] { 2, 3 });
        //
        defaultHorizontalWalls.Add(new int[] { 4, 3 });
        defaultHorizontalWalls.Add(new int[] { 5, 3 });
        //

        defaultHorizontalWalls.Add(new int[] { 4, 4 });
        //
        defaultHorizontalWalls.Add(new int[] { 6, 4 });
        defaultHorizontalWalls.Add(new int[] { 7, 4 });
        defaultHorizontalWalls.Add(new int[] { 8, 4 });

        defaultHorizontalWalls.Add(new int[] { 1, 5 });
        defaultHorizontalWalls.Add(new int[] { 2, 5 });

        defaultHorizontalWalls.Add(new int[] { 4, 6 });
        defaultHorizontalWalls.Add(new int[] { 5, 6 });

        defaultHorizontalWalls.Add(new int[] { 1, 7 });
        defaultHorizontalWalls.Add(new int[] { 2, 7 });
        defaultHorizontalWalls.Add(new int[] { 3, 7 });
        defaultHorizontalWalls.Add(new int[] { 4, 7 });
        defaultHorizontalWalls.Add(new int[] { 5, 7 });
        //
        defaultHorizontalWalls.Add(new int[] { 7, 7 });
        defaultHorizontalWalls.Add(new int[] { 8, 7 });

        defaultVerticalWalls.Add(new int[] { 1, 1 });
        defaultVerticalWalls.Add(new int[] { 1, 2 });
        defaultVerticalWalls.Add(new int[] { 1, 4 });
        //
        defaultVerticalWalls.Add(new int[] { 1, 5 });
        defaultVerticalWalls.Add(new int[] { 1, 6 });

        defaultVerticalWalls.Add(new int[] { 4, 1 });
        defaultVerticalWalls.Add(new int[] { 4, 2 });
        defaultVerticalWalls.Add(new int[] { 4, 4 });
        defaultVerticalWalls.Add(new int[] { 4, 5 });
        defaultVerticalWalls.Add(new int[] { 4, 6 });

        //
        defaultVerticalWalls.Add(new int[] { 6, 5 });
        defaultVerticalWalls.Add(new int[] { 6, 6 });

        defaultVerticalWalls.Add(new int[] { 7, 1 });
        defaultVerticalWalls.Add(new int[] { 7, 2 });
        defaultVerticalWalls.Add(new int[] { 7, 3 });

        defaultVerticalWalls.Add(new int[] { 9, 1 });
        defaultVerticalWalls.Add(new int[] { 9, 2 });
        //
        defaultVerticalWalls.Add(new int[] { 9, 4 });
        defaultVerticalWalls.Add(new int[] { 9, 5 });
        defaultVerticalWalls.Add(new int[] { 9, 6 });
    }

    void randomWall4()
    {
        //horizontal
        //outer wall
        defaultHorizontalWalls.Add(new int[] { 1, 1 });
        defaultHorizontalWalls.Add(new int[] { 2, 1 });
        defaultHorizontalWalls.Add(new int[] { 4, 1 });
        defaultHorizontalWalls.Add(new int[] { 3, 1 });
        defaultHorizontalWalls.Add(new int[] { 6, 1 });
        defaultHorizontalWalls.Add(new int[] { 7, 1 });
        defaultHorizontalWalls.Add(new int[] { 8, 1 });
        //----------------------------------------------------
        defaultHorizontalWalls.Add(new int[] { 1, 7 });
        defaultHorizontalWalls.Add(new int[] { 2, 7 });
        defaultHorizontalWalls.Add(new int[] { 3, 7 });
        defaultHorizontalWalls.Add(new int[] { 4, 7 });
        defaultHorizontalWalls.Add(new int[] { 5, 7 });
        defaultHorizontalWalls.Add(new int[] { 7, 7 });
        defaultHorizontalWalls.Add(new int[] { 8, 7 });

        //Specific Horizontal for random4
        defaultHorizontalWalls.Add(new int[] { 5, 6 });
        defaultHorizontalWalls.Add(new int[] { 7, 6 });
        defaultHorizontalWalls.Add(new int[] { 1, 5 });
        defaultHorizontalWalls.Add(new int[] { 2, 4 });
        defaultHorizontalWalls.Add(new int[] { 3, 4 });
        defaultHorizontalWalls.Add(new int[] { 5, 4 });
        defaultHorizontalWalls.Add(new int[] { 6, 4 });
        defaultHorizontalWalls.Add(new int[] { 7, 4 });
        defaultHorizontalWalls.Add(new int[] { 7, 3 });
        defaultHorizontalWalls.Add(new int[] { 8, 3 });
        defaultHorizontalWalls.Add(new int[] { 4, 2 });
        defaultHorizontalWalls.Add(new int[] { 6, 2 });
        //---------------------------------------------------
        //vertical
        //outer wall
        defaultVerticalWalls.Add(new int[] { 1, 1 });
        defaultVerticalWalls.Add(new int[] { 1, 2 });
        defaultVerticalWalls.Add(new int[] { 1, 4 });
        defaultVerticalWalls.Add(new int[] { 1, 5 });
        defaultVerticalWalls.Add(new int[] { 1, 6 });
        //---------------------------------------------
        defaultVerticalWalls.Add(new int[] { 9, 1 });
        defaultVerticalWalls.Add(new int[] { 9, 2 });
        defaultVerticalWalls.Add(new int[] { 9, 4 });
        defaultVerticalWalls.Add(new int[] { 9, 5 });
        defaultVerticalWalls.Add(new int[] { 9, 6 });

        //Specific Vertical for random4
        defaultVerticalWalls.Add(new int[] { 2, 4 });
        defaultVerticalWalls.Add(new int[] { 3, 3 });
        defaultVerticalWalls.Add(new int[] { 4, 2 });
        defaultVerticalWalls.Add(new int[] { 5, 4 });
        defaultVerticalWalls.Add(new int[] { 5, 1 });
        defaultVerticalWalls.Add(new int[] { 6, 6 });
        defaultVerticalWalls.Add(new int[] { 6, 1 });
        defaultVerticalWalls.Add(new int[] { 7, 6 });
        defaultVerticalWalls.Add(new int[] { 8, 5 });
        defaultVerticalWalls.Add(new int[] { 8, 4 });
    }

    void randomWall5()
    {
        //horizontal
        //outer wall
        defaultHorizontalWalls.Add(new int[] { 1, 1 });
        defaultHorizontalWalls.Add(new int[] { 2, 1 });
        defaultHorizontalWalls.Add(new int[] { 4, 1 });
        defaultHorizontalWalls.Add(new int[] { 3, 1 });
        defaultHorizontalWalls.Add(new int[] { 6, 1 });
        defaultHorizontalWalls.Add(new int[] { 7, 1 });
        defaultHorizontalWalls.Add(new int[] { 8, 1 });
        //----------------------------------------------------
        defaultHorizontalWalls.Add(new int[] { 1, 7 });
        defaultHorizontalWalls.Add(new int[] { 2, 7 });
        defaultHorizontalWalls.Add(new int[] { 3, 7 });
        defaultHorizontalWalls.Add(new int[] { 4, 7 });
        defaultHorizontalWalls.Add(new int[] { 5, 7 });
        defaultHorizontalWalls.Add(new int[] { 7, 7 });
        defaultHorizontalWalls.Add(new int[] { 8, 7 });

        //Specific Horizontal for random5
        defaultHorizontalWalls.Add(new int[] { 2, 6 });
        defaultHorizontalWalls.Add(new int[] { 1, 4 });
        defaultHorizontalWalls.Add(new int[] { 2, 4 });
        defaultHorizontalWalls.Add(new int[] { 4, 4 });
        defaultHorizontalWalls.Add(new int[] { 5, 4 });
        defaultHorizontalWalls.Add(new int[] { 6, 4 });
        defaultHorizontalWalls.Add(new int[] { 8, 2 });
        //---------------------------------------------------
        //vertical
        //outer wall
        defaultVerticalWalls.Add(new int[] { 1, 1 });
        defaultVerticalWalls.Add(new int[] { 1, 2 });
        defaultVerticalWalls.Add(new int[] { 1, 4 });
        defaultVerticalWalls.Add(new int[] { 1, 5 });
        defaultVerticalWalls.Add(new int[] { 1, 6 });
        //---------------------------------------------
        defaultVerticalWalls.Add(new int[] { 9, 1 });
        defaultVerticalWalls.Add(new int[] { 9, 2 });
        defaultVerticalWalls.Add(new int[] { 9, 4 });
        defaultVerticalWalls.Add(new int[] { 9, 5 });
        defaultVerticalWalls.Add(new int[] { 9, 6 });

        //Specific Vertical for random5
        defaultVerticalWalls.Add(new int[] { 2, 6 });
        defaultVerticalWalls.Add(new int[] { 3, 3 });
        defaultVerticalWalls.Add(new int[] { 3, 2 });
        defaultVerticalWalls.Add(new int[] { 5, 6 });
        defaultVerticalWalls.Add(new int[] { 5, 5 });
        defaultVerticalWalls.Add(new int[] { 5, 4 });
        defaultVerticalWalls.Add(new int[] { 6, 3 });
        defaultVerticalWalls.Add(new int[] { 6, 1 });
        defaultVerticalWalls.Add(new int[] { 7, 1 });
        defaultVerticalWalls.Add(new int[] { 8, 6 });
        defaultVerticalWalls.Add(new int[] { 8, 4 });
        defaultVerticalWalls.Add(new int[] { 8, 5 });
    }

    public void BreakWall(int x, int z, int type, int horizontal, bool explosionIsBreaking)
	{
		Debug.Log("Trying to break the wall (" + x + "," + z +")");

		/*	We only call chopWall() if we see that this wall ISNT being damaged from an explosion.
		 *	This is done so as to avoid explosions reducing AP (from damaging walls)
		 */
		bool can_break = (explosionIsBreaking) ? explosionIsBreaking : gm.fireman.chopWall();

		if (can_break)
		{
			if (horizontal == 1) //we're breaking a hwall
			{
				List<int[]> keyList = new List<int[]>(hwallStores.Keys);

				foreach (var key in keyList)
				{
					if (key[0] == x && key[1] == z)
					{
						Debug.Log("Breaking the wall");

						GameObject old = hwallStores[key];
						//Destroy(old);
						gm.DestroyObject(old);
						damagedWalls++;		// Increment the GUI counter to represent # of damaged walls
                        Debug.Log(damagedWalls);
                        gm.displayStats();
                        if (damagedWalls >= 24)
                        {
                            Debug.Log("24 damaged markers");
                            gm.defeat();
                        }

                        WallType wt = wallTypes[type];
						//GameObject objectW = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x *6, 0, z *6-2), Quaternion.identity);
						GameObject objectW = gm.instantiateObject(wt.wallVisualPrefab, new Vector3(x *6, 2, z *6 - 3), Quaternion.identity);

						Wall w = objectW.GetComponent<Wall>();
						w.x = x *6;
						w.z = z *6;
						w.type = type;
						w.wallMap = this;

						/*
                        int[] k = new int[2];
                        k[0] = x;
                        k[1] = z;
						*/
						hwallStores[key] = objectW;
					}
				}
			}
			else  // we're breaking a vwall
			{
				List<int[]> keyList = new List<int[]>(vwallStores.Keys);

				foreach (var key in keyList)
				{
					if (key[0] == x && key[1] == z)
					{

						Debug.Log("Breaking the wall");

						GameObject old = vwallStores[key];
						//Destroy(old);
						gm.DestroyObject(old);
						damagedWalls++;     // Increment the GUI counter to represent # of damaged walls
                        gm.displayStats();
                        if (damagedWalls >= 24)
                        {
                            Debug.Log("24 damaged markers");
                            gm.defeat();
                        }

                        WallType wt = wallTypes[type];
						//GameObject objectW = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x *6-2, 0, z *6), Quaternion.Euler(0,90,0));
						GameObject objectW = gm.instantiateObject(wt.wallVisualPrefab, new Vector3(x *6 - 3, 2, z *6), Quaternion.Euler(0, 90, 0));

						Wall w = objectW.GetComponent<Wall>();
						w.x = x *6;
						w.z = z *6;
						w.type = type;
						w.wallMap = this;

						/*
                        int[] k = new int[2];
                        k[0] = x;
                        k[1] = z;
						*/
						vwallStores[key] = objectW;

					}
				}
			}
		}
	}

	void GenerateMapVisual()
	{

		foreach (var hWall in defaultHorizontalWalls)
		{
			WallType wt = wallTypes[0];
			//GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(hWall[0]*5, 0, hWall[1]*5-2), Quaternion.identity );
			GameObject go = gm.instantiateObject(wt.wallVisualPrefab, new Vector3(hWall[0] *6, 2, hWall[1] *6 - 3), Quaternion.identity);

			Wall w = go.GetComponent<Wall>();
			//Debug.Log(w);
			// Assign the variables as needed
			w.x = hWall[0] *6;
			w.z = hWall[1] *6;
			w.wallMap = this;
			w.type = 0;

            //hwallStores[hWall] = go;
            hwallStores.Add(hWall, go);


		}

		foreach (var vWall in defaultVerticalWalls)
		{
			WallType wt = wallTypes[1];
			Debug.Log(wt);
			//GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(vWall[0]*5-2, 0, vWall[1]*5), Quaternion.Euler(0,90,0) );
			GameObject go = gm.instantiateObject(wt.wallVisualPrefab, new Vector3(vWall[0] *6 - 3, 2, vWall[1] *6), Quaternion.Euler(0, 90, 0));

			Wall w = go.GetComponent<Wall>();
			//Debug.Log(w);
			// Assign the variables as needed
			w.x = vWall[0] *6;
			w.z = vWall[1] *6;
			w.wallMap = this;
			w.type = 1;

            //vwallStores[vWall] = go;
            vwallStores.Add(vWall, go);

		}

	}

	public void loadWallVisual()
	{
		Dictionary<int[], int> h = StaticInfo.hWallMemo;
		Dictionary<int[], int> v = StaticInfo.vWallMemo;

		foreach (KeyValuePair<int[], int> entry in h)
		{
			var hWall = entry.Key;
			var type = entry.Value;
			
			WallType wt = wallTypes[type];
			//GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(hWall[0]*5, 0, hWall[1]*5-2), Quaternion.identity );
			GameObject go = gm.instantiateObject(wt.wallVisualPrefab, new Vector3(hWall[0] *6, 2, hWall[1] *6 - 3), Quaternion.identity);

			Wall w = go.GetComponent<Wall>();
			//Debug.Log(w);
			// Assign the variables as needed
			w.x = hWall[0] *6;
			w.z = hWall[1] *6;
			w.wallMap = this;
			w.type = type;

			//hwallStores[hWall] = go;
			hwallStores.Add(hWall, go);
		}
		
		foreach (KeyValuePair<int[], int> entry in v)
		{
			var vWall = entry.Key;
			var type = entry.Value;
			
			WallType wt = wallTypes[type];
			//Debug.Log(wt);
			//GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(vWall[0]*5-2, 0, vWall[1]*5), Quaternion.Euler(0,90,0) );
			GameObject go = gm.instantiateObject(wt.wallVisualPrefab, new Vector3(vWall[0] *6 - 3, 2, vWall[1] *6), Quaternion.Euler(0, 90, 0));

			Wall w = go.GetComponent<Wall>();
			//Debug.Log(w);
			// Assign the variables as needed
			w.x = vWall[0] *6;
			w.z = vWall[1] *6;
			w.wallMap = this;
			w.type = type;

			//vwallStores[vWall] = go;
			vwallStores.Add(vWall, go);
		}
	}


	// Check if there's a horizontal wall on (x, z) and its not destroyed (called by FireManager.cs)
	public bool checkIfHWall(int x, int z)
	{
		List<int[]> keyList = new List<int[]>(hwallStores.Keys);

		foreach (var key in keyList)
		{   // We check to make sure its the right wall and that its not destroyed
			if (key[0] == x && key[1] == z && hwallStores[key].HasComponent<Wall>())
			{
				//Debug.Log("HasComponent TRUE for (x, z): " + x + "," + z);

				// This is done seperately to avoid segfaults
				if (hwallStores[key].GetComponent<Wall>().type != 4)
				{
					//Debug.Log("Type != 4 for (x, z): " + x + "," + z);
					return true;
				}
			}
		}

		//Debug.Log("Returned FALSE for (x,z): " + x + "," + z);
		return false;
	}

	// Check if there's a vertical wall on (x, z) and its not destroyed (called by FireManager.cs)
	public bool checkIfVWall(int x, int z)
	{
		List<int[]> keyList = new List<int[]>(vwallStores.Keys);

		foreach (var key in keyList)
		{   // We check to make sure its the right wall and that its not destroyed
			if (key[0] == x && key[1] == z && vwallStores[key].HasComponent<Wall>())
			{
				// This is done seperately to avoid segfaults
				if (vwallStores[key].GetComponent<Wall>().type != 5)
				{
					//Debug.Log("Type != 5 for (x, z): " + x + "," + z);
					return true;
				}
			}
		}

		return false;
	}
	public bool checkIfHWall_dog(int x, int z)
	{
		List<int[]> keyList = new List<int[]>(hwallStores.Keys);

		foreach (var key in keyList)
		{   // We check to make sure its the right wall and that its not destroyed
			if (key[0] == x && key[1] == z && hwallStores[key].HasComponent<Wall>())
			{
				//Debug.Log("HasComponent TRUE for (x, z): " + x + "," + z);

				// This is done seperately to avoid segfaults
				if (hwallStores[key].GetComponent<Wall>().type ==0)
				{
					//Debug.Log("Type != 4 for (x, z): " + x + "," + z);
					return true;
				}
			}
		}

		//Debug.Log("Returned FALSE for (x,z): " + x + "," + z);
		return false;
	}

	// Check if there's a vertical wall on (x, z) and its not destroyed (called by FireManager.cs)
	public bool checkIfVWall_dog(int x, int z)
	{
		List<int[]> keyList = new List<int[]>(vwallStores.Keys);

		foreach (var key in keyList)
		{   // We check to make sure its the right wall and that its not destroyed
			if (key[0] == x && key[1] == z && vwallStores[key].HasComponent<Wall>())
			{
				// This is done seperately to avoid segfaults
				if (vwallStores[key].GetComponent<Wall>().type ==1)
				{
					//Debug.Log("Type != 5 for (x, z): " + x + "," + z);
					return true;
				}
			}
		}

		return false;
    }

    

}
