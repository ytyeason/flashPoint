using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallManager
{
    readonly int wallMapSizeX = 10;
    readonly int wallMapSizeZ = 8;

    public Dictionary<int[],GameObject> hwallStores = new Dictionary<int[], GameObject>();
    public Dictionary<int[],GameObject> vwallStores = new Dictionary<int[], GameObject>();

    public List<int[]> defaultHorizontalWalls = new List<int[]>();
    public List<int[]> defaultVerticalWalls = new List<int[]>();

    public WallType[] wallTypes;

    public GameManager gm;

    void StartWallManager()
    {
        PopulateWalls();
        
        GenerateMapVisual();
        
    }

    public WallManager(WallType[] wallTypes, GameManager gm)
    {
        this.wallTypes = wallTypes;
        this.gm = gm;
        StartWallManager();
    }

    void PopulateWalls()
    {  
        defaultHorizontalWalls.Add(new int[] {1,1});
        defaultHorizontalWalls.Add(new int[] {1,2});
        defaultHorizontalWalls.Add(new int[] { 1, 3 });
        defaultHorizontalWalls.Add(new int[] { 1, 4 });
        defaultHorizontalWalls.Add(new int[] { 1, 5 });

        defaultVerticalWalls.Add(new int[] { 1, 1 });
        defaultVerticalWalls.Add(new int[] { 1, 2 });
        defaultVerticalWalls.Add(new int[] { 1, 5 });

    }

    public void BreakWall(int x, int z, int type, int horizontal)
    {

        if (horizontal == 1)//we're breaking a hwall
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
                    
                    WallType wt = wallTypes[type];
                    //GameObject objectW = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x * 5, 0, z * 5-2), Quaternion.identity);
                    GameObject objectW = gm.instantiateObject(wt.wallVisualPrefab, new Vector3(x * 5, 0, z * 5-2), Quaternion.identity);       
                    
                    Wall w = objectW.GetComponent<Wall>();
                    w.x = x * 5;
                    w.z = z * 5;
                    w.type = type;
                    w.wallMap = this;
                
                    int[] k = new int[2];
                    k[0] = x;
                    k[1] = z;
                    hwallStores[k] = objectW;
                    
                }
            }
        }
        else
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
                    
                    WallType wt = wallTypes[type];
                    //GameObject objectW = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x * 5-2, 0, z * 5), Quaternion.Euler(0,90,0));
                    GameObject objectW = gm.instantiateObject(wt.wallVisualPrefab, new Vector3(x * 5-2, 0, z * 5), Quaternion.Euler(0,90,0)); 
                    
                    Wall w = objectW.GetComponent<Wall>();
                    w.x = x * 5;
                    w.z = z * 5;
                    w.type = type;
                    w.wallMap = this;
                
                    int[] k = new int[2];
                    k[0] = x;
                    k[1] = z;
                    vwallStores[k] = objectW;
                    
                }
            }
        }

        
    }

    void GenerateMapVisual() {
        
        foreach (var hWall in defaultHorizontalWalls)
        {
            WallType wt = wallTypes[0];
            //GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(hWall[0]*5, 0, hWall[1]*5-2), Quaternion.identity );
            GameObject go = gm.instantiateObject(wt.wallVisualPrefab, new Vector3(hWall[0]*5, 0, hWall[1]*5-2), Quaternion.identity);
            
            Wall w = go.GetComponent<Wall>();
            //Debug.Log(w);
            // Assign the variables as needed
            w.x = hWall[0]*5;
            w.z = hWall[1]*5;
            w.wallMap = this;
            w.type = 0;

            hwallStores[hWall] = go;
        }

        foreach (var vWall in defaultVerticalWalls)
        {
            WallType wt = wallTypes[1];
            Debug.Log(wt);
            //GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(vWall[0]*5-2, 0, vWall[1]*5), Quaternion.Euler(0,90,0) );
            GameObject go = gm.instantiateObject(wt.wallVisualPrefab, new Vector3(vWall[0]*5-2, 0, vWall[1]*5), Quaternion.Euler(0,90,0));
            
            Wall w = go.GetComponent<Wall>();
            //Debug.Log(w);
            // Assign the variables as needed
            w.x = vWall[0]*5;
            w.z = vWall[1]*5;
            w.wallMap = this;
            w.type = 1;

            vwallStores[vWall] = go;
        }
        
    }


}
