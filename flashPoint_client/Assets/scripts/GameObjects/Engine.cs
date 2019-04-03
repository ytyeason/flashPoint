using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour {

	public GameObject v;
	public int x;
	public int z;
	public TileMap map;
	public GameManager gm;
	public OperationManager om;
	int angle = 90;

		public Engine(GameObject eng, int in_x, int in_z, GameManager gm)
		{
	    this.v=eng;
		this.x = in_x;
		this.z = in_z;
	    this.gm = gm;
		}

	// Occurs when we click the mouse:
	void OnMouseUp()
	{
		if (map.gm.isMyTurn)
		{
			 int vehicleX = x / 6;
			 int vehicleZ = z / 6;

            OperationManager om = map.gm.operationManager;

            Debug.Log(om);

            om.selectTile(vehicleX, vehicleZ);
		}
		else
        {
            Debug.Log("Not my turn, dont click");
        }
	}

	public void moveNextStation(int dx, int dz)
	{
		
		//s.transform.position = new Vector3(x, 0.2f, z);
		//Debug.Log("x, y is outside: " + checkOutside(x, z));
		//float step = speed*Time.deltaTime;
		//v.transform.position = Vector3.MoveTowards(new Vector3(v.transform.position.x, v.transform.position.y, v.transform.position.z), new Vector3(v.transform.position.x, v.transform.position.y, v.transform.position.z+10), step);
		//v.transform.position = Vector3.MoveTowards(new Vector3(v.transform.position.x, v.transform.position.y, v.transform.position.z),  new Vector3(v.transform.position.x, v.transform.position.y, v.transform.position.z+10),  step);
		if ((dx==4&&dz==7)||((dx==5&&dz==7)))
		{
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*670);
			 v.transform.Rotate(0, angle, 0);
		    //System.Threading.Thread.Sleep(500);
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*950);
			 v.transform.position=new Vector3(27, 0, dz*6);
			 x=27;
			 z=dz*6;

		}
		else if ((dx==0&&dz==4)||((dx==0&&dz==5)))
		{
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*670);
			v.transform.Rotate(0, angle, 0);
			// //System.Threading.Thread.Sleep(500);
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*950);
			v.transform.position=new Vector3(dx*6, 0, 27);
			x=dx*6;
			z=27;
		}
		else if ((dx==2&&dz==0)||((dx==3&&dz==0)))
		{
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*670);
			 v.transform.Rotate(0, angle, 0);
			// //System.Threading.Thread.Sleep(500);
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*950);
			v.transform.position=new Vector3(15, 0, dz*6);
			x=15;
			dz=6;
		}
		else if((dx==9&&dz==2)||((dx==9&&dz==3)))
		{
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*670);
		    v.transform.Rotate(0, angle, 0);
			// //System.Threading.Thread.Sleep(500);
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*950);
			v.transform.position=new Vector3(dx*6, 0, 15);
			x=dx*6;
			z=15;

		}



		//v.transform.position = new Vector3(45, 0, 30);

    }
}
