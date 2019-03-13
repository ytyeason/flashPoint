using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{

    // Variables to track our things
    public int x;
    public int z;
    public DoorManager doorMap;
    public int type;

    // Occurs when we click the mouse:
    void OnMouseUp()
    {
        if (doorMap.gm.isMyTurn)
        {
            int doorX = x / 5;
            int doorZ = z / 5;

            Debug.Log("Clicked type: " + type);

            if (type == 0)
            {
                doorMap.ChangeDoor(doorX, doorZ, 2, 0);
                doorMap.gm.UpdateDoor(doorX, doorZ, 2, 0);
            }

            if (type == 1)
            {
                doorMap.ChangeDoor(doorX, doorZ, 3, 1);
                doorMap.gm.UpdateDoor(doorX, doorZ, 3, 1);
            }

            if (type == 2)
            {
                doorMap.ChangeDoor(doorX, doorZ, 0, 2);
                doorMap.gm.UpdateDoor(doorX, doorZ, 0, 2);
            }

            if (type == 3)
            {
                doorMap.ChangeDoor(doorX, doorZ, 1, 3);
                doorMap.gm.UpdateDoor(doorX, doorZ, 1, 3);
            }


            //Debug.Log("Clicked x: " + tileX + ", z: " + tileZ);
        }
        else
        {
            Debug.Log("Not my turn, dont click");
        }

    }
}
