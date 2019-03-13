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

    void PopulateDoors()
    {
        defaultHorizontalDoors.Add(new int[] { 3, 1 });
        defaultHorizontalDoors.Add(new int[] { 3, 3 });
        defaultHorizontalDoors.Add(new int[] { 6, 3 });
        defaultHorizontalDoors.Add(new int[] { 5, 4 });
        defaultHorizontalDoors.Add(new int[] { 6, 7 });

        defaultVerticalDoors.Add(new int[] { 1, 4 });
        defaultVerticalDoors.Add(new int[] { 6, 4 });
        defaultVerticalDoors.Add(new int[] { 9, 3 });

    }

    public void ChangeDoor(int x, int z, int toType, int type)
    {
        if (gm.fireman.changeDoor())
        {
            if (type == 0)//we're opening a hwall
            {
                List<int[]> keyList = new List<int[]>(hdoorStores.Keys);

                foreach (var key in keyList)
                {
                    if (key[0] == x && key[1] == z)
                    {
                        Debug.Log("Open the door");
                        gm.sendNotification("Open the door");

                        GameObject old = hdoorStores[key];
                        //Destroy(old);
                        gm.DestroyObject(old);

                        DoorType dt = doorTypes[toType];
                        //GameObject objectW = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x * 5, 0, z * 5-2), Quaternion.identity);
                        GameObject objectD = gm.instantiateObject(dt.doorVisualPrefab, new Vector3(x * 5-2, 0, z * 5 - 2), Quaternion.identity);

                        Door d = objectD.GetComponent<Door>();
                        d.x = x * 5;
                        d.z = z * 5;
                        d.type = toType;
                        d.doorMap = this;

                        int[] k = new int[2];
                        k[0] = x;
                        k[1] = z;
                        hdoorStores[k] = objectD;
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
                        gm.sendNotification("Open the door");

                        GameObject old = vdoorStores[key];
                        //Destroy(old);
                        gm.DestroyObject(old);

                        DoorType dt = doorTypes[toType];
                        //GameObject objectW = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x * 5-2, 0, z * 5), Quaternion.Euler(0,90,0));
                        GameObject objectD = gm.instantiateObject(dt.doorVisualPrefab, new Vector3(x * 5 - 2, 0, z * 5-2), Quaternion.Euler(0, 90, 0));

                        Door d = objectD.GetComponent<Door>();
                        d.x = x * 5;
                        d.z = z * 5;
                        d.type = toType;
                        d.doorMap = this;

                        int[] k = new int[2];
                        k[0] = x;
                        k[1] = z;
                        vdoorStores[k] = objectD;

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
                        //GameObject objectW = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x * 5, 0, z * 5-2), Quaternion.identity);
                        GameObject objectD = gm.instantiateObject(dt.doorVisualPrefab, new Vector3(x * 5, 0, z * 5 - 2), Quaternion.identity);

                        Door d = objectD.GetComponent<Door>();
                        d.x = x * 5;
                        d.z = z * 5;
                        d.type = toType;
                        d.doorMap = this;

                        int[] k = new int[2];
                        k[0] = x;
                        k[1] = z;
                        hdoorStores[k] = objectD;
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
                        //GameObject objectW = (GameObject)Instantiate(wt.wallVisualPrefab, new Vector3(x * 5-2, 0, z * 5), Quaternion.Euler(0,90,0));
                        GameObject objectD = gm.instantiateObject(dt.doorVisualPrefab, new Vector3(x * 5 - 2, 0, z * 5), Quaternion.Euler(0, 90, 0));

                        Door d = objectD.GetComponent<Door>();
                        d.x = x * 5;
                        d.z = z * 5;
                        d.type = toType;
                        d.doorMap = this;

                        int[] k = new int[2];
                        k[0] = x;
                        k[1] = z;
                        vdoorStores[k] = objectD;

                    }
                }
            }
        }
    }

    void GenerateMapVisual()
    {

        foreach (var hDoor in defaultHorizontalDoors)
        {
            DoorType dt = doorTypes[0];
            //GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(hWall[0]*5, 0, hWall[1]*5-2), Quaternion.identity );
            GameObject go = gm.instantiateObject(dt.doorVisualPrefab, new Vector3(hDoor[0] * 5, 0, hDoor[1] * 5 - 2), Quaternion.identity);

            Door d = go.GetComponent<Door>();
            //Debug.Log(w);
            // Assign the variables as needed
            d.x = hDoor[0] * 5;
            d.z = hDoor[1] * 5;
            d.doorMap = this;
            d.type = 0;

            hdoorStores[hDoor] = go;
        }

        foreach (var vDoor in defaultVerticalDoors)
        {
            DoorType dt = doorTypes[1];
            Debug.Log(dt);
            //GameObject go = (GameObject) Instantiate( wt.wallVisualPrefab, new Vector3(vWall[0]*5-2, 0, vWall[1]*5), Quaternion.Euler(0,90,0) );
            GameObject go = gm.instantiateObject(dt.doorVisualPrefab, new Vector3(vDoor[0] * 5 - 2, 0, vDoor[1] * 5), Quaternion.Euler(0, 90, 0));

            Door d = go.GetComponent<Door>();
            //Debug.Log(w);
            // Assign the variables as needed
            d.x = vDoor[0] * 5;
            d.z = vDoor[1] * 5;
            d.doorMap = this;
            d.type = 1;

            vdoorStores[vDoor] = go;
        }

    }


}
