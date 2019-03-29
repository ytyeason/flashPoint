using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour {
	readonly int wallMapSizeX = 10;
	readonly int wallMapSizeZ = 8;

	public Dictionary<int[],GameObject> vehicleStores = new Dictionary<int[], GameObject>();
	//public GameObject ambulance;
	//public GameObject engine;

	public VehicleType[] vehicleTypes;

	public GameManager gm;

	public void StartvehicleManager()
	{
	//	PopulateVehicles();

		GenerateVehicleVisual(0,9,5,4);

	}
	/*
	void PopulateVehicles()
	{
		//vehicleStores.Add(new int[] { 1, 1 });
	}*/

	public VehicleManager(VehicleType[] vehicleTypes, GameManager gm)
    {
        this.vehicleTypes= vehicleTypes;
        this.gm = gm;
    }

	void GenerateVehicleVisual(int x1, int x2, int z1, int z2)
	{

		VehicleType vt = vehicleTypes[0];
	
		GameObject engine = gm.instantiateObject(vt.vehicleVisualPrefab, new Vector3(x1 * 5, 0, z1 * 5), Quaternion.identity);
		
		// Connect a ClickableTile to each TileType
		ClickableVehicle cv = engine.GetComponent<ClickableVehicle>();
		// Assign the variables as needed
		cv.vehicleX = x1*5;
        cv.vehicleZ = z1*5;
		cv.map = this;
		cv.type = vt;
	
		int[] p = new int[2];
		p[0] = x1;
		p[1] = z1;
		vehicleStores[p] = engine;

		VehicleType vt1 = vehicleTypes[1];
	
		GameObject ambulance = gm.instantiateObject(vt1.vehicleVisualPrefab, new Vector3(x2 * 5, 0, z2 * 5), Quaternion.identity);
		
		// Connect a ClickableTile to each TileType
		ClickableVehicle cv1 = ambulance.GetComponent<ClickableVehicle>();
		// Assign the variables as needed
		cv1.vehicleX = x2*5;
        cv1.vehicleZ = z2*5;
		cv1.map = this;
		cv1.type = vt1;
	
		int[] p1 = new int[2];
		p1[0] = x2;
		p1[1] = z2;
		vehicleStores[p1] = ambulance;

	}
}
