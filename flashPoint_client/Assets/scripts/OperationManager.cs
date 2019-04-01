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

    void Start()
    {
        opPanel.SetActive(false);


        //-------button click------------


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
}

