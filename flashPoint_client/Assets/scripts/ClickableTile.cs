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
        
		Debug.Log(this);
		
		int x = tileX / 5;
		int z = tileZ / 5;

		if (map.tiles[x,z]==1)
		{
			map.buildNewTile(x,z,0);
		}
		else
		{
			map.MoveSelectedUnitTo(tileX, tileZ);
		}
	
		
        //Debug.Log("Clicked x: " + tileX + ", z: " + tileZ);
    }
}
