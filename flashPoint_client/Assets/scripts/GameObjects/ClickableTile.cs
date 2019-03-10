using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour {

	// Variables to track our things
	public int tileX;
	public int tileZ;
	public TileMap map;
	public TileType type;

	// Occurs when we click the mouse:
	void OnMouseUp() {

		//Debug.Log(this);

        int x = tileX / 5;
		int z = tileZ / 5;

        if (map.tiles[x,z]!=0)
		{
			// Extinguish the fire or smoke:
			int result = map.selectedUnit.extinguishFire(map.tiles[x, z]);
			if( result == -1)
				Debug.Log("Not enough AP to extinguish the fire");
			else
				map.buildNewTile(x, z, result);
		}
		else
		{
			// Move to selected tile (only if tile is normal)
			map.MoveSelectedUnitTo(tileX, tileZ, 0);
		}
	
		
        //Debug.Log("Clicked x: " + tileX + ", z: " + tileZ);
    }
}
