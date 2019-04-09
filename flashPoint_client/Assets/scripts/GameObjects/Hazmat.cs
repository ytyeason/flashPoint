using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Hazmat
{
    public HazmatManager hm;

    public HazmatStatus status=HazmatStatus.Hazmat;

    public GameObject prefab;



    public Hazmat(HazmatManager hm, HazmatStatus status=HazmatStatus.Hazmat){
        this.hm=hm;
        this.prefab=hm.gm.hazPrefabs[(int)status];
        this.status = status;
    }

    public void setHazmatStatus(HazmatStatus status){
        this.status=status;
        if(status==HazmatStatus.HotSpot){
            prefab=hm.gm.hazPrefabs[1];
        }
    }


}