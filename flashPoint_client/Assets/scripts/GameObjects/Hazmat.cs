using UnityEngine;
using System.Collections;

public class Hazmat
{
    public HazmatManager hm;

    public HazmatStatus status=HazmatStatus.Hazmat;

    public GameObject prefab;

    public Hazmat(HazmatManager hm){
        this.hm=hm;
        this.prefab=hm.gm.hazPrefabs[0];
    }

    public void setHazmatStatus(HazmatStatus status){
        this.status=status;
        if(status==HazmatStatus.HotSpot){
            prefab==hm.gm.hazPrefabs[1];
        }
    }

    
}