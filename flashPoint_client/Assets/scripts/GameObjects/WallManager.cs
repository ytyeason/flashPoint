using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallManager: MonoBehaviour
{
    public Dictionary<int[],GameObject> wallStores = new Dictionary<int[], GameObject>();
    
    public List<int[]> defaultHorizontalWalls = new List<int[]>();
    public List<int[]> defaultVerticalWalls = new List<int[]>();

    public WallType[] wallTypes;

    void Start()
    {
        populateWalls();
        
        GenerateMapVisual();
        
    }

    void populateWalls()
    {
        defaultHorizontalWalls.Add(new int[]{1,1});
        defaultHorizontalWalls.Add(new int[]{1,2});
        
        defaultVerticalWalls.Add(new int[]{1,1});
        defaultVerticalWalls.Add(new int[]{1,2});
        
    }
    
    void GenerateMapVisual() {

        
        foreach (var hWall in defaultHorizontalWalls)
        {
            WallType wt = wallTypes[0];
            GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(hWall[0]*5, 0, hWall[1]*5), Quaternion.identity );
            
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
            GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(vWall[0]*5, 0, vWall[1]*5), Quaternion.Euler(0,90,0) );
            
            Wall w = go.GetComponent<Wall>();
            Debug.Log(w);
            // Assign the variables as needed
            w.x = vWall[0]*5;
            w.z = vWall[1]*5;
            w.wallManager = this;

            wallStores[vWall] = go;
        }
    }


}
