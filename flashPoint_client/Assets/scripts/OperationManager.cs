using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using SocketIO;

public class OperationManager
{
    public GameManager gm;

    public List<Button> prefabs;

    List<Operation> possibleOp = new List<Operation>();
    List<Button> buttons = new List<Button>();

    public GameObject opPanel;

    public List<GameObject> options;

    private int x, z;

    public SocketIOComponent socket;

    public bool inCommand = false;
    public Fireman controlled;
    public int commandMoves = 1;

    public OperationManager(GameManager gm)
    {
        this.gm = gm;

        prefabs = gm.prefabs;
        opPanel = gm.opPanel;
        options = gm.options;

        opPanel.SetActive(false);


    }

    public void selectTile(int x, int z)
    {
        DestroyAll();
        opPanel.SetActive(false);
        Debug.Log(buttons.Count);

        this.x = x;
        this.z = z;

        possibleOp = new List<Operation>();

        getPossibleOps();

        Debug.Log(possibleOp.Count);

        if(possibleOp.Count>0) opPanel.SetActive(true);

        for(int i = 0; i < possibleOp.Count; i++)
        {
            Button newObject = gm.instantiateOp(possibleOp[i].prefab, options[i].transform,true);
            switch (possibleOp[i].type) {
                case OperationType.Move:
                    newObject.onClick.AddListener(move);
                    break;
                case OperationType.Treat:
                    newObject.onClick.AddListener(treat);
                    break;
                case OperationType.CarryV:
                    newObject.onClick.AddListener(carryV);
                    break;
                case OperationType.LeadV:
                    newObject.onClick.AddListener(leadV);
                    break;
                case OperationType.CarryHazmat:
                    newObject.onClick.AddListener(carryHazmat);
                    break;
                case OperationType.RemoveHazmat:
                    newObject.onClick.AddListener(removeHazmat);
                    break;
                case OperationType.Command:
                    newObject.onClick.AddListener(command);
                    break;
                case OperationType.Imaging:
                    newObject.onClick.AddListener(imaging);
                    break;
                case OperationType.ExtingSmoke:
                    newObject.onClick.AddListener(extingSmoke);
                    break;
                case OperationType.ExtingFire:
                    newObject.onClick.AddListener(extingFire);
                    break;
                case OperationType.Drive:
                    newObject.onClick.AddListener(drive);
                    break;
                case OperationType.Remote:
                    newObject.onClick.AddListener(remote);
                    break;
                case OperationType.Ride:
                    newObject.onClick.AddListener(ride);
                    break;
                //case OperationType.StopDrive:
                    //newObject.onClick.AddListener(stopDrive);
                    //break;
                case OperationType.GetOff:
                    newObject.onClick.AddListener(getOff);
                    break;
                case OperationType.DeckGun:
                    newObject.onClick.AddListener(deckGun);
                    break;
                case OperationType.DropV:
                    newObject.onClick.AddListener(dropeV);
                    break;
                case OperationType.DropHazmat:
                    newObject.onClick.AddListener(dropHazmat);
                    break;
                case OperationType.StopCommand:
                    newObject.onClick.AddListener(stopCommand);
                    break;
            }
            buttons.Add(newObject);
        }
    }

    void getPossibleOps()
    {

        int currentX = gm.tileMap.selectedUnit.currentX/6;
        int currentZ = gm.tileMap.selectedUnit.currentZ/6;

        Debug.Log(currentX);
        Debug.Log(currentZ);

        int diffX = Math.Abs(x - currentX);
        int diffZ = Math.Abs(z - currentZ);

        Debug.Log(diffX);
        Debug.Log(diffZ);

        Boolean extingFire = false;
        Boolean extingsmoke = false;

        if (diffX + diffZ == 0) //same tile (hazmat, poi)
        {
            int[] key = new int[] { x, z };
            Fireman fireman = gm.fireman;
            Debug.Log("same place");
            Debug.Log(gm.pOIManager.placedPOI.Count);
            //------ POI---------------
            if (gm.pOIManager.containsKey(key[0], key[1], gm.pOIManager.placedPOI))
            {
                if (!fireman.carryingVictim)
                {
                    Operation op = new Operation(this, OperationType.CarryV);
                    possibleOp.Add(op);
                }
                if (fireman.role == Role.Paramedic && fireman.FreeAP >= 1)
                {
                    Operation op = new Operation(this, OperationType.Treat);
                    possibleOp.Add(op);
                }
            }

            if (gm.pOIManager.containsKey(key[0], key[1], gm.pOIManager.treated))
            {
                Debug.Log("has treated");
                POI p = gm.pOIManager.getPOI(key[0], key[1], gm.pOIManager.treated);
                if (p.status == POIStatus.Treated)
                {
                    Operation op = new Operation(this, OperationType.LeadV);
                    possibleOp.Add(op);
                }
            }

            if (!gm.pOIManager.containsKey(key[0], key[1], gm.pOIManager.placedPOI)&&!gm.pOIManager.containsKey(key[0], key[1], gm.pOIManager.treated))
            {
                if (fireman.carriedPOI != null || fireman.ledPOI != null)
                {
                    Operation op = new Operation(this, OperationType.DropV);
                    possibleOp.Add(op);
                }

            }

            //------Hazmat------------
            if (gm.hazmatManager.containsKey(key[0], key[1], gm.hazmatManager.placedHazmat))
            {
                if (fireman.role == Role.HazmatTech&&fireman.FreeAP>=2)
                {
                    Operation op = new Operation(this, OperationType.RemoveHazmat);
                    possibleOp.Add(op);
                }
                if (!fireman.carryingVictim)
                {
                    Operation op = new Operation(this, OperationType.CarryHazmat);
                    possibleOp.Add(op);
                }
            }

            if(!gm.hazmatManager.containsKey(key[0], key[1], gm.hazmatManager.placedHazmat))
            {
                if (fireman.carriedHazmat != null)
                {
                    Operation op = new Operation(this, OperationType.DropHazmat);
                    possibleOp.Add(op);
                }
            }

            if (gm.tileMap.tiles[x, z] == 3 ) // fire deck gun && ride
            {
                int vx = gm.engine.GetComponent<Engine>().x / 6;
                int vz = gm.engine.GetComponent<Engine>().z / 6;

                if (Math.Abs(currentX - vx) <= 0.5 && (Math.Abs(currentZ - vz) <= 0.5))
                {
                    //if (fireman.driving)
                    //{
                    //    Operation op = new Operation(this, OperationType.StopDrive);
                    //    possibleOp.Add(op);
                    //}

                    int minX=0, maxX = 0;
                    int minZ=0, maxZ = 0;
                    if (x < 5 && z < 4)
                    {
                        minX = 1;
                        maxX = 4;
                        minZ = 1;
                        maxZ = 3;
                    }
                    else if (x >= 5 && z < 4)
                    {
                        minX = 5;
                        maxX = 8;
                        minZ = 1;
                        maxZ = 3;
                    }
                    else if (x >= 5 && z >= 4)
                    {
                        minX = 5;
                        maxX = 8;
                        minZ = 4;
                        maxZ = 6;
                    }
                    else if (x < 5 && z >= 4)
                    {
                        minX = 1;
                        maxX = 4;
                        minZ = 4;
                        maxZ = 6;
                    }
                    bool existsF = false;
                    foreach (JSONObject o in gm.players.Values)
                    {
                        for(int i = minX; i <= maxX; i++)
                        {
                            for(int j = minZ; j <= maxZ; j++)
                            {
                                if (o["Location"].Equals(i * 6 + "," + j * 6))
                                {
                                    existsF = true;
                                    break;
                                }
                            }
                            if (existsF) break;
                        }
                        if (existsF) break;
                    }
                    if (!existsF)
                    {
                        Operation op = new Operation(this, OperationType.DeckGun);
                        possibleOp.Add(op);
                    }

                    if (fireman.riding)
                    {
                        Operation op = new Operation(this, OperationType.GetOff);
                        possibleOp.Add(op);
                    }

                    if (!fireman.riding) // ------------------
                    {
                        Operation op = new Operation(this, OperationType.Ride);
                        possibleOp.Add(op);
                    }
                }
            }

            if(gm.tileMap.tiles[x, z] == 4)
            {
                int vx = gm.ambulance.GetComponent<Ambulance>().x / 6;
                int vz = gm.ambulance.GetComponent<Ambulance>().z / 6;

                if (fireman.FreeAP >= 2 && (Math.Abs(currentX - vx) != 9 && Math.Abs(currentZ - vz) != 7)){
                    Operation op = new Operation(this, OperationType.Remote);
                    possibleOp.Add(op);
                }
                if (fireman.FreeAP >= 4 && (Math.Abs(currentX - vx) == 9 || Math.Abs(currentZ - vz) == 7))
                {
                    Operation op = new Operation(this, OperationType.Remote);
                    possibleOp.Add(op);
                }
            }

            if (inCommand)
            {
                if (x == controlled.currentX && z == controlled.currentZ&&fireman.specialtyAP>0)
                {
                    Operation op = new Operation(this, OperationType.StopCommand);
                    possibleOp.Add(op);
                }
            }
        }
        else if (diffX + diffZ == 1) // neighbor tile (fire, move)
        {
            Debug.Log("neighbor");

            // check obstable (wall/closed door)
            if (currentX - x == 0)// same column, check above and below
            {
                if (currentZ < z) // below the target
                {
                    int[] key = new int[] { x, z };
                    if (!gm.wallManager.checkIfHWall(key[0], key[1])) // no wall or destroyed wall
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
                    if (!gm.wallManager.checkIfHWall(key[0], key[1])) // no wall or destroyed wall
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
                    if (!gm.wallManager.checkIfVWall(key[0], key[1])) // no wall or destroyed wall
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
                    if (!gm.wallManager.checkIfVWall(key[0], key[1])) // no wall or destroyed wall
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
                if (!gm.wallManager.checkIfHWall(keyM[0], keyM[1])) {
                    moveTo = true;
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
                if (!gm.wallManager.checkIfVWall(keyM[0],keyM[1]))
                {

                    foreach(int[] a in gm.wallManager.vwallStores.Keys)
                    {
                        Debug.Log(a[0] + " " + a[1]);
                    }
                    moveTo = true;
                }
            }
            if (gm.tileMap.tiles[x, z] == 2 && (gm.tileMap.selectedUnit.carryingVictim||gm.tileMap.selectedUnit.ledPOI!=null)) moveTo = false;
            //if (gm.tileMap.selectedUnit.driving) moveTo = false;
            if (inCommand)
            {
                int distantX = Math.Abs(controlled.currentX - x);
                int distantZ = Math.Abs(controlled.currentZ - z);
                if (distantX + distantZ != 1)
                {
                    moveTo = false;
                }
            }

            Debug.Log(extingFire);
            Debug.Log(gm.tileMap.selectedUnit.FreeAP);
            Debug.Log(moveTo);
            if (moveTo) {
                Fireman fireman = gm.tileMap.selectedUnit;
                if (fireman.role == Role.RescueSpec)
                {
                    if (extingFire && fireman.FreeAP + fireman.remainingSpecAp >= 2)
                    {
                        Operation op = new Operation(this, OperationType.Move);
                        possibleOp.Add(op);
                    }else if (!extingFire && fireman.FreeAP + fireman.remainingSpecAp >= 1)
                    {
                        Operation op = new Operation(this, OperationType.Move);
                        possibleOp.Add(op);
                    }
                }
                else
                {
                    Debug.Log("else");
                    if(extingFire && fireman.FreeAP >= 2)
                    {
                        Operation op = new Operation(this, OperationType.Move);
                        possibleOp.Add(op);
                    }else if (!extingFire && fireman.FreeAP >= 1)
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
            if (fireman.role == Role.ImagingTech&&fireman.FreeAP>=1&&gm.pOIManager.containsKey(key[0],key[1],gm.pOIManager.placedPOI)&&gm.pOIManager.getPOI(key[0],key[1], gm.pOIManager.placedPOI).status==POIStatus.Hidden)
            {
                Operation op = new Operation(this, OperationType.Imaging);
                possibleOp.Add(op);
            }

            //command
            if (fireman.role == Role.Captain)
            {
                foreach(JSONObject o in gm.players.Values)
                {
                    if (o["Location"] .Equals( x*6 + "," + z*6))
                    {
                        Operation op = new Operation(this, OperationType.Command);
                        possibleOp.Add(op);
                    }
                }
                if (inCommand)
                {
                    int distantX = Math.Abs(controlled.currentX - x);
                    int distantZ = Math.Abs(controlled.currentZ);

                    Boolean moveTo = false;
                    int[] keyM = new int[2];
                    if (distantX + diffZ == 1)
                    {
                        if (controlled.currentX == x) // same column
                        {
                            if (controlled.currentZ < z) // below the target
                            {
                                keyM[0] = x;
                                keyM[1] = z;
                            }
                            else // above
                            {
                                keyM[0] = controlled.currentX;
                                keyM[1] = controlled.currentZ;
                            }
                            if (!gm.wallManager.checkIfHWall(keyM[0], keyM[1]))
                            {
                                moveTo = true;
                            }
                        }
                        else // same row
                        {
                            if (controlled.currentX < x) // left
                            {
                                keyM[0] = x;
                                keyM[1] = z;
                            }
                            else
                            {
                                keyM[0] = controlled.currentX;
                                keyM[1] = controlled.currentZ;
                            }
                            if (!gm.wallManager.checkIfVWall(keyM[0], keyM[1]))
                            {
                                moveTo = true;
                            }
                        }
                        if (gm.tileMap.tiles[x, z] == 2 && controlled.carryingVictim) {
                            moveTo = false;
                        }
                        //if (controlled.driving) moveTo = false;
                        int ap = fireman.remainingSpecAp;
                        int requiredAP = 1;
                        if(gm.tileMap.tiles[x, z] == 2 || controlled.carryingVictim)
                        {
                            requiredAP = 2;
                        }
                        if (controlled.role == Role.CAFS&&commandMoves>0) // not commanded before
                        {
                            if (moveTo&&fireman.specialtyAP>=requiredAP)
                            {
                                Operation op = new Operation(this, OperationType.Move);
                                possibleOp.Add(op);
                            }
                        }
                        else if(controlled.role == Role.RescueSpec|| controlled.role == Role.Paramedic)
                        {
                            if (moveTo && fireman.specialtyAP >= requiredAP * 2)
                            {
                                Operation op = new Operation(this, OperationType.Move);
                                possibleOp.Add(op);
                            }
                        }
                        else
                        {
                            if (moveTo && fireman.specialtyAP >= requiredAP)
                            {
                                Operation op = new Operation(this, OperationType.Move);
                                possibleOp.Add(op);
                            }
                        }

                    }

                }
            }
            

            if (gm.tileMap.tiles[x, z] == 3) // engine
            {
                double vx = (double)gm.enG.x / 6;
                double vz = (double)gm.enG.z / 6;

                Debug.Log(vx);
                Debug.Log(vz);

                if (gm.tileMap.tiles[fireman.currentX/6, fireman.currentZ/6] == 3&&(Math.Abs(currentX-vx)<=0.5&&(Math.Abs(currentZ-vz)<=0.5)))
                {
                    if (fireman.FreeAP >= 2 && (Math.Abs(x - currentX) !=9 && Math.Abs(currentZ - z) !=7 ))
                    {
                        Operation op = new Operation(this, OperationType.Drive);
                        possibleOp.Add(op);
                    }

                    if (fireman.FreeAP >= 4 && (Math.Abs(x - currentX) ==9 || Math.Abs(z - currentZ) ==7))
                    {
                        Operation op = new Operation(this, OperationType.Drive);
                        possibleOp.Add(op);
                    }

                }
            }

            if (gm.tileMap.tiles[x, z] == 4) // ambulance
            {
                int vx = gm.amB.x / 6;
                int vz = gm.amB.z / 6;

                if (fireman.FreeAP >= 2 && (Math.Abs(x - currentX) != 9 && Math.Abs(currentZ - z) != 7))
                {
                    if ((Math.Abs(currentX - vx) <= 0.5 && (Math.Abs(currentZ - vz) <= 0.5)))
                    {
                        Operation op = new Operation(this, OperationType.Drive);
                        possibleOp.Add(op);
                    }

                    Operation op1 = new Operation(this, OperationType.Remote);
                    possibleOp.Add(op1);
                }

                if (fireman.FreeAP >= 4 && (Math.Abs(x - currentX) == 9 || Math.Abs(z - currentZ) == 7))
                {
                    if ((Math.Abs(currentX - vx) <= 0.5 && (Math.Abs(currentZ - vz) <= 0.5)))
                    {
                        Operation op = new Operation(this, OperationType.Drive);
                        possibleOp.Add(op);
                    }
                    Operation op1 = new Operation(this, OperationType.Remote);
                    possibleOp.Add(op1);
                }
            }
        }


    }

    //void Start()
    //{
    //    opPanel.SetActive(false);


    //    //-------button click------------
    //    prefabs[0].onClick.AddListener(move);
    //    prefabs[1].onClick.AddListener(extingSmoke);
    //    prefabs[2].onClick.AddListener(extingFire);
    //    prefabs[3].onClick.AddListener(treat);
    //    prefabs[4].onClick.AddListener(carryV);
    //    prefabs[5].onClick.AddListener(leadV);
    //    prefabs[6].onClick.AddListener(removeHazmat);
    //    prefabs[7].onClick.AddListener(carryHazmat);
    //    prefabs[8].onClick.AddListener(command);
    //    prefabs[9].onClick.AddListener(imaging);
    //    prefabs[10].onClick.AddListener(drive);
    //    prefabs[11].onClick.AddListener(remote);
    //    prefabs[12].onClick.AddListener(ride);
    //    prefabs[13].onClick.AddListener(deckGun);
    //    prefabs[14].onClick.AddListener(stopDrive);
    //    prefabs[15].onClick.AddListener(getOff);
    //    //prefabs[2].onClick.AddListener(extingFire);
    //    //prefabs[2].onClick.AddListener(extingFire);
    //    //prefabs[2].onClick.AddListener(extingFire);

    //}



    //----Operations---------------

    public void move()
    {
        Debug.Log("move");
        Fireman fireman = gm.tileMap.selectedUnit;
        if (inCommand)
        {
            if (controlled.role == Role.CAFS)
            {
                commandMoves -= 1;
            }
            if (controlled.driving)
            {
                controlled.driving = false;
                gm.stopDrive(controlled.name);
            }
            gm.UpdateLocation(x, z, controlled.name);
            int requiredAP = 1;
            if (gm.tileMap.tiles[x, z] == 2)
            {
                requiredAP = 2;
            }
            if (controlled.role == Role.Paramedic || controlled.role == Role.RescueSpec)
            {
                requiredAP *= 2;
            }
            fireman.setSpecAP(fireman.remainingSpecAp - requiredAP);
        }
        else
        {
            if (fireman.driving)
            {
                fireman.driving = false;
                gm.stopDrive(StaticInfo.name);
            }
            fireman.move(x, z);
        }
        opPanel.SetActive(false);
        DestroyAll();

    }

    public void extingSmoke()
    {
        Debug.Log("extinguish smoke");
        Fireman fireman = gm.tileMap.selectedUnit;

        fireman.extingSmoke(x, z);
        opPanel.SetActive(false);
        DestroyAll();
    }

    public void extingFire()
    {
        Debug.Log("extinguish fire");
        Fireman fireman = gm.tileMap.selectedUnit;

        fireman.extingFire(x, z);
        opPanel.SetActive(false);
        DestroyAll();
    }

    public void treat()
    {
        Debug.Log("treat");
        Fireman fireman = gm.tileMap.selectedUnit;

        fireman.treat(x, z);

        opPanel.SetActive(false);
        DestroyAll();
    }

    public void carryV()
    {
        Debug.Log("carryV");
        Fireman fireman = gm.tileMap.selectedUnit;

        fireman.carryV(x, z);

        opPanel.SetActive(false); 
        DestroyAll();
    }

    public void leadV()
    {
        Debug.Log("leadV");
        Fireman fireman = gm.tileMap.selectedUnit;

        fireman.leadV(x, z);

        opPanel.SetActive(false); 
        DestroyAll();
    }

    public void removeHazmat()
    {
        Debug.Log("removeHazmat");
        Fireman fireman = gm.tileMap.selectedUnit;

        fireman.removeHazmet(x, z);

        opPanel.SetActive(false); 
        DestroyAll();
    }

    public void carryHazmat()
    {
        Debug.Log("carryHazmat");
        Fireman fireman = gm.tileMap.selectedUnit;

        fireman.carryHazmat(x, z);

        opPanel.SetActive(false); 
        DestroyAll();
    }

    public void imaging()
    {
        Debug.Log("imaging");
        Fireman fireman = gm.tileMap.selectedUnit;

        fireman.flipPOI(x, z);

        opPanel.SetActive(false); 
        DestroyAll();
    }

    public void command()
    {
        Debug.Log("command");

        this.inCommand = true;
        Role role = Role.None;
        int drive = 0;
        bool carrying = false;
        string name = "";
        foreach (string o in gm.players.Keys)
        {
            if (gm.players[o]["Location"] .Equals( x * 6 + "," + z * 6))
            {
                role = (Role)Int32.Parse(gm.players[o]["Role"].ToString());
                if(!Int32.TryParse(gm.players[o]["Driving"].ToString(),out drive))
                {
                    drive = 0;
                }
                if (gm.players[o]["Carrying"].ToString().Equals( "true"))
                {
                    carrying = false;
                }
                name = o;
            }
        }
        controlled = new Fireman(x * 6, z * 6, role, drive,carrying,name);

        opPanel.SetActive(false); 
        DestroyAll();
    }

    public void drive()
    {
        if ((x==9&&z==4)||(x==9&&z==5)||(x==4&&z==0)||(x==5&&z==0)||(x==0&&z==2)||(x==0&&z==3)||(x==4&&z==7)||(x==5&&z==7))
        {
            gm.startDrive(1);
            Ambulance amb = gm.tileMap.ambulance;
            amb.moveNextStation(x,z);
            opPanel.SetActive(false);
            DestroyAll();
        }
        else
        {
            gm.startDrive(2);
            Engine eng = gm.tileMap.engine;
            eng.moveNextStation(x,z);
            opPanel.SetActive(false);
            DestroyAll();
        }
    }

    public void remote()
    {
        opPanel.SetActive(false); 
        DestroyAll();

    }

    public void ride()
    {
        opPanel.SetActive(false); 
        DestroyAll();
    }

    public void deckGun()
    {
        opPanel.SetActive(false); 
        DestroyAll();
    }

    public void stopDrive()
    {
        opPanel.SetActive(false); 
        DestroyAll();
    }

    public void getOff()
    {
        opPanel.SetActive(false); 
        DestroyAll();
    }

    public void dropeV()
    {

    }

    public void dropHazmat()
    {

    }

    public void stopCommand()
    {
        controlled = null;
        inCommand = false;
    }

    public void cancel()
    {
        opPanel.SetActive(false);
        DestroyAll();
        Debug.Log("cancel");
    }

    void DestroyAll() {
        //opPanel.SetActive(false);
        Debug.Log("destroying");
        //for(int i = 0; i < gm.options.Count; i++) {
        //    foreach(Button b in gm.options[i].GetComponentsInChildren<Button>()) {
        //        gm.DestroyButton(b);
        //        Debug.Log(b);
        //    }
        //}
        gm.DestroyButtons();
        buttons.RemoveRange(0, buttons.Count);
    }
}

