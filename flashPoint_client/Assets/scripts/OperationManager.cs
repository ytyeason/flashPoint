using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using SocketIO;

public class OperationManager : MonoBehaviour
{
    public GameManager gm;

    public List<Button> prefabs= new List<Button>();

    List<Operation> possibleOp = new List<Operation>();

    public GameObject opPanel;

    public List<GameObject> options = new List<GameObject>();

    private int x, z;

    public SocketIOComponent socket;

    public OperationManager(GameManager gm)
    {
        this.gm = gm;
    }

    public void selectTile(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    void getPossibleOps()
    {

        int currentX = gm.tileMap.selectedUnit.currentX;
        int currentZ = gm.tileMap.selectedUnit.currentZ;

        int diffX = Math.Abs(x - currentX);
        int diffZ = Math.Abs(z - currentZ);

        if (diffX + diffZ == 0) //same tile (hazmat, poi)
        {

        }
        else if (diffX + diffZ == 1) // neighbor tile (fire, move)
        {
            Boolean extingFire = false;
            Boolean extingsmoke = false;
            // check obstable (wall/closed door)
            if (currentX - x == 0)// same column, check above and below
            {
                if (currentZ < z) // below the target
                {
                    int[] key = new int[] { x, z };
                    if (gm.wallManager.hwallStores.ContainsKey(key) && gm.wallManager.hwallStores[key].GetComponent<Wall>().type == 2) // no wall or destroyed wall
                    {
                        if (gm.tileMap.tiles[x, z] >= 1 && gm.tileMap.tiles[x, z] <= 2) 
                        {
                            extingsmoke = true;
                        }

                        if (gm.tileMap.tiles[x, z] == 2)
                        {
                            extingFire = true;
                        }
                    }
                }
                else // above the target
                {
                    int[] key = new int[] { currentX, currentZ };
                    if (gm.wallManager.hwallStores.ContainsKey(key) && gm.wallManager.hwallStores[key].GetComponent<Wall>().type == 2) // no wall or destroyed wall
                    {
                        if (gm.tileMap.tiles[x, z] >= 1 && gm.tileMap.tiles[x, z] <= 2)
                        {
                            extingsmoke = true;
                        }

                        if (gm.tileMap.tiles[x, z] == 2)
                        {
                            extingFire = true;
                        }
                    }
                }
            }
            else if (currentZ == z) // same row, check right and left
            {
                if (currentX < x) // left of the target
                {
                    int[] key = new int[] { x, z };
                    if (gm.wallManager.hwallStores.ContainsKey(key) && gm.wallManager.hwallStores[key].GetComponent<Wall>().type == 2) // no wall or destroyed wall
                    {
                        if (gm.tileMap.tiles[x, z] >= 1 && gm.tileMap.tiles[x, z] <= 2)
                        {
                            extingsmoke = true;
                        }

                        if (gm.tileMap.tiles[x, z] == 2)
                        {
                            extingFire = true;
                        }
                    }
                }
                else // right to the target
                {
                    int[] key = new int[] { currentX, currentZ };
                    if (gm.wallManager.hwallStores.ContainsKey(key) && gm.wallManager.hwallStores[key].GetComponent<Wall>().type == 2) // no wall or destroyed wall
                    {
                        if (gm.tileMap.tiles[x, z] >= 1 && gm.tileMap.tiles[x, z] <= 2)
                        {
                            extingsmoke = true;
                        }

                        if (gm.tileMap.tiles[x, z] == 2)
                        {
                            extingFire = true;
                        }
                    }
                }
            }

            // check for AP

            if (gm.tileMap.selectedUnit.role == Role.Paramedic || gm.tileMap.selectedUnit.role == Role.RescueSpec) // paramedic and rescue spec: double AP
            {
                if (extingsmoke&&gm.tileMap.selectedUnit.FreeAP>=2)
                {
                    Operation op = new Operation(this, OperationType.ExtingSmoke);
                    possibleOp.Add(op);
                }
                if (extingFire && gm.tileMap.selectedUnit.FreeAP >= 4)
                {
                    Operation op = new Operation(this, OperationType.ExtingFire);
                    possibleOp.Add(op);
                }
            }
            else if (gm.tileMap.selectedUnit.role == Role.CAFS) // cafs
            {
                if(extingsmoke && (gm.tileMap.selectedUnit.FreeAP+ gm.tileMap.selectedUnit.remainingSpecAp) >= 1)
                {
                    Operation op = new Operation(this, OperationType.ExtingSmoke);
                    possibleOp.Add(op);
                }
                if (extingFire && (gm.tileMap.selectedUnit.FreeAP + gm.tileMap.selectedUnit.remainingSpecAp) >= 2)
                {
                    Operation op = new Operation(this, OperationType.ExtingFire);
                    possibleOp.Add(op);
                }

            }
            else
            {
                if (extingsmoke && gm.tileMap.selectedUnit.FreeAP >= 1)
                {
                    Operation op = new Operation(this, OperationType.ExtingSmoke);
                    possibleOp.Add(op);
                }
                if (extingFire && gm.tileMap.selectedUnit.FreeAP >= 2)
                {
                    Operation op = new Operation(this, OperationType.ExtingFire);
                    possibleOp.Add(op);
                }
            }

            //----for move
            Boolean moveTo = false;
            int[] keyM = new int[2];
            if (currentX == x) // same column
            {
                if (currentZ < z) // below the target
                {
                    keyM[0] = x;
                    keyM[1] = z;
                }
                else // above
                {
                    keyM[0] = currentX;
                    keyM[1] = currentZ;
                }
            }
            else // same row
            {
                if (currentX < x) // left
                {
                    keyM[0] = x;
                    keyM[1] = z;
                }
                else
                {
                    keyM[0] = currentX;
                    keyM[1] = currentZ;
                }
            }
            if (gm.wallManager.hwallStores.ContainsKey(keyM) && gm.wallManager.hwallStores[keyM].GetComponent<Wall>().type == 2) // no wall or destroyed wall
            {
                Fireman fireman = gm.tileMap.selectedUnit;
                if (fireman.role == Role.RescueSpec)
                {
                    if (extingFire && fireman.FreeAP + fireman.remainingSpecAp >= 2)
                    {
                        Operation op = new Operation(this, OperationType.Move);
                        possibleOp.Add(op);
                    }else if (!extingFire && fireman.FreeAP + fireman.remainingSpecAp == 1)
                    {
                        Operation op = new Operation(this, OperationType.Move);
                        possibleOp.Add(op);
                    }
                }
                else
                {
                    if(extingFire && fireman.FreeAP >= 2)
                    {
                        Operation op = new Operation(this, OperationType.Move);
                        possibleOp.Add(op);
                    }else if (!extingFire && fireman.FreeAP == 1)
                    {
                        Operation op = new Operation(this, OperationType.Move);
                        possibleOp.Add(op);
                    }
                }
            }

        }
        else // not neighboring 
        {
            int[] key = new int[] { x, z };
            Fireman fireman = gm.tileMap.selectedUnit;
            if (fireman.role == Role.ImagingTech&&fireman.FreeAP>=1&&gm.pOIManager.placedPOI.ContainsKey(key)&&gm.pOIManager.placedPOI[key].status==POIStatus.Hidden)
            {
                Operation op = new Operation(this, OperationType.Imaging);
                possibleOp.Add(op);
            }

            if (fireman.role == Role.Captain)
            {
                foreach(JSONObject o in gm.players.Values)
                {
                    if (o["Location"] .Equals( x + "," + z))
                    {
                        Operation op = new Operation(this, OperationType.Command);
                        possibleOp.Add(op);
                    }
                }
            }

            if (gm.tileMap.tiles[x, z] == 3)
            {

            }

            if (gm.tileMap.tiles[x, z] == 4)
            {

            }
        }


    }

    void Start()
    {
        opPanel.SetActive(false);


        //-------button click------------
        prefabs[0].onClick.AddListener(move);
        prefabs[1].onClick.AddListener(extingSmoke);
        prefabs[2].onClick.AddListener(extingFire);
        prefabs[3].onClick.AddListener(treat);
        prefabs[4].onClick.AddListener(carryV);
        prefabs[5].onClick.AddListener(leadV);
        prefabs[6].onClick.AddListener(removeHazmat);
        prefabs[7].onClick.AddListener(carryHazmat);
        prefabs[8].onClick.AddListener(command);
        prefabs[9].onClick.AddListener(imaging);
        prefabs[10].onClick.AddListener(drive);
        prefabs[11].onClick.AddListener(remote);
        prefabs[12].onClick.AddListener(ride);
        //prefabs[2].onClick.AddListener(extingFire);
        //prefabs[2].onClick.AddListener(extingFire);
        //prefabs[2].onClick.AddListener(extingFire);
        //prefabs[2].onClick.AddListener(extingFire);
        //prefabs[2].onClick.AddListener(extingFire);
        //prefabs[2].onClick.AddListener(extingFire);

    }



    //----Operations---------------

    public void move()
    {

    }

    public void extingSmoke()
    {

    }

    public void extingFire()
    {

    }

    public void treat()
    {

    }

    public void carryV()
    {

    }

    public void leadV()
    {

    }

    public void removeHazmat()
    {

    }

    public void carryHazmat()
    {

    }

    public void imaging()
    {

    }

    public void command()
    {

    }

    public void drive()
    {

    }

    public void remote()
    {

    }

    public void ride()
    {

    }



    public void cancel()
    {
        opPanel.SetActive(false);
    }
}

