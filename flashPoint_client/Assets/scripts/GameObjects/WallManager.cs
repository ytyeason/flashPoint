using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallManager: MonoBehaviour
{
    readonly int wallMapSizeX = 10;
    readonly int wallMapSizeZ = 8;

    public Dictionary<int[],GameObject> wallStores = new Dictionary<int[], GameObject>();

    //  public List<int[]> defaultHorizontalWalls = new List<int[]>();
    //  public List<int[]> defaultVerticalWalls = new List<int[]>();
    // public List<int[]> walls = new List<int[]>();

    public WallType[] wallTypes;
    public int[,] walls;

    void Start()
    {
        PopulateWalls();
        
        GenerateMapVisual();
        
    }

    void PopulateWalls()
    {  /*
        defaultHorizontalWalls.Add(new int[]{1,1});
        defaultHorizontalWalls.Add(new int[]{1,2});
        defaultHorizontalWalls.Add(new int[] { 1, 3 });
        defaultHorizontalWalls.Add(new int[] { 1, 4 });
        defaultHorizontalWalls.Add(new int[] { 1, 5 });

        defaultVerticalWalls.Add(new int[]{1,1});
        defaultVerticalWalls.Add(new int[]{1,2});
        defaultVerticalWalls.Add(new int[] { 1, 5 });

        walls.Add(new int[] { 1, 1 });
        walls.Add(new int[] { 1, 2 });
        walls.Add(new int[] { 1, 3 });
        walls.Add(new int[] { 1, 4 });
        walls.Add(new int[] { 1, 5 });
        walls.Add(new int[] { 1, 1 });
        walls.Add(new int[] { 1, 2 });
        walls.Add(new int[] { 1, 5 });
        */
        walls = new int[wallMapSizeX, wallMapSizeZ];
        for (int x = 0; x < wallMapSizeX; x++)
        {
            for (int z = 0; z < wallMapSizeZ; z++)
            {
                walls[x, z] = 0;
            }
        }

        walls[5, 1] = 1;
        walls[5, 2] = 1;
        walls[5, 3] = 1;
        walls[5, 4] = 1;


    }

    public void BreakWall(int x, int z, int type)
    {

        List<int[]> keyList = new List<int[]>(wallStores.Keys);

        foreach (var key in keyList)
        {
            if (key[0] == x && key[1] == z)
            {
                GameObject old = wallStores[key];

                Debug.Log("Breaking the wall");
                walls[x, z] = type;
                WallType wt = wallTypes[walls[x, z]];
                if (type == 0) {
                    Destroy(old);
                    wt = wallTypes[2];
                    GameObject objectW = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x * 5, 0, z * 5), Quaternion.identity);
                    Wall w = objectW.GetComponent<Wall>();
                    w.x = x * 5;
                    w.z = z * 5;
                    int[] k = new int[2];
                    k[0] = x;
                    k[1] = z;
                    wallStores[k] = objectW;
                }
                if (type == 1)
                {
                    Destroy(old);
                    wt = wallTypes[3];
                    GameObject objectW = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x * 5, 0, z * 5), Quaternion.identity);
                    Wall w = objectW.GetComponent<Wall>();
                    w.x = x * 5;
                    w.z = z * 5;
                    w.wallMap = this;
                    w.type = wt;
                    int[] k = new int[2];
                    k[0] = x;
                    k[1] = z;
                    wallStores[k] = objectW;
                }
            }
        }
    }

    void GenerateMapVisual() {

        for (int x = 0; x < wallMapSizeX; x++)
        {
            for (int z = 3; z < wallMapSizeZ; z++)
            {
                WallType wt = wallTypes[walls[x, z]];
                if (walls[x, z] == 0 || walls[x, z] == 2) {
                    GameObject go = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x * 5, 0, z * 5), Quaternion.identity);
                    Wall w = go.GetComponent<Wall>();
                    w.x = x * 5;
                    w.z = z * 5;
                    w.wallMap = this;
                    w.type = wt;
                    int[] key = new int[2];
                    key[0] = x;
                    key[1] = z;
                    wallStores[key] = go;
                }
                else {
                    GameObject go = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x * 5, 0, z * 5), Quaternion.identity);
                    Wall w = go.GetComponent<Wall>();
                    w.x = x * 5;
                    w.z = z * 5;
                    w.wallMap = this;
                    w.type = wt;
                    int[] key = new int[2];
                    key[0] = x;
                    key[1] = z;
                    wallStores[key] = go;
                }
               
            }
        }

        /*
        foreach (var hWall in walls)
        {
            WallType wt = wallTypes[0];
            GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(hWall[0]*5, 0, hWall[1]*5-2), Quaternion.identity );
            
            Wall w = go.GetComponent<Wall>();
            Debug.Log(w);
            // Assign the variables as needed
            w.x = hWall[0]*5;
            w.z = hWall[1]*5;
            w.wallManager = this;

            wallStores[hWall] = go;
        }

        foreach (var vWall in defaultVerticalWalls)
        {
            WallType wt = wallTypes[1];
            Debug.Log(wt);
            GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(vWall[0]*5-2, 0, vWall[1]*5), Quaternion.Euler(0,90,0) );
            
            Wall w = go.GetComponent<Wall>();
            Debug.Log(w);
            // Assign the variables as needed
            w.x = vWall[0]*5;
            w.z = vWall[1]*5;
            w.wallManager = this;

            wallStores[vWall] = go;
        }
        */
    }


}
