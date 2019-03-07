using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall: MonoBehaviour
{
    private WallStatus status;
    public int x;
    public int z;
    public WallManager wallManager;
    public int id;

    public Wall (int id)
    {
        this.id = id;
    }

    public void SetStatus(WallStatus s)
    {
        status = s;
    }

    public WallStatus GetStatus()
    {
        return status;
    }
    
    void OnMouseUp() {
	
		
        Debug.Log("Clicked x: " + x + ", z: " + z);
    }
}
