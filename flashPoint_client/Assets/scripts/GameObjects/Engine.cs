using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


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
	//void OnMouseUp()
	//{
	//	if (map.gm.isMyTurn)
	//	{
	//		 int vehicleX = x / 6;
	//		 int vehicleZ = z / 6;

 //           OperationManager om = map.gm.operationManager;

 //           Debug.Log(om);

 //           om.selectTile(vehicleX, vehicleZ);
	//	}
	//	else
 //       {
 //           Debug.Log("Not my turn, dont click");
 //       }
	//}

	public void moveNextStation(int dx, int dz)
	{
		
		//s.transform.position = new Vector3(x, 0.2f, z);
		//Debug.Log("x, y is outside: " + checkOutside(x, z));
		//float step = speed*Time.deltaTime;
		//v.transform.position = Vector3.MoveTowards(new Vector3(v.transform.position.x, v.transform.position.y, v.transform.position.z), new Vector3(v.transform.position.x, v.transform.position.y, v.transform.position.z+10), step);
		//v.transform.position = Vector3.MoveTowards(new Vector3(v.transform.position.x, v.transform.position.y, v.transform.position.z),  new Vector3(v.transform.position.x, v.transform.position.y, v.transform.position.z+10),  step);
		if ((dx==7&&dz==7)||((dx==8&&dz==7)))
		{
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*670);
			if (Math.Abs(dx-x/6)>8||Math.Abs(dz-z/6)>6)
			{
				v.transform.Rotate(0, angle*2, 0);
			}
			else{
			 	v.transform.Rotate(0, angle, 0);
			}
		    //System.Threading.Thread.Sleep(500);
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*950);
			 v.transform.position=new Vector3(45, 0, dz*6);
			 x=45;
			 z=dz*6;

		}
		else if ((dx==0&&dz==5)||((dx==0&&dz==6)))
		{
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*670);
			if (Math.Abs(dx-x/6)>8||Math.Abs(dz-z/6)>6)
			{
				v.transform.Rotate(0, angle*2, 0);
			}
			else{
			 	v.transform.Rotate(0, angle, 0);
			}
			// //System.Threading.Thread.Sleep(500);
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*950);
			v.transform.position=new Vector3(dx*6, 0, 33);
			x=dx*6;
			z=33;
		}
		else if ((dx==1&&dz==0)||((dx==2&&dz==0)))
		{
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*670);
			if (Math.Abs(dx-x/6)>8||Math.Abs(dz-z/6)>6)
			{
				v.transform.Rotate(0, angle*2, 0);
			}
			else{
			 	v.transform.Rotate(0, angle, 0);
			}
			// //System.Threading.Thread.Sleep(500);
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*950);
			v.transform.position=new Vector3(9, 0, dz*6);
			x=9;
			z=dz*6;
		}
		else if((dx==9&&dz==1)||((dx==9&&dz==2)))
		{
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*670);
			if (Math.Abs(dx-x/6)>8||Math.Abs(dz-z/6)>6)
			{
				v.transform.Rotate(0, angle*2, 0);
			}
			else{
			 	v.transform.Rotate(0, angle, 0);
			}
			// //System.Threading.Thread.Sleep(500);
			// v.transform.Translate(new Vector3(0,0,1)*Time.deltaTime*950);
			v.transform.position=new Vector3(dx*6, 0, 9);
			x=dx*6;
			z=9;

		}



		//v.transform.position = new Vector3(45, 0, 30);

    }
}
