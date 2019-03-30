using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using System.Text.RegularExpressions;
using SocketIO;
using System;


public class TileMap  {

   //public Fireman selectedUnit2;
	public Fireman selectedUnit;
	

	// Dimensions of our map
	readonly int mapSizeX = 10;
	readonly int mapSizeZ = 8;

	// The player (only 1 for now)
	//public Unit selectedUnit;
	
	public Dictionary<int[],GameObject> tileStores = new Dictionary<int[], GameObject>();

    //public Dictionary<int[], ClickableTile> clickTileStores = new Dictionary<int[], ClickableTile>();
    public Dictionary<String, GameObject> firemanSores = new Dictionary<string, GameObject>();

	public string[] strings;
	// Array of possible tiles:
	public TileType[] tileTypes;
	public VehicleType[] vehicleTypes;
	public GameManager gm;

	// Array populated by 
	public int[,] tiles;

    public GameObject gmo;
	public GameObject goo;
	public GameObject goo1;
	public GameObject goo2; 



	void StartTileMap() {

		// Generate the TileTypes and ClickableTiles
		GenerateMapData();
		// Display them in the game world
		GenerateMapVisual();
		//goo  = gm.instantiateObject(selectedUnit.s, new Vector3(5, 0, 5), Quaternion.identity);
		//goo1 = gm.instantiateObject(selectedUnit.s, new Vector3(5, 0, 15), Quaternion.identity);
		//goo2 = gm.instantiateObject(selectedUnit.s, new Vector3(40, 0, 30), Quaternion.identity);

		

	}

	public TileMap(TileType[] tileTypes, GameManager gm, Fireman selectedUnit)
	{
		this.tileTypes = tileTypes;
		this.gm = gm;
		this.selectedUnit = selectedUnit;
		
		StartTileMap();
	}

	// Populate the data structure
	void GenerateMapData() {
		// Allocate our map tiles
		tiles = new int[mapSizeX, mapSizeZ];
		
		// Initialize our map tiles to be normal or fire
		for(int x = 0; x < mapSizeX; x++) {
			for(int z = 0; z < mapSizeZ; z++) {
				// Family fire setup:
				if (x == 2 && z == 5) tiles[x, z] = 2;
				else if (x == 3 && z == 5) tiles[x, z] = 2;
				else if (x == 2 && z == 4) tiles[x, z] = 2;
				else if (x == 3 && z == 4) tiles[x, z] = 2;
				else if (x == 4 && z == 4) tiles[x, z] = 2;
				else if (x == 5 && z == 4) tiles[x, z] = 2;
				else if (x == 4 && z == 3) tiles[x, z] = 2;
				else if (x == 6 && z == 2) tiles[x, z] = 2;
				else if (x == 6 && z == 1) tiles[x, z] = 2;
				else if (x == 7 && z == 2) tiles[x, z] = 2;
				// parking spots
				else if (x == 7 && z == 7) tiles[x, z] = 3;
				else if (x == 5 && z == 7) tiles[x, z] = 4;
				else if (x == 0 && z == 3) tiles[x, z] = 4;
				else if (x == 0 && z == 5) tiles[x, z] = 3;
				else if (x == 9 && z == 2) tiles[x, z] = 3;
				else if (x == 9 && z == 4) tiles[x, z] = 4;
				else if (x == 2 && z == 0) tiles[x, z] = 3;
				else if (x == 4 && z == 0) tiles[x, z] = 4;
				/* Make Smoke tiles for Testing
				else if (x == mapSizeX - 2 && z == mapSizeZ - 1) tiles[x, z] = 0;
				else if (x == mapSizeX - 3 && z == mapSizeZ - 1) tiles[x, z] = 0;
				else if (x == mapSizeX - 4 && z == mapSizeZ - 1) tiles[x, z] = 0;
				else if (x == 0 && z == 0) tiles[x, z] = 0;
				
				else if (x == 1 && z == 0) tiles[x, z] = 2;
				else if (x == 1 && z == mapSizeZ - 1) tiles[x, z] = 2;
				else if (x == 0 && z == 1) tiles[x, z] = 2;
				else if (x == mapSizeX - 1 && z == 1) tiles[x, z] = 2;
				//*/
				else //*/ 
					tiles[x, z] = 0;		// 2 -> code for Fire
			}
		}

	}

	// Use GameObject.Instantiate to 'spawn' the TileType objects into the world
	void GenerateMapVisual() {
		for(int x=0; x < mapSizeX; x++) {
			for(int z=0; z < mapSizeZ; z++) {
				TileType tt = tileTypes[ tiles[x,z] ];
				//GameObject go = (GameObject) Instantiate( tt.tileVisualPrefab, new Vector3(x*5, 0, z*5), Quaternion.identity );
				GameObject go = gm.instantiateObject(tt.tileVisualPrefab, new Vector3(x * 5, 0, z * 5), Quaternion.identity);
				
				// Connect a ClickableTile to each TileType
				ClickableTile ct = go.GetComponent<ClickableTile>();
				// Assign the variables as needed
				ct.tileX = x*5;
                ct.tileZ = z*5;
				ct.map = this;
				ct.type = tt;
				
				int[] p = new int[2];
				p[0] = x;
				p[1] = z;
				tileStores[p] = go;
				//clickTileStores[p] = ct;
			}
		}
	}

	public void GenerateFiremanVisual(Dictionary<String, JSONObject> players)
	{
        //Debug.Log("in GenerateFiremanVisual");
        List<String> names = new List<string>(players.Keys);
        foreach(var name in names)
        {
            //Debug.Log("ppppppppppppppp: "+name);
            if (!name.Equals(StaticInfo.name))
            {
                var location = players[name]["Location"].ToString();
                location = location.Substring(1, location.Length - 2);
                var cord = location.Split(',');
                int x = Convert.ToInt32(cord[0]);
                int z = Convert.ToInt32(cord[1]);
                GameObject go = gm.instantiateObject(selectedUnit.s, new Vector3(x, 0, z), Quaternion.identity);
                firemanSores[name] = go;
                if (x == 5)					// TODO: Why is this here?
                {
                    gmo = go;
                }
            }
        }
        firemanSores[StaticInfo.name] = selectedUnit.s;
        selectedUnit.move(selectedUnit.currentX, selectedUnit.currentZ, gmo);
    }

    public void UpdateFiremanVisual(Dictionary<String, JSONObject> p)
    {
        Debug.Log("In update fireman visual");
        List<String> names = new List<string>(p.Keys);
        foreach (var name in names)
        {
            Debug.Log("updating fireman : " + name);

            var location = p[name]["Location"].ToString();
            location = location.Substring(1, location.Length - 2);
            var cord = location.Split(',');
            int x = Convert.ToInt32(cord[0]);
            int z = Convert.ToInt32(cord[1]);


            if (firemanSores.ContainsKey(name))
            {
                var f = firemanSores[name];
                f.transform.position = new Vector3(x, 0.2f, z);
                firemanSores[name] = f;
            }
            else
            {
                Debug.Log("Register new fireman in firemanStore");
                GameObject go = gm.instantiateObject(selectedUnit.s, new Vector3(x, 0, z), Quaternion.identity);
                firemanSores[name] = go;
            }

           

        }
    }

    public void buildNewTile(int x, int z, int type)
	{
        //Debug.Log(tileStores.Keys);
        //Debug.Log("Building new tile");
        //Debug.Log(x);
        //Debug.Log(z);
        //Debug.Log(type);

        List<int[]> keyList = new List<int[]>(tileStores.Keys);

		foreach (var key in keyList)
		{
			if (key[0] == x && key[1] == z)
			{
				GameObject old = tileStores[key];
				//Destroy(old);
				gm.DestroyObject(old);
		
				//Debug.Log("Building new tile");
				tiles[x, z] = type;
				TileType tt = tileTypes[ tiles[x,z] ];
				//GameObject go = (GameObject) Instantiate( tt.tileVisualPrefab, new Vector3(x*5, 0, z*5), Quaternion.identity );
				GameObject go = gm.instantiateObject(tt.tileVisualPrefab, new Vector3(x*5, 0, z*5), Quaternion.identity );

				// Connect a ClickableTile to each TileType
				ClickableTile ct = go.GetComponent<ClickableTile>();
				// Assign the variables as needed
				ct.tileX = x*5;
				ct.tileZ = z*5;
				ct.map = this;
				ct.type = tt;	// Change type to reflect new state
				
				//Debug.Log("(DEBUG) TileMap.buildNewTile(" + x + ", " + z + ")'s spaceState is: " + ct.spaceState);

				// Store appropriate GameObjects into their respective dictionaries
				tileStores[key] = go;
				//clickTileStores[key] = ct;
			}
		}


		
	}
	
    public void MoveSelectedUnitTo(int x, int z, int in_status ) {
		//selectedUnit2.transform.position = new Vector3(x, 0.2f, z);

		//Debug.Log("Running TileMap.MoveSelectedUnitTo(" + x + ", " + z + ")");
		selectedUnit.tryMove(x, z, in_status, goo);
	}
}
