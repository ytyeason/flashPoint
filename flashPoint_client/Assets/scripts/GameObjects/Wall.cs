using UnityEngine;
using System.Collections;
using System;

[Serializable]
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
		Debug.Log("MOUSE UP WALL");
		if (wallMap.gm.isMyTurn&&!StaticInfo.StartingPosition)
		{
			int wallX = x / 6;
			int wallZ = z / 6;

			Debug.Log("Clicked type: " + type);

            int currentX = wallMap.gm.fireman.currentX;
            int currentZ = wallMap.gm.fireman.currentZ;

            bool canDo = true;

            if (wallMap.gm.operationManager.inCommand)
            {
                canDo = false;
            }
            else
            {
                if (wallMap.gm.fireman.role!=Role.RescueSpec&&wallMap.gm.fireman.FreeAP < 2||wallMap.gm.fireman.role==Role.RescueSpec&&wallMap.gm.fireman.FreeAP<1)
                {
                    canDo = false;
                }
            }

            if (currentX == x && currentZ - z == -6 || currentX == x && currentZ == z || currentZ == z && currentX - x == -6 && canDo)
            {
                if (type == 0) // Horizontal normal
                {
                    Debug.Log("NORMAL: Wall coord (x, z):  " + x + "," + z);
                    wallMap.BreakWall(wallX, wallZ, 2, 1, false);
                    wallMap.gm.UpdateWall(wallX, wallZ, 2, 1);
                    
                }

                if (type == 1) // Vertical normal
                {
                    Debug.Log("NORMAL: Wall coord (x, z):  " + x + "," + z);
                    wallMap.BreakWall(wallX, wallZ, 3, 0, false);
                    wallMap.gm.UpdateWall(wallX, wallZ, 3, 0);
               
                }

                if (type == 2) // Horizontal damaged
                {
                    Debug.Log("NORMAL -> DESTROYED");
                    wallMap.BreakWall(wallX, wallZ, 4, 1, false);
                    wallMap.gm.UpdateWall(wallX, wallZ, 4, 1);
                   
                }

                if (type == 3) // Vertical damaged
                {
                    Debug.Log("NORMAL -> DESTROYED");
                    wallMap.BreakWall(wallX, wallZ, 5, 0, false);
                    wallMap.gm.UpdateWall(wallX, wallZ, 5, 0);
                    
                }
                //      4 -> Horizontal destroyed
                //      5 -> Vertical destroyed

                
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
