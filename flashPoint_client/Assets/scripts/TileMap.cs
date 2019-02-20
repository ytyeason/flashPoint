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


    public string[] strings;
	// Array of possible tiles:
	public TileType[] tileTypes;

	// Array populated by 
	int[,] tiles;


	void Start() {
        //Allocate our map tiles
        tiles = new int[mapSizeX, mapSizeZ];

        //Initialize our map tiles
        for (int x = 0; x < mapSizeX; x++) {
            for (int z = 0; z < mapSizeZ; z++) {
                tiles[x, z] = 0;
            }
        }

		// Setup the selectedUnit's variables by attaching its references:
	/*	selectedUnit.tileX = (int) selectedUnit.transform.position.x;
		selectedUnit.tileZ = (int) selectedUnit.transform.position.z;
		selectedUnit.map = this;

		//Debug.Log("Start:");
*/
		// Generate the TileTypes and ClickableTiles
		GenerateMapData();
		// Display them in the game world
		GenerateMapVisual();
	}

	// Move 'selectedUnit' (and the camera with it) - using the 'w', 'a', 's' & 'd' keys
/*	void Update() {
		if (Input.GetKeyDown("w")) {
			selectedUnit.tileZ += 1;
			print("w");
		}
		else if (Input.GetKeyDown("a")) {
			selectedUnit.tileX -= 1;
			print("a");
		}
		else if (Input.GetKeyDown("s")) {
			selectedUnit.tileZ -= 1;
			print("s");
		}
		else if (Input.GetKeyDown("d")) {
			selectedUnit.tileX += 1;
			print("d");
		}
*/
		// Move selectedUnit to the new position that the keys dictate
	/*selectedUnit.transform.position = new Vector3( (float) selectedUnit.tileX, 0.2f, (float) selectedUnit.tileZ);
	}*/

	// Populate the data structure
	void GenerateMapData() {
		// Allocate our map tiles
		tiles = new int[mapSizeX, mapSizeZ];
		
		// Initialize our map tiles to be normal
		for(int x = 0; x < mapSizeX; x++) {
			for(int z = 0; z < mapSizeZ; z++) {
				tiles[x,z] = 0;
				//Debug.Log("Just made: x: " + x + ", y: " + y);
			}
		}

		//Debug.Log("Just finished");
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
			}
		}
	}
    public void MoveSelectedUnitTo(int x, int z) {
        selectedUnit.transform.position = new Vector3(x, 0.2f, z);
    }
}
