using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Needed for Unity to know that this data type can be modified, instantiated etc)
[System.Serializable]
public class WallType {
    public string name;

    // Will be used to figure out what material (specifically which 'prefab') will be used in-game
    public GameObject wallVisualPrefab;


}
