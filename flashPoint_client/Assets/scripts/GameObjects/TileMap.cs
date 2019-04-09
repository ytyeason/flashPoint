using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using System.Text.RegularExpressions;
using SocketIO;
using System;


[Serializable]
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
	//public VehicleType[] vehicleTypes;
	public GameManager gm;
	public Engine engine;
	public Ambulance ambulance;

	// Array populated by 
	public int[,] tiles;
	public VicinityTile[,] VeteranVicinity;	// Used only for Veteran bookkeeping

    public GameObject gmo;
	public GameObject goo;
	public GameObject goo1;
	public GameObject goo2; 



	public void StartTileMap(int loadGame) {

		if (loadGame == 0)
		{
			GenerateMapData();

			GenerateMapVisual();
		}
		else
		{
			tiles = StaticInfo.tiles;
			
			GenerateMapVisual();	
		}
	}

	public TileMap(TileType[] tileTypes, GameManager gm, Fireman selectedUnit, Engine enG, Ambulance amB, int loadGame)
	{
		this.tileTypes = tileTypes;
		this.gm = gm;
		this.selectedUnit = selectedUnit;
		this.engine = enG;
		this.ambulance = amB;
		
		StartTileMap(loadGame);
	}

	// Populate the data structure
	void GenerateMapData() {
		// Allocate our map tiles
		tiles = new int[mapSizeX, mapSizeZ];
		
		// Initialize our map tiles to be normal or fire
		for(int x = 0; x < mapSizeX; x++) {
			for(int z = 0; z < mapSizeZ; z++) {
				// Family fire setup:
				// if (x == 2 && z == 5) tiles[x, z] = 2;
				// else if (x == 3 && z == 5) tiles[x, z] = 2;
				// else if (x == 2 && z == 4) tiles[x, z] = 2;
				// else if (x == 3 && z == 4) tiles[x, z] = 2;
				// else if (x == 4 && z == 4) tiles[x, z] = 2;
				// else if (x == 5 && z == 4) tiles[x, z] = 2;
				// else if (x == 4 && z == 3) tiles[x, z] = 2;
				// else if (x == 6 && z == 2) tiles[x, z] = 2;
				// else if (x == 6 && z == 1) tiles[x, z] = 2;
				// else if (x == 7 && z == 2) tiles[x, z] = 2;
				// parking spots
				if (x == 7 && z == 7) tiles[x, z] = 3;
				else if (x == 8 && z == 7) tiles[x, z] = 3;
				else if (x == 5 && z == 7) tiles[x, z] = 4;
				else if (x == 6 && z == 7) tiles[x, z] = 4;
				else if (x == 0 && z == 3) tiles[x, z] = 4;
				else if (x == 0 && z == 4) tiles[x, z] = 4;
				else if (x == 0 && z == 5) tiles[x, z] = 3;
				else if (x == 0 && z == 6) tiles[x, z] = 3;
				else if (x == 9 && z == 2) tiles[x, z] = 3;
				else if (x == 9 && z == 1) tiles[x, z] = 3;
				else if (x == 9 && z == 4) tiles[x, z] = 4;
				else if (x == 9 && z == 3) tiles[x, z] = 4;
				else if (x == 2 && z == 0) tiles[x, z] = 3;
				else if (x == 1 && z == 0) tiles[x, z] = 3;
				else if (x == 3 && z == 0) tiles[x, z] = 4;
				else if (x == 4 && z == 0) tiles[x, z] = 4;
				/* Make Smoke tiles for Testing
				else if (x == mapSizeX - 2 && z == mapSizeZ - 1) tiles[x, z] = 0;
				else if (x == mapSizeX - 3 && z == mapSizeZ - 1) tiles[x, z] = 0;
				else if (x == mapSizeX - 4 && z == mapSizeZ - 1) tiles[x, z] = 0;
				else if (x == 0 && z == 0) tiles[x, z] = 0;
				//
				else if (x == 1 && z == 0) tiles[x, z] = 2;
				else if (x == 1 && z == mapSizeZ - 1) tiles[x, z] = 2;
				else if (x == 0 && z == 1) tiles[x, z] = 2;
				else if (x == mapSizeX - 1 && z == 1) tiles[x, z] = 2;
				*/
				// else if (x == 1 && z == 3) tiles[x, z] = 1;
				//*/
				else
					tiles[x, z] = 0;		// 2 -> code for Fire
			}
		}

	}

	// Use GameObject.Instantiate to 'spawn' the TileType objects into the world
	public void GenerateMapVisual() {
		for(int x=0; x < mapSizeX; x++) {
			for(int z=0; z < mapSizeZ; z++) {
				if(tiles[x,z]==3||tiles[x,z]==4)
				{
					TileType tt = tileTypes[ tiles[x,z] ];
					//GameObject go = (GameObject) Instantiate( tt.tileVisualPrefab, new Vector3(x*5, 0, z*5), Quaternion.identity );
					GameObject go = gm.instantiateObject(tt.tileVisualPrefab, new Vector3(x * 6, -2, z * 6), Quaternion.identity);
					
					// Connect a ClickableTile to each TileType
					ClickableTile ct = go.GetComponent<ClickableTile>();
					// Assign the variables as needed
					ct.tileX = x*6;
	                ct.tileZ = z*6;
					ct.map = this;
					ct.type = tt;
					
					int[] p = new int[2];
					p[0] = x;
					p[1] = z;
					tileStores[p] = go;
					//clickTileStores[p] = ct;
				}
				else
				{
					TileType tt = tileTypes[ tiles[x,z] ];
					//GameObject go = (GameObject) Instantiate( tt.tileVisualPrefab, new Vector3(x*5, 0, z*5), Quaternion.identity );
					GameObject go = gm.instantiateObject(tt.tileVisualPrefab, new Vector3(x * 6, 0, z * 6), Quaternion.identity);
					
					// Connect a ClickableTile to each TileType
					ClickableTile ct = go.GetComponent<ClickableTile>();
					// Assign the variables as needed
					ct.tileX = x*6;
	                ct.tileZ = z*6;
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
                GameObject go = gm.instantiateObject(selectedUnit.s, new Vector3(x*6, 0, z*6), Quaternion.identity);
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
                GameObject go = gm.instantiateObject(selectedUnit.s, new Vector3(x*6, 0, z*6), Quaternion.identity);
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
				GameObject go = gm.instantiateObject(tt.tileVisualPrefab, new Vector3(x*6, 0, z*6), Quaternion.identity );

				// Connect a ClickableTile to each TileType
				ClickableTile ct = go.GetComponent<ClickableTile>();
				// Assign the variables as needed
				ct.tileX = x*6;
				ct.tileZ = z*6;
				ct.map = this;
				ct.type = tt;	// Change type to reflect new state
				
				//Debug.Log("(DEBUG) TileMap.buildNewTile(" + x + ", " + z + ")'s spaceState is: " + ct.spaceState);

				// Store appropriate GameObjects into their respective dictionaries
				tileStores[key] = go;
				//clickTileStores[key] = ct;
				if(type==2){
					Debug.Log("Check Hazmat");
					Debug.Log(gm.hazmatManager.containsKey(x,z,gm.hazmatManager.placedHazmat));
					if(gm.hazmatManager.containsKey(x,z,gm.hazmatManager.placedHazmat)||gm.hazmatManager.containsKey(x,z,gm.hazmatManager.movingHazmat)){
						int[] tmp = new int[2];
						tmp[0] = x;
						tmp[1] = z;
						gm.fireManager.hazametList.AddLast(tmp);
					}
				}
				
			}
		}


		
	}

	public void InitializeFamily(){
		buildNewTile(3,5,2);
		gm.UpdateTile(3,5,2);
		buildNewTile(2,4,2);
		gm.UpdateTile(2,4,2);
		buildNewTile(3,4,2);
		gm.UpdateTile(3,4,2);
		buildNewTile(4,4,2);
		gm.UpdateTile(4,4,2);
		buildNewTile(5,4,2);
		gm.UpdateTile(5,4,2);
		buildNewTile(4,3,2);
		gm.UpdateTile(4,3,2);
		buildNewTile(6,2,2);
		gm.UpdateTile(6,2,2);
		buildNewTile(6,1,2);
		gm.UpdateTile(6,1,2);
		buildNewTile(7,2,2);
		gm.UpdateTile(7,2,2);
		buildNewTile(2,5,2);
		gm.UpdateTile(2,5,2);
	}

	public void InitializeExperienced(){
		Debug.Log("Initiating Experienced");
		System.Random rand=new System.Random();
		// First Blood
		List<int[]> firstExplosion=new List<int[]>();
		firstExplosion.Add(new int[]{3,4});
		firstExplosion.Add(new int[]{4,4});
		firstExplosion.Add(new int[]{5,4});
		firstExplosion.Add(new int[]{6,4});
		firstExplosion.Add(new int[]{6,3});
		firstExplosion.Add(new int[]{5,3});
		firstExplosion.Add(new int[]{4,3});
		firstExplosion.Add(new int[]{3,3});
		
		int x=rand.Next(0,8);
		int[] first=firstExplosion[x];
		gm.hazmatManager.addHazmat(first[0],first[1],(int)HazmatStatus.Hazmat);
		gm.AddHazmat(first[0],first[1],(int)HazmatStatus.Hazmat);
		buildNewTile(first[0],first[1],2);
		gm.UpdateTile(first[0],first[1],2);
		gm.fireManager.explosion();

		Debug.Log("First Explosion");

		// Second Blood
		int numOfExplosion=2;
		if(StaticInfo.level.Equals("Experienced-Heroic")){
			numOfExplosion=3;
		}
		int i=0;
		Debug.Log(numOfExplosion);
		while(i<numOfExplosion){
			Debug.Log("in numOfExplosion");
			int randX=rand.Next(1,9);
			int randZ=rand.Next(1,7);
			while(tiles[randX,randZ]==2){
				randX=rand.Next(1,9);
				randZ=rand.Next(1,7);
			}
			gm.hazmatManager.addHazmat(randX,randZ,(int)HazmatStatus.Hazmat);
			gm.AddHazmat(randX,randZ,(int)HazmatStatus.Hazmat);
			buildNewTile(randX,randZ,2);
			gm.UpdateTile(randX,randZ,2);
			gm.fireManager.explosion();
			i++;
		}
	}
	
    public void MoveSelectedUnitTo(int x, int z, int in_status ) {
		//selectedUnit2.transform.position = new Vector3(x, 0.2f, z);

		//Debug.Log("Running TileMap.MoveSelectedUnitTo(" + x + ", " + z + ")");
		selectedUnit.tryMove(x, z, in_status, goo);
	}
}
