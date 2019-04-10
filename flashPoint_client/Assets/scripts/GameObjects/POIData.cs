using System;

[Serializable]
public class POIData{
    public POIManager poiManager;

    public POIData(POIManager poiManager){
        this.poiManager=poiManager;
    }

    public void Update(POIManager poiManager){
        this.poiManager=poiManager;
    }
}