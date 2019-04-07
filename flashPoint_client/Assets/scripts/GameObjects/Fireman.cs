
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;


public class Fireman
{
    public GameObject s;
    public GameObject fireandvictim;
    public TileMap map;
    public TileType[] tileTypes;
    public int currentX;
    public int currentZ;
    public Boolean debugMode = true;        // Toggle this for more descriptive Debug.Log() output
    private static int REFRESH_AP = 4;

    public String name = "eason";

    public Colors color = Colors.White;//default to white

    public int AP = REFRESH_AP; //whatever the initial value is

    public int FreeAP = REFRESH_AP;

    public int savedAP = 0;

    public int specialtyAP = 0;

    public int remainingSpecAp = 0;

    public bool carryingVictim = false;
    public bool leadingVictim=false;
    public POI carriedPOI;
    public Hazmat carriedHazmat;
    public POI ledPOI;

    public bool driving = false;
    public int vehicle = 0;

    public bool riding = false;

    public GameManager gm;

    public POIManager pOIManager;
    public HazmatManager hazamatManager;



    //public int freeExtinguishAp = 3;

    //public int freeMovementAp = 3;

    public Role role = StaticInfo.role;

    public String level = StaticInfo.level;

    public Fireman(String name, Colors color, GameObject s, GameObject firemanplusvictim, int in_x, int in_z, int AP, GameManager gm, Role role, POIManager
    pOIManager, HazmatManager hazmatManager)
    {
        this.name = name;
        this.color = color;
        this.s = s;
        this.fireandvictim = firemanplusvictim;
        this.currentX = in_x;
        this.currentZ = in_z;
        this.gm = gm;
        setRole(role);
        this.FreeAP = AP + savedAP;
        this.remainingSpecAp = this.specialtyAP;

        this.pOIManager = pOIManager;
        this.hazamatManager = hazmatManager;


        //if (string.Equals(level, "Experienced"))
        //{
        //    setRole(role);
        //}
    }

    public Fireman(int in_x, int in_z, Role role, int drive, int ride, bool carrying, bool leading, string name)
    {
        this.currentX = in_x;
        this.currentZ = in_z;
        this.role = role;
        if (drive != 0)
        {
            this.driving = true;
            vehicle = drive;
        }
        if(ride!=0){
            this.riding=true;
            vehicle=ride;
        }
        this.carryingVictim = carrying;
        this.leadingVictim=leading;
        this.name = name;
    }


    public void setAP(int in_ap)
    {
        FreeAP = in_ap;
        gm.displayAP();
    }

    public void setSpecAP(int specAP)
    {
        remainingSpecAp = specAP;
        gm.displayAP();
    }

    // Refresh AP while rolling over unused AP (a maximum of 4)
    public void refreshAP()
    {
        Debug.Log("EOT AP: " + FreeAP);
        int rollover_AP = Math.Min(FreeAP, 4);

        // Had AP from a previous turn
        //if (FreeAP > 4)
        //{
        //  // Stores values between 0 and 4 which are allowed to be rolled-over
        //  rollover_AP = 4;
        //}
        //// No rollover (i.e. FreeAP < 4
        //else
        //{
        //  rollover_AP = FreeAP % 5;
        //}

        // Change AP
        setAP(AP + rollover_AP);
        savedAP = rollover_AP;
        setSpecAP(specialtyAP);
        Debug.Log("Rolling over: " + rollover_AP);
        Debug.Log("Total AP for new turn is: " + FreeAP);
    }


    // Fireman's method call from Door.cs
    public Boolean changeDoor(int doorX, int doorZ)
    {
        if(this.role!=Role.Dog)
        {
            // If debugMode is enabled, better reporting is enabled
            if (debugMode) Debug.Log("Running extuinguishFire(" + doorX + ", " + doorZ + ")");

            // AP check
            if (gm.operationManager.inCommand)
            {
                if (remainingSpecAp >= 1)
                {
                    setSpecAP(remainingSpecAp - 1);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (FreeAP >= 1)
                {
                    setAP(FreeAP - 1);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
        //if (FreeAP < 1)
        //{
        //    if (debugMode) Debug.Log("ToggleDoor() failed, AP unchanged: " + FreeAP);
        //    return false;
        //}
        //// toggleDoor()
        //else
        //{
        //    setAP(FreeAP - 1);
        //    if (debugMode) Debug.Log("Changed (toggleDoor): AP is now: " + FreeAP);
        //    return true;
        //}
    }


    // Pre-condition via ClickableTiles: in_status != 0
    public int extinguishFire(int in_status)
    {
        if(this.role!=Role.Dog)
        {
            if (debugMode) Debug.Log("Running extuinguishFire(" + in_status + ")");

            // AP check
            if (FreeAP < 1)
            {
                if (debugMode) Debug.Log("AP unchanged: " + FreeAP);
                return -1;
            }
            else // Fire -> Smoke || Smoke -> Normal: 1 AP
            {
                setAP(FreeAP - 1);
                if (debugMode) Debug.Log("Changed extFire: AP is now: " + FreeAP);
                return (in_status - 1);
            }
        }
        return -1;
    }

    public Boolean chopWall()
    {
        if(this.role!=Role.Dog)
        {
            if (FreeAP >= 2)
            {
                setAP(FreeAP - 2);
                if (debugMode) Debug.Log("AP is now: " + FreeAP);
                return true;
            }
            else
            {
                if (debugMode) Debug.Log("No AP left to chop the Wall!");
                return false;
            }
        }
        return false;
    }

    public void tryMove(int x, int z, int in_status, GameObject gmo) //int[] ct_key, Dictionary<int[], ClickableTile> ct_table)
    {
        // FreeAP must be positive
        if (FreeAP > 0)
        {
            // Validate tile
            if (x >= 0 && z >= 0)
            {
                if (x == currentX - 6 || x == currentX + 6 || x == currentX)
                {
                    if (z == currentZ - 6 || z == currentZ + 6 || z == currentZ)
                    {
                        //ClickableTile cur_ct = ct_table[ct_key];
                        //Debug.Log("(DEBUG) tryMove(" + x + ", " + z + ")'s spaceState is: " + in_status);


                        // Now that chosen ClickableTile is valid, check AP constraints:
                        if (in_status != 2 && FreeAP >= 1 && !carryingVictim) // Safe
                        {
                            setAP(FreeAP - 1);
                            String condition = (debugMode) ? " - ran with (!CarryVictim, Safe, AP >= 1)" : "";
                            Debug.Log("AP is now: " + FreeAP + condition);
                            move(x, z, gmo);
                            gm.UpdateLocation(x, z,StaticInfo.name);
                        }
                        else if (in_status == 2 && FreeAP >= 2 && !carryingVictim) // Fire
                        {
                            setAP(FreeAP - 2);
                            String condition = (debugMode) ? " - ran with (!CarryVictim, Fire, AP >= 2)" : "";
                            Debug.Log("AP is now: " + FreeAP + condition);
                            move(x, z, gmo);
                            gm.UpdateLocation(x, z,StaticInfo.name);
                        }
                        else if (in_status != 2 && carryingVictim && FreeAP >= 2)
                        {
                            setAP(FreeAP - 2);
                            String condition = (debugMode) ? " - ran with (CarryVictim, !Fire, AP >= 2)" : "";
                            Debug.Log("AP is now: " + FreeAP + condition);
                            move(x, z, gmo);
                            gm.UpdateLocation(x, z,StaticInfo.name);
                        }
                        else if (in_status != 2 && carryingVictim && FreeAP >= 2)
                        {
                            setAP(FreeAP - 2);
                            String condition = (debugMode) ? " - ran with (CarryVictim, !Fire, AP >= 2)" : "";
                            //Debug.Log("AP is now: " + FreeAP + condition);
                            move(x, z, gmo);
                            gm.UpdateLocation(x, z,StaticInfo.name);
                        }
                        else
                        {
                            //Debug.Log("Need more than " + FreeAP + " to move to target tile (" + x + ", " + z +")");
                        }
                    }
                    else
                    {
                        //Debug.Log("MoveSelectedUnitTo(z): Fireman can move at most one z-unit at a time.");
                    }
                }
                else
                {
                    //Debug.Log("MoveSelectedUnitTo(x): Fireman can move at most one x-unit at a time.");

                }
            }
            else
            {
                //Debug.Log("MoveSelectedUnitTo(" + x + ", " + z + "): x & z have to be non-negative.");
            }
        }
        else
        {
            //Debug.Log("Fireman.move(" + x + ", " + z + "): FreeAP must be positive to move.");
        }
    }

    // Check if x & z coordinates place fireman outside
    public bool checkOutside(int x_coord, int z_coord)
    {
        if ((x_coord == 0 || (x_coord / 6) == map.gm.mapSizeX - 1) &&
            (z_coord == 0 || (z_coord / 6) == map.gm.mapSizeZ - 1))
        {
            Debug.Log("x, z : " + x_coord + ", " + z_coord);
            return true;
        }

        return false;
    }


    // Once move is validated the following, unconditionally succesful, move is called
    public void move(int x, int z, GameObject gmo)
    {
        currentX = x;
        currentZ = z;
        s.transform.position = new Vector3(x, 0.2f, z);
        //Debug.Log("x, y is outside: " + checkOutside(x, z));

        //     if(x==5 && z == 5)
        //     {
        ////firemanplusvictim firemanandvictim = new firemanplusvictim(name, FreeAP, color, fireandvictim, currentX, currentZ);
        //Debug.Log("You are now carrying the victim!");
        //carryingVictim = true;
        ////GameManager gmm = new GameManager();
        //    gm.DestroyObject(gmo);

        //    //s = gmm.instantiateObject(firemanandvictim.s, new Vector3(5, 0, 5), Quaternion.identity);
        //}

        // Check if fireman is outside & carrying a victim 
        if (carryingVictim && checkOutside(x, z) == true)
        {
            // Rescue the victim
            gm.rescued_vict_num++;

            Debug.Log("The victim has been rescued!");
        }
    }

    // For operations-----------------------------------------
    public void move(int x, int z)
    {
        int requiredAP = 1;
        if ((this.role==Role.Dog && !carryingVictim && (currentX/6-1==x&&currentZ/6==z&&!gm.wallManager.checkIfVWall_dog(currentX/6,currentZ/6)&&gm.wallManager.checkIfVWall(currentX/6,currentZ/6))
            ||(currentX/6+1==x&&currentZ/6==z&&!gm.wallManager.checkIfVWall_dog(x,z)&&gm.wallManager.checkIfVWall(x,z))||(currentX/6==x&&currentZ/6+1==z&&!gm.wallManager.checkIfHWall_dog(x,z+1)&&gm.wallManager.checkIfHWall(x,z+1))
            ||(this.currentX/6==x&&this.currentZ/6-1==z&&!gm.wallManager.checkIfHWall_dog(x,z)&&gm.wallManager.checkIfHWall(x,z))) || (this.role!=Role.Dog && gm.tileMap.tiles[x, z] == 2) || (this.role!=Role.Dog && carryingVictim))
        {
            requiredAP = 2;

        }
        if (this.role == Role.Dog && this.carryingVictim==true)
        {
            requiredAP = 4;
        } 
        if (this.role == Role.RescueSpec) // Rescue Specialist
        {
            if (remainingSpecAp >= requiredAP)
            {
                setSpecAP(remainingSpecAp - requiredAP);
            }
            else if (remainingSpecAp + FreeAP >= requiredAP)
            {

                setAP(FreeAP - requiredAP + remainingSpecAp);
                setSpecAP(0);
            }
        }
        else
        {
            if (FreeAP >= requiredAP)
            {
                setAP(FreeAP - requiredAP);
            }
        }

        int origX = currentX/6;
        int origZ = currentZ/6;

        currentX = x * 6;
        currentZ = z * 6;
        s.transform.position = new Vector3(currentX, 0.2f, currentZ);
        gm.UpdateLocation(currentX, currentZ,StaticInfo.name);

        Debug.Log(carriedPOI);

        if (carriedPOI != null)
        {
            gm.pOIManager.movePOI(origX, origZ, x, z);
            gm.UpdatePOILocation(origX, origZ, x, z);
        }

        if (ledPOI != null)
        {
            gm.pOIManager.moveTreated(origX, origZ, x, z);
            gm.UpdateTreatedLocation(origX, origZ, x, z);
        }

        if (carriedHazmat != null)
        {
            gm.hazmatManager.moveHazmat(origX, origZ, x, z);
            
        }

        

        int[] key = new int[] { x, z };
        if (gm.pOIManager.containsKey(key[0],key[1],gm.pOIManager.placedPOI) && gm.pOIManager.getPOI(key[0],key[1],gm.pOIManager.placedPOI).status == POIStatus.Hidden)
        {
            gm.pOIManager.reveal(x, z);
            Debug.Log("has poi");
            gm.updateRevealPOI(x, z);
        }
    }



    public void setRole(Role role)
    {
        this.role = role;

        if (role.Equals(Role.Generalist))
        {
            AP = 5;
            // role = "Generalist";
        }

        if (role.Equals(Role.ImagingTech))
        {
            AP = 4;
            // role = "imagingTech";
        }

        if (role.Equals(Role.CAFS))
        {
            AP = 3;
            specialtyAP = 3;
            // role = "CAFSfighter";
        }

        if (role == Role.Captain)
        {
            AP = 4;
            specialtyAP = 2;
        }

        if (role == Role.Paramedic)
        {
            AP = 4;
        }

        if (role == Role.HazmatTech)
        {
            AP = 4;
        }

        if (role == Role.RescueSpec)
        {
            AP = 4;
            specialtyAP = 3;
        }

        if (role == Role.Driver)
        {
            AP = 4;
        }

        if (role == Role.Dog)
        {
            AP = 12;
        }

        if (role == Role.Veteran)
        {
            AP = 4;
        }

        //setAP(AP + savedAP);
        FreeAP = AP + savedAP;
        StaticInfo.role = role;
    }

    public void flipPOI(int x, int z)
    {
        gm.pOIManager.reveal(x, z);
        gm.updateRevealPOI(x, z);
    }


    public void removeHazmet(int x, int z)
    {

        gm.hazmatManager.removeHazmat(x, z);
        gm.RemoveHazmat(x, z);

    }

    public void extingSmoke(int x, int z)
    {
        if (this.role != Role.Dog)
        {
            int requiredAP = 1;
            if (role.Equals(Role.CAFS))
            {
                if (remainingSpecAp >= requiredAP)
                {
                    setSpecAP(remainingSpecAp - requiredAP);
                }
                else
                {
                    setAP(FreeAP - remainingSpecAp);
                    setSpecAP(0);
                }
            }
            else if (role.Equals(Role.RescueSpec)||role==Role.Paramedic)
            {
                requiredAP *= 2;
                if (FreeAP >= requiredAP)
                {
                    setAP(FreeAP - requiredAP);
                }
            }
            else
            {
                setAP(FreeAP - requiredAP);
            }

            if (gm.tileMap.tiles[x, z] == 1)
            {
                gm.tileMap.buildNewTile(x, z, 0);
                gm.UpdateTile(x, z, 0);
            }
            else if (gm.tileMap.tiles[x, z] == 2)
            {
                gm.tileMap.buildNewTile(x, z, 1);
                gm.UpdateTile(x, z, 1);
            }
        }
    }



    public void extingFire(int x, int z)
    {
        if(this.role!=Role.Dog)
        {
            int requiredAP = 2;
            if (role.Equals(Role.CAFS))
            {
                if (remainingSpecAp >= requiredAP)
                {
                    setSpecAP(remainingSpecAp - requiredAP);
                }
                else
                {
                    setAP(FreeAP - remainingSpecAp);
                    setSpecAP(0);
                }
            }

            else if (role.Equals(Role.RescueSpec) || role == Role.Paramedic)
            {
                requiredAP *= 2;
                if (FreeAP >= requiredAP)
                {
                    setAP(FreeAP - requiredAP);
                }
            }
            else
            {
                setAP(FreeAP - requiredAP);
            }
            gm.tileMap.buildNewTile(x, z, 0);
            gm.UpdateTile(x, z, 0);
        }
        
    }

    public void treat(int x, int z)
    {
        if(this.role!=Role.Dog)
        {
            setAP(FreeAP - 1);
            gm.pOIManager.treat(x, z);
        }

    }

    public void carryV(int x, int z)
    {
        
        this.carryingVictim = true;
        this.carriedPOI = gm.pOIManager.getPOI(x, z, gm.pOIManager.placedPOI);
        gm.pOIManager.carryPOI(x, z);
        gm.startCarryV(x,z);
    }

    public void leadV(int x, int z)
    {

        this.ledPOI = gm.pOIManager.getPOI(x, z, gm.pOIManager.treated);
        gm.pOIManager.leadPOI(x, z);
        this.leadingVictim=true;
        gm.startLeadV(x,z);
    }

    public void carryHazmat(int x,int z)
    {
        if(this.role!=Role.Dog)
        {
            this.carriedHazmat = gm.hazmatManager.get(x, z, gm.hazmatManager.placedHazmat);
            this.carryingVictim = true;
            gm.hazmatManager.carryHazmat(x, z);
            gm.startCarryHazmat(x,z);
        }
    }



}
