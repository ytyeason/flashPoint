//		DOOR

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
		Debug.Log("MOUSE UP DOOR");
		if (doorMap.gm.isMyTurn)
		{
			int doorX = x / 6;
			int doorZ = z / 6;

			Debug.Log("Clicked type: " + type);
			Debug.Log("doorX: " + doorX + ", doorZ: " + doorZ);

			if (type == 0) // Closed horizontal
			{
				doorMap.ChangeDoor(doorX, doorZ, 2, 0);
				doorMap.gm.UpdateDoor(doorX, doorZ, 2, 0);
			}

			if (type == 1) // Closed vertical
			{
				doorMap.ChangeDoor(doorX, doorZ, 3, 1);
				doorMap.gm.UpdateDoor(doorX, doorZ, 3, 1);
			}

			if (type == 2) // Open horizontal
			{
				doorMap.ChangeDoor(doorX, doorZ, 0, 2);
				doorMap.gm.UpdateDoor(doorX, doorZ, 0, 2);
			}

			if (type == 3) // Open vertical
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
