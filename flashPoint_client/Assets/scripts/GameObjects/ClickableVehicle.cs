using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableVehicle : MonoBehaviour {

	// Variables to track our things
	public int vehicleX;
	public int vehicleZ;
	public VehicleManager map;
	public VehicleType type;
    bool chosen;

	// Occurs when we click the mouse:
	void OnMouseUp() {
		Debug.Log("MOUSE UP CLICKABLE TILE");
		if (map.gm.isMyTurn)
        {
            //Debug.Log(this);

            int x = vehicleX / 5;
            int z = vehicleZ / 5;

            if (chosen == false)
            {
                chosen = true;
            }

/*
            if (map.tiles[x, z] != 0)
            {
                // Extinguish the fire or smoke:
                int result = map.selectedUnit.extinguishFire(map.tiles[x, z]);
                if (result == -1)
                    Debug.Log("Not enough AP to extinguish the fire");
                else
                {
                    map.buildNewTile(x, z, result);
                    //broadcast new tile
                    map.gm.UpdateTile(x, z, result);
                }

            }
            else
            {
                // Move to selected tile (only if tile is normal)
                map.MoveSelectedUnitTo(tileX, tileZ, 0);
            }
*/

            //Debug.Log("Clicked x: " + tileX + ", z: " + tileZ);
        }
        else
        {
            Debug.Log("Not my turn, dont click");
        }


    }
}
