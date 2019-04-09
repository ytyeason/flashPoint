using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VicinityTile : MonoBehaviour {

	public int x_coord;
	public int z_coord;
	public bool explored = false;
	public int distFromVet = -1;

	public VicinityTile(int in_x, int in_z){
		x_coord = in_x;
		z_coord = in_z;
	}	
}
