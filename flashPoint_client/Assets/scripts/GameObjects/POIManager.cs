using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using System.Text.RegularExpressions;
using SocketIO;
using System;
using System;

[Serializable]
public class POIManager{

    public GameManager gm;

    public Dictionary<int[],POI> placedPOI=new Dictionary<int[],POI>();
    public Dictionary<int[], GameObject> poiLookup = new Dictionary<int[], GameObject>();
    public List<POI> poi = new List<POI>();
    public Dictionary<int[], POI> treated = new Dictionary<int[], POI>();
    public Dictionary<int[], GameObject> treatedLookup = new Dictionary<int[], GameObject>();
    public Dictionary<int[], POI> movingPOI = new Dictionary<int[], POI>();
    public Dictionary<int[], GameObject> movingPOILookup = new Dictionary<int[], GameObject>();
    public Dictionary<int[], POI> movingTreated = new Dictionary<int[], POI>();
    public Dictionary<int[], GameObject> movingTreatedLookup = new Dictionary<int[], GameObject>();

    private System.Random rand = new System.Random();
    private float posY = -1;

    public int rescued = 0;
    public int killed = 0;

    public POIManager(GameManager gm)
    {
        this.gm = gm;
        generatePOI();//still need this when loading
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

    public void initiatePOI(){
        placedPOI=new Dictionary<int[], POI>();
        foreach(var key in poiLookup.Keys){
            gm.DestroyObject(poiLookup[key]);
        }
        poiLookup=new Dictionary<int[], GameObject>();
        gm.initializePOI();
        replenishPOI();
    }

    public void refreshPOI(){
        placedPOI=new Dictionary<int[], POI>();
        foreach(var key in poiLookup.Keys){
            gm.DestroyObject(poiLookup[key]);
        }
        poiLookup=new Dictionary<int[], GameObject>();
    }

    public void replenishPOI()
    {
        int size = placedPOI.Count+movingPOI.Count+movingTreated.Count+treated.Count;

        while(size<3)
        {
            int randX = rand.Next(1, 9);
            int randZ = rand.Next(1, 7);
            int[] key = new int[] { randX, randZ };
            bool reveal=false;
            if(StaticInfo.level.Equals("Family")){
                while (containsKey(key[0],key[1],placedPOI) || containsKey(key[0],key[1],treated)||containsKey(key[0],key[1],movingPOI)||containsKey(key[0],key[1],movingTreated))
                {
                    randX = rand.Next(1, 9);
                    randZ = rand.Next(1, 7);
                    key[0] = randX;
                    key[1] = randZ;
                }
                if(gm.tileMap.tiles[randX,randZ]==1||gm.tileMap.tiles[randX,randZ]==2){
                    gm.tileMap.buildNewTile(randX,randZ,0);
                    gm.UpdateTile(randX,randZ,0);
                }
                foreach(var o in gm.players.Keys){
                    if(gm.players[o]["Location"].Equals("\""+randX*6+","+randZ*6+"\"")){
                        reveal=true;
                        break;
                    }
                }
            }else{
                bool cont=false;
                foreach(var o in gm.players.Keys){
                    if(gm.players[o]["Location"].Equals("\""+randX*6+","+randZ*6+"\"")){
                        cont=true;
                        break;
                    }else{
                        cont=false;
                    }
                }
                Debug.Log("check fireman");
                Debug.Log(cont);
                while (containsKey(key[0],key[1],placedPOI) || containsKey(key[0],key[1],treated)||containsKey(key[0],key[1],movingPOI)||containsKey(key[0],key[1],movingTreated)||gm.tileMap.tiles[randX,randZ]==2||gm.tileMap.tiles[randX,randZ]==1||cont)
                {
                    randX = rand.Next(1, 9);
                    randZ = rand.Next(1, 7);
                    key[0] = randX;
                    key[1] = randZ;
                    foreach(var o in gm.players.Keys){
                        if(gm.players[o]["Location"].Equals("\""+randX*6+","+randZ*6+"\"")){
                            cont=true;
                            break;
                        }else{
                            cont=false;
                        }
                    }
                    Debug.Log("check fireman");
                    Debug.Log(cont);
                }

            }

            int randIndex = rand.Next(0, poi.Count);

            POI p = poi[randIndex];
            placedPOI.Add(key,p);

            GameObject go = gm.instantiateObject(p.Prefab, new Vector3((float)((double)randX*6 - 1.5), posY, (float)((double)randZ*6 + 1.5)), Quaternion.identity);
            go.transform.Rotate(90, 0, 0);
            poiLookup.Add(key, go);
            poi.Remove(p);
            gm.AddPOI(randX, randZ, (int)p.type);
            if(reveal){
                this.reveal(randX,randZ);
                gm.updateRevealPOI(randX,randZ);
            }
            size = placedPOI.Count+movingPOI.Count+movingTreated.Count+treated.Count;
        }
    }

    public void addPOI(int x,int z, int type)
    {
        int[] key = new int[] { x, z };
        POI p0 = null;
        foreach(POI p in poi)
        {
            if (p.type == (POIType)type)
            {
                p0 = p;
            }
        }
        placedPOI.Add(key, p0);
        GameObject go = gm.instantiateObject(p0.Prefab, new Vector3((float)((double)x * 6 - 1.5), posY, (float)((double)z * 6 + 1.5)),Quaternion.identity);
        go.transform.Rotate(90, 0, 0);
        poiLookup.Add(key, go);
        poi.Remove(p0);
    }

    public void kill(int x, int z)
    {
        int[] key = new int[] { x, z };

        if(containsKey(key[0], key[1], placedPOI))
        {
            POI p = getPOI(key[0],key[1],placedPOI);
            p.setStatus(POIStatus.Removed);
            if (p.type != POIType.FalseAlarm)
            {
                killed++;
            }
            Remove(key[0],key[1],placedPOI);
            gm.DestroyObject(getPOIPrefab(key[0],key[1], poiLookup));
            Remove(key[0],key[1],poiLookup);
        }
        if (containsKey(key[0], key[1], movingPOI))
        {
            POI p = getPOI(key[0], key[1], movingPOI);
            p.setStatus(POIStatus.Removed);
            killed++;
            Remove(key[0], key[1], movingPOI);
            gm.DestroyObject(getPOIPrefab(key[0], key[1], movingPOILookup));
            Remove(key[0], key[1], movingPOILookup);
        }

        if (containsKey(key[0], key[1], movingTreated))
        {
            POI p = getPOI(key[0], key[1], movingTreated);
            p.setStatus(POIStatus.Removed);
            killed++;
            Remove(key[0], key[1], movingTreated);
            gm.DestroyObject(getPOIPrefab(key[0], key[1], movingTreatedLookup));
            Remove(key[0], key[1], movingTreatedLookup);
        }
    }

    public void rescueCarried(int x, int z)
    {
        int[] key = new int[] { x, z };
        if (containsKey(key[0], key[1], movingPOI))
        {
            POI p = getPOI(key[0],key[1],movingPOI);
            p.setStatus(POIStatus.Removed);
            rescued++;
            Remove(key[0],key[1],movingPOI);
            gm.DestroyObject(getPOIPrefab(key[0],key[1], movingPOILookup));
            Remove(key[0],key[1],movingPOILookup);
        }
    }

    public void rescueTreated(int x, int z)
    {
        int[] key = new int[] { x, z };
        if (containsKey(key[0], key[1], movingTreated))
        {
            POI p = getPOI(key[0], key[1], movingTreated);
            p.setStatus(POIStatus.Removed);
            rescued++;
            Remove(key[0], key[1], movingTreated);
            gm.DestroyObject(getPOIPrefab(key[0], key[1], movingTreatedLookup));
            Remove(key[0], key[1], movingTreatedLookup);
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
        POI p = getPOI(origx, origz, movingPOI);
        GameObject obj = getPOIPrefab(origx, origz, movingPOILookup);
        Remove(origx, origz, gm.pOIManager.movingPOI);
        Remove(origx, origz, gm.pOIManager.movingPOILookup);
        obj.transform.position = new Vector3((float)(newx*6-1.5), posY, (float)(newz*6-1.5));
        int[] key = new int[] { newx, newz };
        movingPOI.Add(key, p);
        movingPOILookup.Add(key, obj);
    }

    public void moveTreated(int origx, int origz, int newx, int newz)
    {
        POI p = getPOI(origx, origz, movingTreated);
        GameObject obj = getPOIPrefab(origx, origz, movingTreatedLookup);
        Remove(origx, origz, gm.pOIManager.movingTreated);
        Remove(origx, origz, gm.pOIManager.movingTreatedLookup);
        obj.transform.position = new Vector3((float)(newx * 6 + 1.5), posY, (float)(newz * 6 + 1.5));
        int[] key = new int[] { newx, newz };
        movingTreated.Add(key, p);
        movingTreatedLookup.Add(key, obj);
    }

    public void carryPOI(int x,int z)
    {
        POI p = getPOI(x, z, placedPOI);
        GameObject obj = getPOIPrefab(x, z, poiLookup);
        Remove(x, z, gm.pOIManager.placedPOI);
        Remove(x, z, gm.pOIManager.poiLookup);

        int[] key = new int[] { x, z };
        movingPOI.Add(key, p);
        movingPOILookup.Add(key, obj);
        obj.transform.position = new Vector3((float)(x * 6 - 1.5), posY, (float)(z * 6 - 1.5));
    }

    public void leadPOI(int x, int z)
    {
        POI p = getPOI(x, z, treated);
        GameObject obj = getPOIPrefab(x, z, treatedLookup);
        Remove(x, z, gm.pOIManager.treated);
        Remove(x, z, gm.pOIManager.treatedLookup);

        int[] key = new int[] { x, z };
        movingTreated.Add(key, p);
        movingTreatedLookup.Add(key, obj);
        obj.transform.position = new Vector3((float)(x * 6 + 1.5), posY, (float)(z * 6 + 1.5));
    }

    public void dropPOI(int x, int z){
        if(containsKey(x,z,movingPOI)){
            POI p=getPOI(x,z,movingPOI);
            GameObject obj=getPOIPrefab(x,z,movingPOILookup);
            Remove(x,z,movingPOI);
            Remove(x,z,movingPOILookup);

            int[] key=new int[]{x,z};
            placedPOI.Add(key,p);
            poiLookup.Add(key,obj);
            obj.transform.position=new Vector3((float)(x*6-1.5),posY,(float)(z*6+1.5));
        }
        else if(containsKey(x,z,movingTreated)){
            POI p=getPOI(x,z,movingTreated);
            GameObject obj=getPOIPrefab(x,z,movingTreatedLookup);
            Remove(x,z,movingTreated);
            Remove(x,z,movingTreatedLookup);

            int[] key=new int[]{x,z};
            treated.Add(key,p);
            treatedLookup.Add(key,obj);
            obj.transform.position=new Vector3((float)(x*6-1.5),posY,(float)(z*6+1.5));
        }
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
