using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;
using System.Linq;
using System;

[Serializable]
public class MyClass
{
    public int level;
    public float timeElapsed;
    public string playerName;
    public Dictionary<int[], int> defaultHorizontalWallsMemo;
}