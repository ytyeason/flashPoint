using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour {

	// Variables to track our things
	public int tileX;
	public int tileZ;
	public TileMap map;

	// Occurs when we click the mouse:
	void OnMouseUp() {
        map.MoveSelectedUnitTo(tileX, tileZ);
        Debug.Log("Clicked x: " + tileX + ", z: " + tileZ);
    }
}
