using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using System.Text.RegularExpressions;
using SocketIO;
using System;

public class POIManager{

    public GameManager gm;

    public Dictionary<int[],POI> placedPOI=new Dictionary<int[],POI>();
    public Dictionary<int[], GameObject> poiLookup = new Dictionary<int[], GameObject>();
    public List<POI> poi = new List<POI>();
    public Dictionary<int[], POI> treated = new Dictionary<int[], POI>();
    public Dictionary<int[], GameObject> treatedLookup = new Dictionary<int[], GameObject>();

    private System.Random rand = new System.Random();
    private float posY = -1;

    public int rescued = 0;
    public int killed = 0;

    public POIManager(GameManager gm)
    {
        this.gm = gm;
        generatePOI();
        replenishPOI();
    }

    public void generatePOI()
    {
        for (int i = 0; i < 10; i++)
        {
            POI p = new POI(POIType.Victim, this);
            poi.Add(p);
        }
        for(int i = 0; i < 5; i++)
        {
            POI p = new POI(POIType.FalseAlarm, this);
            poi.Add(p);
        }
    }

    public void replenishPOI()
    {
        int size = placedPOI.Count;
        for(int i = 0; i < 3 - size; i++)
        {
            int randX = rand.Next(1, 9);
            int randZ = rand.Next(1, 7);
            int[] key = new int[] { randX, randZ };
            while (containsKey(key[0],key[1],placedPOI) || gm.tileMap.tiles[randX, randZ] == 2)
            {
                randX = rand.Next(1, 9);
                randZ = rand.Next(1, 7);
                key[0] = randX;
                key[1] = randZ;
            }
            int randIndex = rand.Next(0, poi.Count);
            POI p = poi[randIndex];
            placedPOI.Add(key,p);
            GameObject go = gm.instantiateObject(p.Prefab, new Vector3((float)((double)randX*6 - 1.5), posY, (float)((double)randZ*6 + 1.5)), Quaternion.identity);
            go.transform.Rotate(90, 0, 0);
            poiLookup.Add(key, go);
            poi.Remove(p);
        }
    }

    public void kill(int x, int z)
    {
        int[] key = new int[] { x, z };
        if(containsKey(key[0], key[1], placedPOI))
        {
            POI p = getPOI(key[0],key[1],placedPOI);
            p.setStatus(POIStatus.Removed);
            killed++;
            Remove(key[0],key[1],placedPOI);
            gm.DestroyObject(getPOIPrefab(key[0],key[1], poiLookup));
            Remove(key[0],key[1],poiLookup);
        }
    }

    public void rescue(int x, int z)
    {
        int[] key = new int[] { x, z };
        if (containsKey(key[0], key[1], placedPOI))
        {
            POI p = getPOI(key[0],key[1],placedPOI);
            p.setStatus(POIStatus.Removed);
            rescued++;
            Remove(key[0],key[1],placedPOI);
            gm.DestroyObject(getPOIPrefab(key[0],key[1], poiLookup));
            Remove(key[0],key[1],poiLookup);
        }
    }

    public void reveal(int x,int z)
    {
        Debug.Log("reveal");
        int[] key = new int[] { x, z };
        POI p = getPOI(key[0],key[1],placedPOI);
        p.setStatus(POIStatus.Revealed);
        gm.DestroyObject(getPOIPrefab(key[0],key[1], poiLookup));
        Remove(key[0],key[1],poiLookup);
        if (p.type == POIType.FalseAlarm)
        {
            Remove(key[0],key[1],placedPOI);
            p.setStatus(POIStatus.Removed);
        }
        else
        {
            GameObject go = gm.instantiateObject(p.Prefab, new Vector3((float)((double)x*6 - 1.5), posY, (float)((double)z*6 + 1.5)), Quaternion.identity);

            go.transform.Rotate(90, 0, 0);
            poiLookup.Add(key, go);
        }

    }

    public void treat(int x, int z)
    {
        int[] key = new int[] { x, z };
        POI p = getPOI(key[0],key[1],placedPOI);
        if (p.type == POIType.Victim)
        {
            p.setStatus(POIStatus.Treated);
            treated.Add(key, p);
        }
        gm.DestroyObject(getPOIPrefab(key[0],key[1], poiLookup));
        Remove(key[0], key[1], poiLookup);
        GameObject go = gm.instantiateObject(p.Prefab, new Vector3((float)((double)x*6 - 1.5), posY, (float)((double)z*6 + 1.5)), Quaternion.identity);
        go.transform.Rotate(90, 0, 0);
        treatedLookup.Add(key, go);
    }

    public void movePOI(int origx, int origz, int newx, int newz)
    {
        POI p = getPOI(origx, origz, placedPOI);
        GameObject obj = getPOIPrefab(origx, origz, poiLookup);
        Remove(origx, origz, gm.pOIManager.placedPOI);
        Remove(origx, origz, gm.pOIManager.poiLookup);
        obj.transform.position = new Vector3(newx, 0.2f, newz);
        int[] key = new int[] { newx, newz };
        placedPOI.Add(key, p);
        poiLookup.Add(key, obj);
    }

    public void moveTreated(int origx, int origz, int newx, int newz)
    {
        POI p = getPOI(origx, origz, treated);
        GameObject obj = getPOIPrefab(origx, origz, treatedLookup);
        Remove(origx, origz, gm.pOIManager.treated);
        Remove(origx, origz, gm.pOIManager.treatedLookup);
        obj.transform.position = new Vector3(newx, 0.2f, newz);
        int[] key = new int[] { newx, newz };
        treated.Add(key, p);
        treatedLookup.Add(key, obj);
    }

    public bool containsKey(int x, int z, Dictionary<int[],POI> list)
    {
        foreach(var key in list.Keys)
        {
            if (key[0] == x && key[1] == z)
            {
                return true;
            }
        }
        return false;
    }

    public bool containsKey(int x, int z, Dictionary<int[], GameObject> list)
    {
        foreach (var key in list.Keys)
        {
            if (key[0] == x && key[1] == z)
            {
                return true;
            }
        }
        return false;
    }

    public POI getPOI(int x, int z, Dictionary<int[],POI> list)
    {
        foreach (var key in list.Keys)
        {
            if (key[0] == x && key[1] == z)
            {
                return list[key];
            }
        }
        return null;
    }

    public GameObject getPOIPrefab(int x, int z, Dictionary<int[], GameObject> list)
    {
        foreach (var key in list.Keys)
        {
            if (key[0] == x && key[1] == z)
            {
                return list[key];
            }
        }
        return null;
    }

    public void Remove(int x, int z, Dictionary<int[], GameObject> list)
    {
        foreach (var key in list.Keys)
        {
            if (key[0] == x && key[1] == z)
            {
                list.Remove(key);
                break;
            }
        }
    }

    public void Remove(int x, int z, Dictionary<int[], POI> list)
    {
        foreach (var key in list.Keys)
        {
            if (key[0] == x && key[1] == z)
            {
                list.Remove(key);
                break;
            }
        }
    }
}
