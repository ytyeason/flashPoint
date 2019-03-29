using UnityEngine;
using System.Collections;

public class Hazmat : MonoBehaviour
{
    public HazmatManager hazmatManager;
    public int x;
    public int y;

    public HazmatStatus status=HazmatStatus.Hazmat;

    void OnMouseUp(){

        Debug.Log("MOUSE UP HAZMAT");
        
        if(hazmatManager.gm)
            if(status==HazmatStatus.HotSpot){
                return;
            }else{
                
            }

    }
}