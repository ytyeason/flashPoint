using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ActionManager
{
    public GameManager gm;

    public Dictionary<ActionType, GameObject> prefabs= new Dictionary<ActionType,GameObject>();
}

public class Operation
{

}

public enum ActionType
{
    Move,
    ExtingSmoke,
    ExtingFire,
    Treat,
    CarryV,
    RemoveHazmat,
    CarryHazmat,
    LeadV,
    Command,
    Imaging,
    Drive,
    Remote,
    Ride,

}