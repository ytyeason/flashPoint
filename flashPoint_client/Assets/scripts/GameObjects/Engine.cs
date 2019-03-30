using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour {

	public GameObject v;
	public int x;
	public int z;
	public TileMap map;
	public GameManager gm;

	// Occurs when we click the mouse:
	void OnMouseUp()
	{
		Debug.Log("MOUSE UP WALL");
		if (map.gm.isMyTurn)
		{
			int vehicleX = x / 5;
			int vehicleZ = z / 5;

		}
	}

	public Engine(GameObject eng, int in_x, int in_z, GameManager gm)
    {
        this.v=eng;
		this.x = in_x;
		this.z = in_z;
        this.gm = gm;
    }

    public void moveToEngStationTwo(GameObject gmo)
	{
		x = 5;
		z = 7;
		//s.transform.position = new Vector3(x, 0.2f, z);
		//Debug.Log("x, y is outside: " + checkOutside(x, z));
		v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime);

    }
}
