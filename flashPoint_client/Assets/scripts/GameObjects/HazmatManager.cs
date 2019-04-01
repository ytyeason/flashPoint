using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HazmatManager{
    public GameManager gm;

    public Dictionary<int[], GameObject> lookUp = new Dictionary<int[], GameObject>();

    public Dictionary<int[], Hazmat> placedHazmat=new Dictionary<int[], Hazmat>();

    public Dictionary<int[], Hazmat> placedHotspot=new Dictionary<int[], Hazmat>();

    private System.Random rand = new System.Random();
    private float posY=-10;

    public int numOfHazmat = 0; // default to 0 for Family
    public int additionalHotspot = 0;

    public HazmatManager(GameManager gm){
        this.gm=gm;
        if (Int32.TryParse(StaticInfo.numOfHazmat,out this.numOfHazmat)){

        }


        if (Int32.TryParse(StaticInfo.numOfHotspot, out this.additionalHotspot))
        {

        }

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
            switch (StaticInfo.numberOfPlayer)
            {
                case "3":
                    additionalHotspot += 3;
                    break;
                case "4":
                    additionalHotspot += 4;
                    break;
                case "5":
                    additionalHotspot += 4;
                    break;
                case "6":
                    additionalHotspot += 4;
                    break;
                default:
                    break;
            }
        }
        //initiate();

    }

    void putHazmat(HazmatStatus status=HazmatStatus.Hazmat)
    {
        int randX = rand.Next(1, 9);
        int randZ = rand.Next(1, 7);

        int[] key = new int[] { randX, randZ };

        while (placedHazmat.ContainsKey(key) || placedHotspot.ContainsKey(key)||gm.tileMap.tiles[randX,randZ]==2)
        {
            randX = rand.Next(1, 9);
            randZ = rand.Next(1, 7);

            key = new int[] { randX, randZ };
        }

        Hazmat h = new Hazmat(this,status);
        placedHazmat.Add(key, h);
        GameObject go = gm.instantiateObject(h.prefab, new Vector3((float)(randX*6 + 1.5), posY, (float)(randZ*6 - 1.5)), Quaternion.identity);
        if (h.status == HazmatStatus.Hazmat)
        {
            lookUp.Add(key, go);
        }
    }

    public void explodeHazmat(int x, int z)
    {
        int[] key = new int[] { x, z };
        if (placedHazmat.ContainsKey(key))
        {
            Hazmat h = placedHazmat[key];
            h.setHazmatStatus(HazmatStatus.Hazmat);
            placedHazmat.Remove(key);
            lookUp.Remove(key);
            gm.DestroyObject(lookUp[key]);
            placedHotspot.Add(key, h);
            gm.instantiateObject(h.prefab, new Vector3((float)(x * 6 + 1.5), posY, (float)(z * 6 - 1.5)), Quaternion.identity);
        }
    }

    public void placeHazmat()
    {
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
        gm.instantiateObject(h.prefab, new Vector3((float)(x * 6 + 1.5), posY, (float)(z * 6 - 1.5)), Quaternion.identity);
    }

}