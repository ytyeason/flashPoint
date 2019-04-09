using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class POI : GameUnit
{
    [SerializeField]
    public POIType type;
    [SerializeField]
    public POIStatus status;

    [SerializeField]
    public POIManager pm;

    [SerializeField]
    public GameObject Prefab;

    public POI(POIType type,POIManager pm)
    {
        this.type = type;
        this.status = POIStatus.Hidden;
        this.pm = pm;
        if (status == POIStatus.Hidden)
        {
            Prefab = pm.gm.poiPrefabs[0];
        }else if (status == POIStatus.Revealed&&type==POIType.Victim)
        {
            Prefab = pm.gm.poiPrefabs[1];
        }else if(status==POIStatus.Treated && type == POIType.Victim)
        {
            Prefab = pm.gm.poiPrefabs[2];
        }
    }

    public void setStatus(POIStatus status)
    {
        this.status = status;
        if (status == POIStatus.Hidden)
        {
            Prefab = pm.gm.poiPrefabs[0];
        }
        else if (status == POIStatus.Revealed && type == POIType.Victim)
        {
            Prefab = pm.gm.poiPrefabs[1];
        }
        else if (status == POIStatus.Treated && type == POIType.Victim)
        {
            Prefab = pm.gm.poiPrefabs[2];
        }
    }
}
