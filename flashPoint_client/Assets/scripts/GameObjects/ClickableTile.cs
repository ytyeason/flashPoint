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
