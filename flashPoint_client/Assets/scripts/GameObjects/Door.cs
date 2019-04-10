//		DOOR

using UnityEngine;
using System.Collections;
using System;
using System;

[Serializable]
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
		if (doorMap.gm.isMyTurn && !StaticInfo.StartingPosition)
		{
			int doorX = x / 6;
			int doorZ = z / 6;

			Debug.Log("Clicked type: " + type);
			Debug.Log("doorX: " + x + ", doorZ: " + x);

            int currentX = doorMap.gm.fireman.currentX;
            int currentZ = doorMap.gm.fireman.currentZ;

            bool canDo = true;
            if (doorMap.gm.operationManager.inCommand)
            {
                currentX = doorMap.gm.operationManager.controlled.currentX;
                currentZ = doorMap.gm.operationManager.controlled.currentZ;
                if (doorMap.gm.operationManager.controlled.role == Role.CAFS)
                {
                    if (doorMap.gm.operationManager.commandMoves+doorMap.gm.fireman.FreeAP <1)
                    {
                        canDo = false;
                    }else{
                        
                    }
                }else
                if (doorMap.gm.fireman.remainingSpecAp+doorMap.gm.fireman.FreeAP < 1)
                {
                    canDo = false;
                }

            }
            else
            {
                if (doorMap.gm.fireman.FreeAP < 1)
                {
                    canDo = false;
                }
            }

            if(doorMap.gm.fireman.role==Role.Dog){
                canDo=false;
            }

            bool det=doorMap.gm.operationManager.inCommand;

            if (currentX==x&&currentZ-z==-6||currentX==x&&currentZ==z||currentZ==z&&currentX-x==-6&&canDo) // reachable door
            {
                if (type == 0) // Closed horizontal
                {
                    doorMap.ChangeDoor(doorX, doorZ, 2, 0,det);
                    doorMap.gm.UpdateDoor(doorX, doorZ, 2, 0,det);
                }

                if (type == 1) // Closed vertical
                {
                    doorMap.ChangeDoor(doorX, doorZ, 3, 1,det);
                    doorMap.gm.UpdateDoor(doorX, doorZ, 3, 1,det);
                }

                if (type == 2) // Open horizontal
                {
                    doorMap.ChangeDoor(doorX, doorZ, 0, 2,det);
                    doorMap.gm.UpdateDoor(doorX, doorZ, 0, 2,det);
                }

                if (type == 3) // Open vertical
                {
                    doorMap.ChangeDoor(doorX, doorZ, 1, 3,det);
                    doorMap.gm.UpdateDoor(doorX, doorZ, 1, 3,det);
                }

                if(doorMap.gm.operationManager.inCommand){
                    if(doorMap.gm.operationManager.controlled.role==Role.CAFS){
                        if(doorMap.gm.operationManager.commandMoves<=0){
                            doorMap.gm.fireman.setAP(doorMap.gm.fireman.FreeAP-1);
                        }else{
                            doorMap.gm.operationManager.commandMoves-=1;
                            if(doorMap.gm.fireman.remainingSpecAp>=1){
                                doorMap.gm.fireman.setSpecAP(doorMap.gm.fireman.remainingSpecAp-1);
                            }else{
                                doorMap.gm.fireman.setAP(doorMap.gm.fireman.FreeAP-1);
                            }
                        }
                    }else{
                        if(doorMap.gm.fireman.remainingSpecAp>=1){
                            doorMap.gm.fireman.setSpecAP(doorMap.gm.fireman.remainingSpecAp-1);
                        }else{
                            doorMap.gm.fireman.setAP(doorMap.gm.fireman.FreeAP-1);
                        }
                    }
                }
                
                
            }




			//Debug.Log("Clicked x: " + tileX + ", z: " + tileZ);
		}
		else
		{
			Debug.Log("Not my turn, dont click");
		}

	}
}
