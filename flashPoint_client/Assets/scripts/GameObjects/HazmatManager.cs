using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System;

[Serializable]
public class HazmatManager{
    public GameManager gm;

    public Dictionary<int[], GameObject> lookUp = new Dictionary<int[], GameObject>();

    public Dictionary<int[], Hazmat> placedHazmat=new Dictionary<int[], Hazmat>();

    public Dictionary<int[], Hazmat> placedHotspot=new Dictionary<int[], Hazmat>();

    public Dictionary<int[], Hazmat> movingHazmat = new Dictionary<int[], Hazmat>();
    public Dictionary<int[], GameObject> movingLookup = new Dictionary<int[], GameObject>();

    private System.Random rand = new System.Random();
    private float posY=-1;

    public int numOfHazmat = 0; // default to 0 for Family
    public int additionalHotspot = 0;

    public int removedHazmat = 0;

    public HazmatManager(GameManager gm){
        this.gm=gm;

        //this.additionalHotspot
        switch (StaticInfo.level)
        {
            case "Family":
                numOfHazmat=0;
                additionalHotspot = 0;
                break;
            case "Experienced-Recruit":
                numOfHazmat = 3;
                break;
            case "Experienced-Veteran":
                numOfHazmat = 4;
                additionalHotspot += 3;
                break;
            case "Experienced-Heroic":
                numOfHazmat = 5;
                additionalHotspot += 3;
                break;
        }
        if (StaticInfo.level == "Random")
        {
            if (!Int32.TryParse(StaticInfo.numOfHazmat, out this.numOfHazmat))
            {
                numOfHazmat = Int32.Parse(StaticInfo.numOfHazmat);
            }

            if (!Int32.TryParse(StaticInfo.numOfHotspot, out this.numOfHazmat))
            {
                additionalHotspot += Int32.Parse(StaticInfo.numOfHotspot);
            }

        }

        switch (StaticInfo.numberOfPlayer)
        {
            case "3":
                additionalHotspot += 2;
                break;
            case "4":
                additionalHotspot += 3;
                break;
            case "5":
                additionalHotspot += 3;
                break;
            case "6":
                additionalHotspot += 3;
                break;
            default:
                break;
        }
        //initiate();
        initiateHazmat();
    }

    public void initiateHazmat(){
        placedHazmat=new Dictionary<int[], Hazmat>();
        foreach(var key in lookUp.Keys){
            gm.DestroyObject(lookUp[key]);
        }
        lookUp=new Dictionary<int[], GameObject>();
        gm.initializeHazmat();
        placeHazmat();
    }

    public void refreshHazmat(){
        placedHazmat=new Dictionary<int[], Hazmat>();
        foreach(var key in lookUp.Keys){
            gm.DestroyObject(lookUp[key]);
        }
        lookUp=new Dictionary<int[], GameObject>();
    }

    public void putHazmat(HazmatStatus status=HazmatStatus.Hazmat)
    {
        int randX = rand.Next(1, 9);
        int randZ = rand.Next(1, 7);

        int[] key = new int[] { randX, randZ };

        while (containsKey(key[0],key[1],placedHazmat) || containsKey(key[0],key[1],placedHotspot)||gm.tileMap.tiles[randX,randZ]==2)
        {
            randX = rand.Next(1, 9);
            randZ = rand.Next(1, 7);

            key = new int[] { randX, randZ };
        }

        Hazmat h = new Hazmat(this,status);
        if((HazmatStatus)status==HazmatStatus.Hazmat){
            placedHazmat.Add(key, h);
        }else{
            placedHotspot.Add(key, h);
        }
        GameObject go = gm.instantiateObject(h.prefab, new Vector3((float)(randX*6 + 1.5), posY, (float)(randZ*6 - 1.5)), Quaternion.identity);
        go.transform.Rotate(90, 0, 0);
        if (h.status == HazmatStatus.Hazmat)
        {
            lookUp.Add(key, go);
        }
        gm.AddHazmat(randX,randZ,(int)h.status);
    }

    public void addHazmat(int x, int z, int status){
        int[] key=new int[]{x,z};
        Hazmat h = new Hazmat(this,(HazmatStatus)status);
        if((HazmatStatus)status==HazmatStatus.Hazmat){
            placedHazmat.Add(key, h);
        }else{
            placedHotspot.Add(key, h);
        }
        GameObject go = gm.instantiateObject(h.prefab, new Vector3((float)(x*6 + 1.5), posY, (float)(z*6 - 1.5)), Quaternion.identity);
        go.transform.Rotate(90, 0, 0);
        if (h.status == HazmatStatus.Hazmat)
        {
            lookUp.Add(key, go);
        }
    }

    public void explodeHazmat(int x, int z)
    {
        int[] key = new int[] { x, z };
        if (containsKey(key[0],key[1],placedHazmat))
        {
            Hazmat h = get(key[0],key[1],placedHazmat);
            h.setHazmatStatus(HazmatStatus.Hazmat);
            Remove(key[0],key[1],placedHazmat);
            Remove(key[0],key[1],lookUp);
            gm.DestroyObject(get(key[0],key[1],lookUp));
            placedHotspot.Add(key, h);
            GameObject go=gm.instantiateObject(h.prefab, new Vector3((float)(x * 6 + 1.5), posY, (float)(z * 6 - 1.5)), Quaternion.identity);
            go.transform.Rotate(90, 0, 0);
        }
    }

    public void placeHazmat()
    {
        Debug.Log("placing Hazmat");
        Debug.Log(numOfHazmat);
        Debug.Log(StaticInfo.level);
        for(int i = 0; i < numOfHazmat; i++)
        {
            putHazmat();
        }
        for(int i = 0; i < additionalHotspot; i++)
        {
            putHazmat(HazmatStatus.HotSpot);
        }
    }

    public void explosion(int x, int z)
    {
        int[] key = new int[] { x, z };
        Hazmat h = new Hazmat(this, HazmatStatus.HotSpot);
        placedHotspot.Add(key, h);
        GameObject go=gm.instantiateObject(h.prefab, new Vector3((float)(x * 6 + 1.5), posY, (float)(z * 6 - 1.5)), Quaternion.identity);
        go.transform.Rotate(90, 0, 0);
    }

    public void removeHazmat(int x, int z)
    {
        int[] key = new int[] { x, z };

        if (containsKey(key[0],key[1],placedHazmat))
        {
            Remove(key[0],key[1],placedHazmat);
        }
        gm.DestroyObject(get(key[0],key[1],lookUp));
        removedHazmat++;
    }

    public void moveHazmat(int origx, int origz, int newx, int newz)
    {
        Hazmat p = get(origx, origz, movingHazmat);
        GameObject obj = get(origx, origz, movingLookup);
        Remove(origx, origz, movingHazmat);
        Remove(origx, origz, movingLookup);
        obj.transform.position = new Vector3((float)(newx*6-1.5), posY, (float)(newz*6-1.5));
        int[] key = new int[] { newx, newz };
        movingHazmat.Add(key, p);
        movingLookup.Add(key, obj);
    }

    public void carryHazmat(int x, int z)
    {
        Hazmat p = get(x, z, placedHazmat);
        GameObject obj = get(x, z, lookUp);
        Remove(x, z, placedHazmat);
        Remove(x, z, lookUp);

        int[] key = new int[] { x, z };
        movingHazmat.Add(key, p);
        movingLookup.Add(key, obj);
        obj.transform.position = new Vector3((float)(x * 6 - 1.5), posY, (float)(z * 6 - 1.5));
    }

    public void dropHazmat(int x, int z){
        if(containsKey(x,z,movingHazmat)){
            Hazmat p=get(x,z,movingHazmat);
            GameObject obj=get(x,z,movingLookup);
            Remove(x,z,movingHazmat);
            Remove(x,z,movingLookup);

            int[] key=new int[]{x,z};
            placedHazmat.Add(key,p);
            lookUp.Add(key,obj);
            obj.transform.position=new Vector3((float)(x*6+1.5),posY,(float)(z*6-1.5));
        }
    }

    public bool containsKey(int x, int z, Dictionary<int[], Hazmat> list)
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

    public Hazmat get(int x, int z, Dictionary<int[], Hazmat> list)
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

    public GameObject get(int x, int z, Dictionary<int[], GameObject> list)
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

    public void Remove(int x, int z, Dictionary<int[], Hazmat> list)
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
