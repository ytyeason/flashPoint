using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{

    // Variables to track our things
    public int x;
    public int z;
    public WallManager wallMap;
    public WallType type;

    // Occurs when we click the mouse:
    void OnMouseUp()
    {
        int wallX = x / 5;
        int wallZ = z / 5;

        Debug.Log("Clicked " + wallMap.walls[wallX, wallZ]);

        if (wallMap.walls[wallX, wallZ]==0)
        {
            wallMap.BreakWall(wallX, wallZ, 0);
        }

        if (wallMap.walls[wallX, wallZ]== 1)
        {
            wallMap.BreakWall(wallX, wallZ, 1);
        }


        //Debug.Log("Clicked x: " + tileX + ", z: " + tileZ);
    }
}

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall: MonoBehaviour
{
    private WallStatus status;
    public int x;
    public int z;
    public WallManager wallManager;
    public int id;

    public Wall (int id)
    {
        this.id = id;
    }

    public void SetStatus(WallStatus s)
    {
        status = s;
    }

    public WallStatus GetStatus()
    {
        return status;
    }
    
    void OnMouseUp() {
	
		
        Debug.Log("Clicked x: " + x + ", z: " + z);
    }
}*/
