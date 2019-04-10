using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceTile : MonoBehaviour {

	public bool explored = false;
	public int distFromFireman;

	public DistanceTile(int in_distance)
	{
		distFromFireman = in_distance;
	}
}
