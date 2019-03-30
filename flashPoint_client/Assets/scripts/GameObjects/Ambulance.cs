using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambulance : MonoBehaviour {

	// Variables to track our things
	public GameObject v;
	public int x;
	public int z;
	public TileMap map;
	public GameManager gm;
	private float speed = 50.0f;
	//private Transform target;
	private float startTime;
	float wait = 2f;
	float angle = 90.0f;
	float smooth = 5.0f;

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
	public Ambulance(GameObject amb, int in_x, int in_z, GameManager gm)
    {
        this.v=amb;
		this.x = in_x;
		this.z = in_z;
        this.gm = gm;
 
    }

    public void moveToAmbStationTwo()
	{

		
		//s.transform.position = new Vector3(x, 0.2f, z);
		//Debug.Log("x, y is outside: " + checkOutside(x, z));
		float step = speed*Time.deltaTime;
		//v.transform.position = Vector3.MoveTowards(new Vector3(v.transform.position.x, v.transform.position.y, v.transform.position.z), new Vector3(v.transform.position.x, v.transform.position.y, v.transform.position.z+10), step);
		//v.transform.position = Vector3.MoveTowards(new Vector3(v.transform.position.x, v.transform.position.y, v.transform.position.z),  new Vector3(v.transform.position.x, v.transform.position.y, v.transform.position.z+10),  step);
		v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*600);
		v.transform.Rotate(0, -angle, 0);
		System.Threading.Thread.Sleep(500);
		
		//v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*600);



		//v.transform.position = new Vector3(45, 0, 30);

    }
}
