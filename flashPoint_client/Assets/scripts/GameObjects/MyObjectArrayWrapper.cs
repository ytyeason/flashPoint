
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MyObjectArrayWrapper
{
    public int[] objects;
    public Dictionary<int[], int> defaultHorizontalWallsMemo;
}