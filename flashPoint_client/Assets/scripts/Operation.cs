using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Operation
{
    public OperationType type;
    public OperationManager om;

    public Button prefab;

    public Operation(OperationManager om, OperationType type)
    {
        this.om = om;
        this.type = type;
        this.prefab = om.prefabs[(int)this.type];
    }
}

