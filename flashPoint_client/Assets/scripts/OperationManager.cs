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
        int[] key = new int[] { x, z };

        int currentX = gm.tileMap.selectedUnit.currentX;
        int currentZ = gm.tileMap.selectedUnit.currentZ;

        int diffX = Math.Abs(x - currentX);
        int diffZ = Math.Abs(z - currentZ);

        if (diffX + diffZ == 0) //same tile (hazmat, poi)
        {

        }
        else if (diffX + diffZ == 1) // neighbor tile (fire, move)
        {

            // check obstable (wall/closed door)
            if (currentX - x == 0)// same column, check above and below
            {
                if (currentZ < z) // below the target
                {
                    if (gm.wallManager.hwallStores.ContainsKey(key)&&gm.wallManager.hwallStores[key].GetComponent<Wall>().type == 2) // no wall or destroyed wall
                    {
                        if (gm.tileMap.tiles[x, z] >= 1 && gm.tileMap.tiles[x, z] <= 2)
                        {
                            Operation op = new Operation(this, OperationType.ExtingSmoke);
                        }

                        if (gm.tileMap.tiles[x, z] == 2)
                        {
                            Operation op = new Operation(this, OperationType.ExtingFire);
                        }
                    }
                }
            }



            
        }else // not neighboring 
        {

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

