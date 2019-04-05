using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoorManager
{
	public Dictionary<int[], GameObject> hdoorStores = new Dictionary<int[], GameObject>();
	public Dictionary<int[], GameObject> vdoorStores = new Dictionary<int[], GameObject>();

	public List<int[]> defaultHorizontalDoors = new List<int[]>();
	public List<int[]> defaultVerticalDoors = new List<int[]>();

	public DoorType[] doorTypes;

	public GameManager gm;

	void StartDoorManager()
	{
		PopulateDoors();

		GenerateMapVisual();

	}

	public DoorManager(DoorType[] doorTypes, GameManager gm)
	{
		this.doorTypes = doorTypes;
		this.gm = gm;
		StartDoorManager();
	}


public int HorizontalDoor(int x, int z)
    {
        List<int[]> keyList = new List<int[]>(hdoorStores.Keys);

        foreach (var key in keyList)
        {
            if (key[0] == x && key[1] == z)
            {
                return hdoorStores[key].GetComponent<Door>().type;
            }
        }
        return -1;
    }

    public int VerticalDoor(int x, int z)
    {
        List<int[]> keyList = new List<int[]>(vdoorStores.Keys);

        foreach (var key in keyList)
        {
            if (key[0] == x && key[1] == z)
            {
                return vdoorStores[key].GetComponent<Door>().type;
            }
        }
        return -1;
    }

    public bool isVerticalDoor(int x, int z)
    {
        List<int[]> keyList = new List<int[]>(vdoorStores.Keys);

        foreach (var key in keyList)
        {
            if (key[0] == x && key[1] == z)
            {
                return true;
            }
        }

        return false;
    }

    public bool isHorizontalDoor(int x, int z)
    {
        List<int[]> keyList = new List<int[]>(hdoorStores.Keys);

        foreach (var key in keyList)
        {
            if (key[0] == x && key[1] == z)
            {
                return true;
            }
        }

        return false;
    }
    
	void PopulateDoors()
	{
		defaultHorizontalDoors.Add(new int[] { 5, 1 });
		defaultHorizontalDoors.Add(new int[] { 3, 3 });
		defaultHorizontalDoors.Add(new int[] { 6, 3 });
		defaultHorizontalDoors.Add(new int[] { 5, 4 });
		defaultHorizontalDoors.Add(new int[] { 6, 7 });
        defaultHorizontalDoors.Add(new int[] { 3, 5 });

		defaultVerticalDoors.Add(new int[] { 1, 3 });
		defaultVerticalDoors.Add(new int[] { 6, 4 });
		defaultVerticalDoors.Add(new int[] { 9, 3 });

	}

	public void ChangeDoor(int x, int z, int toType, int type)
	{
		if (gm.fireman.changeDoor(x * 6, z * 6))
		{
			if (type == 0)//we're opening a hwall
			{
				List<int[]> keyList = new List<int[]>(hdoorStores.Keys);

				foreach (var key in keyList)
				{
					if (key[0] == x && key[1] == z)
					{
						Debug.Log("Open the door");

						GameObject old = hdoorStores[key];
						//Destroy(old);
						gm.DestroyObject(old);

						DoorType dt = doorTypes[toType];
						//GameObject objectW = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x * 6, 0, z * 6-2), Quaternion.identity);
						GameObject objectD = gm.instantiateObject(dt.doorVisualPrefab, new Vector3(x * 6 - 3, 2, z * 6 - 3), Quaternion.identity);

						Door d = objectD.GetComponent<Door>();
						d.x = x * 6;
						d.z = z * 6;
						d.type = toType;
						d.doorMap = this;

						/*
                        int[] k = new int[2];
                        k[0] = x;
                        k[1] = z;
						*/
						hdoorStores[key] = objectD;
					}
				}
			}
			if (type == 1)
			{
				List<int[]> keyList = new List<int[]>(vdoorStores.Keys);

				foreach (var key in keyList)
				{
					if (key[0] == x && key[1] == z)
					{

						Debug.Log("Open the door");

						GameObject old = vdoorStores[key];
						//Destroy(old);
						gm.DestroyObject(old);

						DoorType dt = doorTypes[toType];
						//GameObject objectW = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x * 6-2, 0, z * 6), Quaternion.Euler(0,90,0));
						GameObject objectD = gm.instantiateObject(dt.doorVisualPrefab, new Vector3(x * 6 - 3, 2, z * 6 - 3), Quaternion.Euler(0, 90, 0));

						Door d = objectD.GetComponent<Door>();
						d.x = x * 6;
						d.z = z * 6;
						d.type = toType;
						d.doorMap = this;

						/*
                        int[] k = new int[2];
                        k[0] = x;
                        k[1] = z;
						*/
						vdoorStores[key] = objectD;

					}
				}
			}
			if (type == 2)
			{
				List<int[]> keyList = new List<int[]>(hdoorStores.Keys);

				foreach (var key in keyList)
				{
					if (key[0] == x && key[1] == z)
					{
						Debug.Log("Close the door");

						GameObject old = hdoorStores[key];
						//Destroy(old);
						gm.DestroyObject(old);

						DoorType dt = doorTypes[toType];
						//GameObject objectW = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x * 6, 0, z * 6-2), Quaternion.identity);
						GameObject objectD = gm.instantiateObject(dt.doorVisualPrefab, new Vector3(x * 6, 2, z * 6 - 3), Quaternion.identity);

						Door d = objectD.GetComponent<Door>();
						d.x = x * 6;
						d.z = z * 6;
						d.type = toType;
						d.doorMap = this;

						/*
                        int[] k = new int[2];
                        k[0] = x;
                        k[1] = z;
						*/
						hdoorStores[key] = objectD;
					}
				}
			}
			if (type == 3)
			{
				List<int[]> keyList = new List<int[]>(vdoorStores.Keys);

				foreach (var key in keyList)
				{
					if (key[0] == x && key[1] == z)
					{

						Debug.Log("Close the door");

						GameObject old = vdoorStores[key];
						//Destroy(old);
						gm.DestroyObject(old);

						DoorType dt = doorTypes[toType];
						//GameObject objectW = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x * 6-2, 0, z * 6), Quaternion.Euler(0,90,0));
						GameObject objectD = gm.instantiateObject(dt.doorVisualPrefab, new Vector3(x * 6 - 3, 2, z * 6), Quaternion.Euler(0, 90, 0));

						Door d = objectD.GetComponent<Door>();
						d.x = x * 6;
						d.z = z * 6;
						d.type = toType;
						d.doorMap = this;

						/*
                        int[] k = new int[2];
                        k[0] = x;
                        k[1] = z;
						*/
						vdoorStores[key] = objectD;

					}
				}
			}
		}
	}

	void GenerateMapVisual()
	{

		foreach (var hDoor in defaultHorizontalDoors)
		{
			if ((hDoor[0] == 5&&hDoor[1] == 1)||(hDoor[0] == 6&&hDoor[1] == 7))
			{
				DoorType dt = doorTypes[2];
				//GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(hWall[0]*5, 0, hWall[1]*5-2), Quaternion.identity );
				GameObject go = gm.instantiateObject(dt.doorVisualPrefab, new Vector3(hDoor[0] * 6 - 3, 2, hDoor[1] * 6 - 3), Quaternion.identity);

				Door d = go.GetComponent<Door>();
				//Debug.Log(w);
				// Assign the variables as needed
				d.x = hDoor[0] * 6;
				d.z = hDoor[1] * 6;
				d.doorMap = this;
				d.type = 2;
				hdoorStores[hDoor] = go;
			}
			else
			{
				DoorType dt = doorTypes[0];
				//GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(hWall[0]*5, 0, hWall[1]*5-2), Quaternion.identity );
				GameObject go = gm.instantiateObject(dt.doorVisualPrefab, new Vector3(hDoor[0] * 6, 2, hDoor[1] * 6 - 3), Quaternion.identity);

				Door d = go.GetComponent<Door>();
				//Debug.Log(w);
				// Assign the variables as needed
				d.x = hDoor[0] * 6;
				d.z = hDoor[1] * 6;
				d.doorMap = this;
				d.type = 0;
				hdoorStores[hDoor] = go;
			}
			
		}

		foreach (var vDoor in defaultVerticalDoors)
		{
			if ((vDoor[0] == 1&&vDoor[1] == 3)||(vDoor[0] == 9&&vDoor[1] == 3))
			{
				DoorType dt = doorTypes[3];
				Debug.Log(dt);
				//GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(vWall[0]*5-2, 0, vWall[1]*5), Quaternion.Euler(0,90,0) );
				GameObject go = gm.instantiateObject(dt.doorVisualPrefab, new Vector3(vDoor[0] * 6 - 3, 2, vDoor[1] * 6 - 3), Quaternion.Euler(0, 90, 0));

				Door d = go.GetComponent<Door>();
				//Debug.Log(w);
				// Assign the variables as needed
				d.x = vDoor[0] * 6;
				d.z = vDoor[1] * 6;
				d.doorMap = this;
				d.type = 3;

				vdoorStores[vDoor] = go;
			}
			else
			{
				DoorType dt = doorTypes[1];
				Debug.Log(dt);
				//GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(vWall[0]*5-2, 0, vWall[1]*5), Quaternion.Euler(0,90,0) );
				GameObject go = gm.instantiateObject(dt.doorVisualPrefab, new Vector3(vDoor[0] * 6 - 3, 2, vDoor[1] * 6), Quaternion.Euler(0, 90, 0));

				Door d = go.GetComponent<Door>();
				//Debug.Log(w);
				// Assign the variables as needed
				d.x = vDoor[0] * 6;
				d.z = vDoor[1] * 6;
				d.doorMap = this;
				d.type = 1;

				vdoorStores[vDoor] = go;
			}
			
		}

	}


	// Added to interface with FireManager.cs:
	public bool checkIfHDoor(int x, int z)
	{

		List<int[]> keyList = new List<int[]>(hdoorStores.Keys);

		foreach (var key in keyList)
		{
			if (key[0] == x && key[1] == z && hdoorStores[key].GetComponent<Door>() != null&&hdoorStores[key].GetComponent<Door>().type!=4)
			{
				return true;
			}
		}

		return false;
	}

	public bool checkIfOpenHDoor(int x, int z){
		List<int[]> keyList = new List<int[]>(hdoorStores.Keys);

		foreach (var key in keyList)
		{
			if (key[0] == x && key[1] == z && hdoorStores[key].GetComponent<Door>() != null&&hdoorStores[key].GetComponent<Door>().type==2)
			{
				return true;
			}
		}
		return false;
	}

	public bool checkIfVDoor(int x, int z)
	{

		List<int[]> keyList = new List<int[]>(vdoorStores.Keys);

		foreach (var key in keyList)
		{
			if (key[0] == x && key[1] == z && vdoorStores[key].HasComponent<Door>() && vdoorStores[key].GetComponent<Door>().type != 5)
			{
				return true;
			}
		}

		return false;
	}

	public bool checkIfOpenVDoor(int x, int z){
		List<int[]> keyList = new List<int[]>(vdoorStores.Keys);

		foreach (var key in keyList)
		{
			if (key[0] == x && key[1] == z && vdoorStores[key].HasComponent<Door>()&&vdoorStores[key].GetComponent<Door>().type==3)
			{
				return true;
			}
		}
		return false;
	}


}