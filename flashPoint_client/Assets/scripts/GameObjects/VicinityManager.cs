using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VicinityManager : MonoBehaviour {

	public GameManager gm;
	public int[,] tiles;
	public VicinityTile[,] VeteranVicinity; // Used only for Veteran bookkeeping
	public int f_x;
	public int f_z;

	// Constructor
	public VicinityManager( GameManager in_gm, int[,] tile_arr)
	{
		// Connect back to singleton GameManager
		gm = in_gm;

		//	Fetch info about tile status (fire, smoke etc)
		tiles = tile_arr;

		// Initiliaze the 2D array
		VeteranVicinity = new VicinityTile[gm.mapSizeX, gm.mapSizeZ];
		for (int x = 0; x < gm.mapSizeX; x++)
		{
			for (int z = 0; z < gm.mapSizeZ; z++)
			{
				VeteranVicinity[x, z] = new VicinityTile(x, z);
			}
		}
	}


	// Reset status of explore 
	public void resetExplored()
	{
		for (int x = 0; x < gm.mapSizeX; x++) {
			for (int z = 0; x < gm.mapSizeZ; z++) {
				VeteranVicinity[x, z].explored = false;
			}
		}
	}

	// Called after the knockdown() called during the Veteran's turn
	public void updateVicinityArr(int in_f_x, int in_f_z)
	{
		// Coordinates of the Veteran
		f_x = in_f_x;
		f_z = in_f_z;

		// Mark the vicinity TODO ADD GUI COMPONENT
		VeteranVicinity[f_x, f_z].explored = true;
		if( f_x <= 8)	rec_markVicinity(f_x + 1, f_z, 1);
		if (f_x >= 1) rec_markVicinity(f_x - 1, f_z, 1);
		if (f_z <= 6) rec_markVicinity(f_x, f_z + 1, 1);
		if (f_z >= 1) rec_markVicinity(f_x, f_z - 1, 1);

		// Reset explored status
		resetExplored();
	}

	//
	public void rec_markVicinity(int x_loc, int z_loc, int numStepsTaken){
		// Recursive calls to markVicinity
		if(numStepsTaken <= 3){
			// Check right
			if (x_loc <= 8 && VeteranVicinity[x_loc + 1, z_loc].explored == false &&
				// If the tile is not clear or an unopened door or undestroyed wall we can't continue:
				!(tiles[x_loc + 1, z_loc] != 0 || !gm.doorManager.checkIfOpenVDoor(x_loc + 1, z_loc) || gm.wallManager.checkIfVWall(x_loc + 1, z_loc))	)
			{
				VeteranVicinity[x_loc + 1, z_loc].explored = true;
				VeteranVicinity[x_loc + 1, z_loc].distFromVet = numStepsTaken;
				rec_markVicinity(x_loc + 1, z_loc, numStepsTaken + 1);
			}
			// Check left
			if (x_loc >= 1 && VeteranVicinity[x_loc - 1, z_loc].explored == false &&
				!(tiles[x_loc - 1, z_loc] != 0 || !gm.doorManager.checkIfOpenVDoor(x_loc, z_loc) || gm.wallManager.checkIfVWall(x_loc, z_loc))	)
			{
				VeteranVicinity[x_loc - 1, z_loc].explored = true;
				VeteranVicinity[x_loc - 1, z_loc].distFromVet = numStepsTaken;
				rec_markVicinity(x_loc - 1, z_loc, numStepsTaken + 1);
			}
			// Check tile above
			if (z_loc <= 6 && VeteranVicinity[x_loc, z_loc + 1].explored == false &&
				!(tiles[x_loc, z_loc + 1] != 0 || !gm.doorManager.checkIfOpenHDoor(x_loc, z_loc + 1) || gm.wallManager.checkIfHWall(x_loc, z_loc + 1))	)
			{
				VeteranVicinity[x_loc, z_loc + 1].explored = true;
				VeteranVicinity[x_loc, z_loc + 1].distFromVet = numStepsTaken;
				rec_markVicinity(x_loc, z_loc + 1, numStepsTaken + 1);
			}
			// Check tile below
			if (z_loc >= 1 && VeteranVicinity[x_loc, z_loc - 1].explored == false &&
				!(tiles[x_loc, z_loc - 1] != 0 || !gm.doorManager.checkIfOpenHDoor(x_loc, z_loc) || gm.wallManager.checkIfHWall(x_loc, z_loc))	)
			{
				VeteranVicinity[x_loc, z_loc - 1].explored = true;
				VeteranVicinity[x_loc, z_loc - 1].distFromVet = numStepsTaken;
				rec_markVicinity(x_loc, z_loc - 1, numStepsTaken + 1);
			}
		}
	}
}
