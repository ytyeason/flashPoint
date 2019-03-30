using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazmatManager{
    public GameManager gm;

    public Dictionary<int[], GameObject> lookUp = new Dictionary<int[], GameObject>();

    public Dictionary<int[], Hazmat> placedHazmat=new Dictionary<int[], Hazmat>();

    public Dictionary<int[], Hazmat> placedHotspot=new Dictionary<int[], Hazmat>();

    public HazmatManager(GameManager gm){
        this.gm=gm;
    }

    
}