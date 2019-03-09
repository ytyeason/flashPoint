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
				tiles[x,z] = 1;
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
	
	public void calcDist(int x_src, int y_src, int x_dst, int y_dst) {
		
	}

    public void MoveSelectedUnitTo(int x, int z) {
        selectedUnit.transform.position = new Vector3(x, 0.2f, z);
    }

	// Until EOF: this is functionality present for Pathfinding
	public int num_x_tiles = 8;
	public int num_y_tiles = 6;
	public int totalTiles = 48;
	public int totalCost = 0;

	int minDistance(int[] dist, bool[] sptSet)
	{
		// Initialize min value 
		int min = int.MaxValue, min_index = -1;

		for (int v = 0; v < totalTiles; v++)
		{
			if (sptSet[v] == false && dist[v] <= min)
			{
				min = dist[v];
				min_index = v;
			}
		}

		return min_index;
	}

	// A utility function to print the constructed distance array 
	void printSolution(int[] dist, int n)
	{
		Debug.Log("Vertex distance from source:");
		for (int i = 0; i < totalTiles; i++)
			Debug.Log(i + ": \t\t" + dist[i]);
	}

	/*	Funtion that implements Dijkstra's single source shortest path algorithm for a graph represented using adjacency  
		matrix representation  */
	void dijkstra(int[,] graph, int src)
	{
		/* The output array. dist[i] will hold the shortest distance from src to i */
		int[] dist = new int[totalTiles];

		/*	sptSet[i] will true if vertex i is included in shortest path tree or shortest distance from
			src to i is finalized  */
		bool[] sptSet = new bool[totalTiles];

		// Initialize all distances as INFINITE and stpSet[] as false 
		for (int i = 0; i < totalTiles; i++)
		{
			dist[i] = int.MaxValue;
			sptSet[i] = false;
		}

		// Distance of source vertex from itself is always 0 
		dist[src] = 0;

		// Find shortest path for all vertices 
		for (int count = 0; count < totalTiles - 1; count++)
		{
			/*	Pick the minimum distance vertex from the set of vertices not yet processed. u is always equal to
				src in first iteration. */
			int u = minDistance(dist, sptSet);

			// Mark the picked vertex as processed 
			sptSet[u] = true;

			// Update dist value of the adjacent vertices of the picked vertex. 
			for (int v = 0; v < totalTiles; v++)
			{
				/*	Update dist[v] only if is not in sptSet, there is an edge from u to v, and total weight of path  
					from src to v through u is smaller than current value of dist[v]  */
				if (!sptSet[v] && graph[u, v] != 0 && dist[u] != int.MaxValue && dist[u] + graph[u, v] < dist[v])
				{
					dist[v] = dist[u] + graph[u, v];
				}
			}
		}

		// print the constructed distance array 
		printSolution(dist, V);
	}
}
