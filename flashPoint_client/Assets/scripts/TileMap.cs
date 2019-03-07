using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class TileMap : MonoBehaviour {

   public GameObject selectedUnit;

	// Dimensions of our map
	readonly int mapSizeX = 10;
	readonly int mapSizeZ = 8;

	// The player (only 1 for now)
	//public Unit selectedUnit;
	
	public Dictionary<int[],GameObject> tileStores = new Dictionary<int[], GameObject>();


    public string[] strings;
	// Array of possible tiles:
	public TileType[] tileTypes;

	// Array populated by 
	public int[,] tiles;


	void Start() {

		// Generate the TileTypes and ClickableTiles
		GenerateMapData();
		// Display them in the game world
		GenerateMapVisual();
	}

	// Populate the data structure
	void GenerateMapData() {
		// Allocate our map tiles
		tiles = new int[mapSizeX, mapSizeZ];
		
		// Initialize our map tiles to be normal or fire
		for(int x = 0; x < mapSizeX; x++) {
			for(int z = 0; z < mapSizeZ; z++) {
				tiles[x,z] = 0;
			}
		}
	}

	// Use GameObject.Instantiate to 'spawn' the TileType objects into the world
	void GenerateMapVisual() {
		for(int x=0; x < mapSizeX; x++) {
			for(int z=0; z < mapSizeZ; z++) {
				TileType tt = tileTypes[ tiles[x,z] ];
				GameObject go = (GameObject) Instantiate( tt.tileVisualPrefab, new Vector3(x*5, 0, z*5), Quaternion.identity );

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
			}
		}
	}

	public void buildNewTile(int x, int z, int type)
	{
		Debug.Log(tileStores.Keys);
		
		List<int[]> keyList = new List<int[]>(tileStores.Keys);

		foreach (var key in keyList)
		{
			if (key[0] == x && key[1] == z)
			{
				GameObject old = tileStores[key];
				Destroy(old);
		
				Debug.Log("Building new tile");
				tiles[x, z] = type;
				TileType tt = tileTypes[ tiles[x,z] ];
				GameObject go = (GameObject) Instantiate( tt.tileVisualPrefab, new Vector3(x*5, 0, z*5), Quaternion.identity );

				// Connect a ClickableTile to each TileType
				ClickableTile ct = go.GetComponent<ClickableTile>();
				// Assign the variables as needed
				ct.tileX = x*5;
				ct.tileZ = z*5;
				ct.map = this;
				ct.type = tt;

				tileStores[key] = go;
			}
		}
		
	}
	
    public void MoveSelectedUnitTo(int x, int z) {
        selectedUnit.transform.position = new Vector3(x, 0.2f, z);
    }
}
