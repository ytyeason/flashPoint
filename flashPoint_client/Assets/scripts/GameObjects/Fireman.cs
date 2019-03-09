using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;


public class Fireman
{
	private Game gameInstance;
	private TileMap map;

	// Fireman's location
	private int my_x_coord;
	private int my_y_coord;

	public String name = "undefined";

    public Colors color = Colors.White;	//default to white

    public int AP = 4;					//whatever the initial value is

    public int FreeAP = 4;

	public Boolean isCarrying = false;	// Default is 'not carrying'


    public Fireman(String name, Colors color, Game in_game, int in_x, int in_y)
    {
        this.name = name;
        this.color = color;
		this.gameInstance = in_game;
		this.my_x_coord = in_x;
		this.my_y_coord = in_y;
    }

    public void setAP(int in_ap)
    {
		if (in_ap > 0)
		{
			AP = in_ap;
		}
		else
		{
			Debug.Log("ERROR: " + this + ".setAP(" + in_ap + "): in_ap must be non-negative!");
		}
    }

	// Operation for " Flashpoint::move(destination: Space) "
	public Boolean move( TileMap map, int x_coord, int y_coord )
	{
		// Fireman cannot move
		if (FreeAP <= 0)
		{   // Replace later with UI-functionality/responses
			Debug.Log("ERROR: " + this + ".move(): FreeAP is non-positive!");
			return false;
		}
		// Check if Fireman is carrying & doesn't have enough AP
		else if (isCarrying == true && FreeAP < 2)
		{
			Debug.Log("ERROR: " + this + ".move(): Fireman is carrying untreated victim. Cannot move with FreeAP < 2!");
			return false;
		}
		// Check if not carrying & not enough AP
		else if (isCarrying == false && FreeAP < 1)
		{
			Debug.Log("ERROR: " + this + ".move(): Fireman cannot move with FreeAP < 1!");
			return false;
		}
		// Fireman is able to move; check if space is adjacent to current space
		else
		{
			







			return true;
		}
	}



}
