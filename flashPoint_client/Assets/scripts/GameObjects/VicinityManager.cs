using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VicinityManager : MonoBehaviour {

	public GameManager gm;
	public int[,] tiles;
	public VicinityTile[,] VeteranVicinity; // Used only for Veteran bookkeeping
	public int f_x;
	public int f_z;
	public bool debugMode = false;

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

	// Returns true if coordinates are in the Veteran's vicinity
	public bool checkIfInVicinity(int in_x, int in_z)
	{
		for (int x = 0; x < gm.mapSizeX; x++)
		{
			for (int z = 0; z < gm.mapSizeZ; z++)
			{
				if (in_x == x && in_z == z && VeteranVicinity[x, z].distFromVet != -1)
				{
					return true;
				}
			}
		}

		return false;
	}

	// Reset status of explored and distFromVet
	public void reset()
	{
		for (int x = 0; x < gm.mapSizeX; x++) {
			for (int z = 0; z < gm.mapSizeZ; z++) {
				VeteranVicinity[x, z].explored = false;
				VeteranVicinity[x, z].distFromVet = -1;
			}
		}
	}

	// Used for sanity checks
	public void printVicinityLocations()
	{
		for (int x = 0; x < gm.mapSizeX; x++)
		{
			for (int z = 0; z < gm.mapSizeZ; z++)
			{	// Print to console
				if(VeteranVicinity[x, z].distFromVet != -1 ) Debug.Log("VICINITY LOCATION:  (x, z)  ->  (" + x + ", " + z + ")");
			}
		}
	}

	// Called after the knockdown() called during the Veteran's turn
	public void updateVicinityArr(int in_f_x, int in_f_z)
	{
		// Reset explored & distFromVet
		reset();

		// Coordinates of the Veteran
		f_x = in_f_x;
		f_z = in_f_z;
		//Debug.Log("VIC-TEST: x, z   " + f_x + ", " + f_z);

		// Mark the vicinity's propagation point
		VeteranVicinity[f_x, f_z].explored = true;
		//VeteranVicinity[f_x, f_z].distFromVet = 0;
		// Begin recursively mapping the Veteran's vicinity
		rec_markVicinity(f_x, f_z, 1);


		// Sanity check
		printVicinityLocations();
	}

	
	public void rec_markVicinity(int x_loc, int z_loc, int numStepsTaken){
		if (debugMode) Debug.Log("Looking at (x, z, num): " + x_loc + ", " + z_loc + ", " + numStepsTaken);

		// Recursive calls to markVicinity
		if(numStepsTaken <= 3){
			// Check right
			if (x_loc <= 8 && VeteranVicinity[x_loc + 1, z_loc].explored == false
				&& (tiles[x_loc + 1, z_loc] != 1 && tiles[x_loc + 1, z_loc] != 2) && !gm.wallManager.checkIfVWall(x_loc + 1, z_loc))
			{
				if (debugMode) Debug.Log("R: (" + (x_loc + 1) + ", " + z_loc + ", " + numStepsTaken + "): Not explored, normal tile, no vertical wall");
				if (gm.doorManager.checkIfVDoor(x_loc + 1, z_loc))
				{
					if (debugMode) Debug.Log("R: (" + (x_loc + 1) + ", " + z_loc + ", " + numStepsTaken + "): Found VDoor (" + (x_loc + 1) + ", " + z_loc + ")");

					if (gm.doorManager.checkIfOpenVDoor(x_loc + 1, z_loc))
					{
						if (debugMode) Debug.Log("R: (" + (x_loc + 1) + ", " + z_loc + ", " + numStepsTaken + "): VDoor was open, marking!");

						VeteranVicinity[x_loc + 1, z_loc].explored = true;
						VeteranVicinity[x_loc + 1, z_loc].distFromVet = numStepsTaken;
						rec_markVicinity(x_loc + 1, z_loc, numStepsTaken + 1);
					}
				}
				else
				{
					if (debugMode) Debug.Log("R: (" + (x_loc + 1) + ", " + z_loc + ", " + numStepsTaken + "): No door was found, marking!");

					VeteranVicinity[x_loc + 1, z_loc].explored = true;
					VeteranVicinity[x_loc + 1, z_loc].distFromVet = numStepsTaken;
					rec_markVicinity(x_loc + 1, z_loc, numStepsTaken + 1);
				}
			}

			// Check left
			if (x_loc >= 1 && VeteranVicinity[x_loc - 1, z_loc].explored == false
				&& (tiles[x_loc - 1, z_loc] != 1 && tiles[x_loc - 1, z_loc] != 2) && !gm.wallManager.checkIfVWall(x_loc, z_loc))
			{
				if (debugMode) Debug.Log("L: (" + (x_loc - 1) + ", " + z_loc + ", " + numStepsTaken + "): Not explored, normal tile, no vertical wall");
				if (gm.doorManager.checkIfVDoor(x_loc, z_loc))
				{
					if (debugMode) Debug.Log("L: (" + (x_loc - 1) + ", " + z_loc + ", " + numStepsTaken + "): Found VDoor (" + x_loc + ", " + z_loc + ")");
					
					if (gm.doorManager.checkIfOpenVDoor(x_loc, z_loc))
					{
						if (debugMode) Debug.Log("L: (" + (x_loc - 1) + ", " + z_loc + ", " + numStepsTaken + "): VDoor was open, marking!");

						VeteranVicinity[x_loc - 1, z_loc].explored = true;
						VeteranVicinity[x_loc - 1, z_loc].distFromVet = numStepsTaken;
						rec_markVicinity(x_loc - 1, z_loc, numStepsTaken + 1);
					}
				}
				else
				{
					if (debugMode) Debug.Log("L: (" + (x_loc - 1) + ", " + z_loc + ", " + numStepsTaken + "): No door was found, marking!");

					VeteranVicinity[x_loc - 1, z_loc].explored = true;
					VeteranVicinity[x_loc - 1, z_loc].distFromVet = numStepsTaken;
					rec_markVicinity(x_loc - 1, z_loc, numStepsTaken + 1);
				}
			}

			// Check tile above
			if (z_loc <= 6 && VeteranVicinity[x_loc, z_loc + 1].explored == false &&
				(tiles[x_loc, z_loc + 1] != 1 && tiles[x_loc, z_loc + 1] != 2) && !gm.wallManager.checkIfHWall(x_loc, z_loc + 1))
			{
				// Check that above us isn't an open door or a wall that is intact
				if (debugMode) Debug.Log("U: (" + x_loc + ", " + (z_loc + 1) + ", " + numStepsTaken + "): Not explored, normal tile, no horizontal wall");

				// If there's a door it has to be open to continue
				if (gm.doorManager.checkIfHDoor(x_loc, z_loc + 1))
				{
					if (debugMode) Debug.Log("U: (" + x_loc + ", " + (z_loc + 1) + ", " + numStepsTaken + "): Found HDoor (" + x_loc + ", " + (z_loc + 1) + ")");

					if (gm.doorManager.checkIfOpenHDoor(x_loc, z_loc + 1)){
						if (debugMode) Debug.Log("U: (" + x_loc + ", " + (z_loc + 1) + ", " + numStepsTaken + "): HDoor was open, marking!");

						VeteranVicinity[x_loc, z_loc + 1].explored = true;
						VeteranVicinity[x_loc, z_loc + 1].distFromVet = numStepsTaken;
						rec_markVicinity(x_loc, z_loc + 1, numStepsTaken + 1);
					}
				}
				else {
					if (debugMode) Debug.Log("U: (" + x_loc + ", " + (z_loc + 1) + ", " + numStepsTaken + "): No door was found, marking!");

					VeteranVicinity[x_loc, z_loc + 1].explored = true;
					VeteranVicinity[x_loc, z_loc + 1].distFromVet = numStepsTaken;
					rec_markVicinity(x_loc, z_loc + 1, numStepsTaken + 1);
				}
			}

			// Check tile below
			if (z_loc >= 1 && VeteranVicinity[x_loc, z_loc - 1].explored == false &&
				(tiles[x_loc, z_loc - 1] != 1 && tiles[x_loc, z_loc - 1] != 2) && !gm.wallManager.checkIfHWall(x_loc, z_loc))
			{
				if (debugMode) Debug.Log("D: (" + x_loc + ", " + (z_loc - 1) + ", " + numStepsTaken + "): Not explored, normal tile, no horizontal wall");
				if (gm.doorManager.checkIfHDoor(x_loc, z_loc))
				{
					if (debugMode) Debug.Log("D: (" + x_loc + ", " + (z_loc - 1) + ", " + numStepsTaken + "): Found HDoor (" + x_loc + ", " + z_loc + ")");

					if (gm.doorManager.checkIfOpenHDoor(x_loc, z_loc))
					{
						if (debugMode) Debug.Log("D: (" + x_loc + ", " + (z_loc - 1) + ", " + numStepsTaken + "): HDoor was open, marking!");

						VeteranVicinity[x_loc, z_loc - 1].explored = true;
						VeteranVicinity[x_loc, z_loc - 1].distFromVet = numStepsTaken;
						rec_markVicinity(x_loc, z_loc - 1, numStepsTaken + 1);
					}
				}
				else
				{
					if (debugMode) Debug.Log("D: (" + x_loc + ", " + (z_loc - 1) + ", " + numStepsTaken + "): No door was found, marking!");

					VeteranVicinity[x_loc, z_loc - 1].explored = true;
					VeteranVicinity[x_loc, z_loc - 1].distFromVet = numStepsTaken;
					rec_markVicinity(x_loc, z_loc - 1, numStepsTaken + 1);
				}
			}
		}
	}
}
