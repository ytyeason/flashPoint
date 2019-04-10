using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ClickableTile : MonoBehaviour {

    // Variables to track our things
    public int tileX;
    public int tileZ;
    public TileMap map;
    public TileType type;


    // Occurs when we click the mouse:
    void OnMouseUp() {
        Debug.Log("MOUSE UP CLICKABLE TILE");
        if(StaticInfo.StartingPosition){
            int x=tileX/6;
            int z=tileZ/6;
            if(x==0||x==9||z==0||z==7){    // outside tiles
                map.gm.fireman.s.transform.position=new Vector3(tileX,0.2f,tileZ);
            }
        }
        else if(StaticInfo.StartingAmbulancePosition){
            int x = tileX/6;
            int z = tileZ/6;
            if((x==9&&z==3)||(x==9&&z==4)){
                map.ambulance.moveNextStation(9,3);
                // map.gm.UpdateAmbulanceLocation(54,21,-5,-5);
            }
            if((x==4&&z==0)||(x==3&&z==0)){
                map.ambulance.moveNextStation(3,0);
                // map.gm.UpdateAmbulanceLocation(21,0,-5,-5);
            }
            if((x==0&&z==4)||(x==0&&z==3)){
                map.ambulance.moveNextStation(0,3);
                // map.gm.UpdateAmbulanceLocation(0,21,-5,-5);
            }
            if((x==6&&z==7)||(x==5&&z==7)){
                map.ambulance.moveNextStation(5,7);
                // map.gm.UpdateAmbulanceLocation(33,42,-5,-5);
            }
        }
        else if(StaticInfo.StartingEnginePosition){
            int x = tileX/6;
            int z = tileZ/6;
            if((x==7&&z==7)||(x==8&&z==7)){
                map.engine.moveNextStation(7, 7);
                // map.gm.UpdateEngineLocation(45,42,-5,-5);
            }
            if((x==0&&z==5)||(x==0&&z==6)){
                map.engine.moveNextStation(0, 5);
                // map.gm.UpdateEngineLocation(0,33,-5,-5);
            }
            if((x==1&&z==0)||(x==2&&z==0)){
                map.engine.moveNextStation(1, 0);
                // map.gm.UpdateEngineLocation(9,0,-5,-5);
            }
            if((x==9&&z==1)||(x==9&&z==2)){
                map.engine.moveNextStation(9, 1);
                // map.gm.UpdateEngineLocation(9,0,-5,-5);
            }
        }
        else{
            if (map.gm.isMyTurn)
            {
                //Debug.Log(this);

                int x = tileX / 6;
                int z = tileZ / 6;

                OperationManager om = map.gm.operationManager;

                Debug.Log(om);

                om.selectTile(x, z);

                // if (x==9&&z==4)
                // {
                //     List<int[]> keyList = new List<int[]>(vm.vehicleStores.Keys);
                //     foreach (var key in keyList)
                //     {
                //         if (key[0] == 9 && key[1] == 4)
                //         {
                //             Debug.Log("Destroy the old object");

                //             GameObject am = vm.vehicleStores[key]; 
                //             am.transform.Translate(new Vector3(0, 0, 1)*Time.deltaTime);
                //         }
                //     }
                // }

                //if (map.tiles[x, z] != 0&&map.tiles[x, z] != 3&&map.tiles[x, z] != 4)
                //{
                //    // Extinguish the fire or smoke:
                //    int result = map.selectedUnit.extinguishFire(map.tiles[x, z]);
                //    if (result == -1)
                //        Debug.Log("Not enough AP to extinguish the fire");
                //    else
                //    {
                //        map.buildNewTile(x, z, result);
                //        //broadcast new tile
                //        map.gm.UpdateTile(x, z, result);
                //    }

                //}
                //else if (map.tiles[x, z] == 4 && x==9 && z==4)
                //{
                //    map.MoveAmbulanceTo();
                //}
                //else
                //{
                //    // Move to selected tile (only if tile is normal)
                //    map.MoveSelectedUnitTo(tileX, tileZ, 0);
                //}


                //Debug.Log("Clicked x: " + tileX + ", z: " + tileZ);
            }
            else
            {
                Debug.Log("Not my turn, dont click");
            }
        }
        


    }
}
