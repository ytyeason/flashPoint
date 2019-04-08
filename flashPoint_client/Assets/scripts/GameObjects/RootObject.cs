

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
using System;

[Serializable]
public class RootObject
{
    public List<string> participants { get; set; }
    public string Owner { get; set; }
    public string level { get; set; }
    public string numberOfPlayer { get; set; }
}