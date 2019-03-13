using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{

    // Variables to track our things
    public int x;
    public int z;
    public WallManager wallMap;
    public int type;

    // Occurs when we click the mouse:
    void OnMouseUp()
    {
        if (wallMap.gm.isMyTurn)
        {
            int wallX = x / 5;
            int wallZ = z / 5;

            Debug.Log("Clicked type: " + type);

            if (type == 0)
            {
                wallMap.BreakWall(wallX, wallZ, 2, 1);
                wallMap.gm.UpdateWall(wallX, wallZ, 2, 1);
            }

            if (type == 1)
            {
                wallMap.BreakWall(wallX, wallZ, 3, 0);
                wallMap.gm.UpdateWall(wallX, wallZ, 3, 0);
            }

            if (type == 2)
            {
                wallMap.BreakWall(wallX, wallZ, 4, 1);
                wallMap.gm.UpdateWall(wallX, wallZ, 4, 1);
            }

            if (type == 3)
            {
                wallMap.BreakWall(wallX, wallZ, 5, 0);
                wallMap.gm.UpdateWall(wallX, wallZ, 5, 0);
            }
        }
        else
        {
            Debug.Log("Not my turn, dont click");
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
