﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using Newtonsoft.Json;
//using System.Web.Script.Serialization;


public class GameManager: MonoBehaviour
{
    public SocketIOComponent socket;
    public GameObject firemanObject;
    public GameObject firemanplusObject;
    public GameObject ambulance;
    public GameObject engine;
  //  public GameObject clickableVehicle;
    public TileType[] tileTypes;
    public DoorType[] doorTypes;
    public WallType[] wallTypes;
 //   public VehicleType[] vehicleTypes;
    public GameObject[] poiPrefabs;
    public GameObject[] hazPrefabs;
	public int mapSizeX = 10;
	public int mapSizeZ = 8;
	public int damaged_wall_num = 0;
	public int rescued_vict_num = 0;

	public JSONObject game_info = StaticInfo.game_info;

    public WallManager wallManager;
 //   public VehicleManager vehicleManager;
    public TileMap tileMap;
    public DoorManager doorManager;
	public FireManager fireManager;
    public POIManager pOIManager;
    public HazmatManager hazmatManager;
    public OperationManager operationManager;
	public VicinityManager vicinityManager;

    // --op
    public List<Button> prefabs = new List<Button>();
    public GameObject opPanel;

    public List<GameObject> options = new List<GameObject>();

    // change role
    public Dropdown possibleRoles;
    public GameObject selectRolePanel;
    public Text selectedRole;
    public GameObject changeRoleButton;

    //drive
    public int confirmed=0;

	// Dodging
	public bool isDodging = false;
	public bool leftDodgeDown = false;
	public bool upDodgeDown = false;
	public bool rightDodgeDown = false;
	public bool downDodgeDown = false;
	bool canUp = false;
	bool canRight = false;
	bool canDown = false;
	bool canLeft = false;
	bool confirmDodgeDown = false;
	bool wantDodge = false;

	private JSONObject room;
    private JSONObject participants;
    private String level;
    private String numberOfPlayer;
    private String numberOfHazmat;
    private String numberOfHotspot;
    public Dictionary<String, JSONObject> players = new Dictionary<string, JSONObject>();
    public Ambulance amB;
    public Engine enG;
    public Fireman fireman;

    public Boolean isMyTurn = false;

    public List<Notification> chatLog = new List<Notification>();
    public GameObject notificationPanel, notificationText;

    public Text chat;

    public Text nameAP;

    public Text roles;

    public GameObject tooltipPanel;
    public Text tooltip;


    void Start()
    {
        StartCoroutine(ConnectToServer());
        socket.On("LocationUpdate_SUCCESS", LocationUpdate_SUCCESS);
        socket.On("TileUpdate_Success", TileUpdate_Success);
        socket.On("WallUpdate_Success", WallUpdate_Success);
        socket.On("checkingTurn_Success", checkingTurn_Success);
        socket.On("changingTurn_Success", changingTurn_Success);
        socket.On("isMyTurnUpdate", isMyTurnUpdate);
        socket.On("sendChat_Success", sendChat_Success);
        socket.On("DoorUpdate_Success", DoorUpdate_Success);
        socket.On("sendNotification_Success",sendNotification_SUCCESS);
        socket.On("revealPOI_Success", revealPOI_SUCCESS);
        socket.On("treatV_Success", UpdateTreatV_Success);
        socket.On("UpdatePOILocation_Success", UpdatePOILocation_Success);
        socket.On("UpdateAmbulanceLocation_Success", UpdateAmbulanceLocation_Success);
        socket.On("UpdateEngineLocation_Success", UpdateEngineLocation_Success);
        socket.On("AskForRide_Success", AskForRide_Success);
        socket.On("UpdateTreatedLocation_Success", UpdateTreatedLocation_Success);
        socket.On("RemoveHazmat_Success", RemoveHazmat_Success);
        socket.On("UpdateHazmatLocation_Success", UpdateHazmatLocation_Success);
        socket.On("AddPOI_Success", AddPOI_Success);
        socket.On("AddHazmat_Success", AddHazmat_Success);
        socket.On("InitializePOI_Success", initializePOI_Success);
        socket.On("InitializeHazmat_Success", initializeHazmat_Success);
        socket.On("StartCarryV_Success", StartCarryV_Success);
        socket.On("StartLeadV_Success", StartLeadV_Success);
        socket.On("StartCarryHazmat_Success",StartCarryHazmat_Success);
        socket.On("ConfirmRide", ConfirmRide);
        socket.On("StopDrive_Success",stopDrive_Success);
        socket.On("StopRide_Success",StopRide_Success);
        socket.On("StopCarry_Success",StopCarry_Success);
        socket.On("StopLead_Success",StopLead_Success);
        socket.On("changeRole_Success",changeRole_Success);
        socket.On("RescueCarried_Success",rescueCarried_Success);
        socket.On("RescueTreated_Success",rescueTreated_Success);
        socket.On("victory_Success",victory_Success);
        socket.On("defeat_Success",defeat_Success);

        if (game_info != null)
        {
            room = game_info[StaticInfo.roomNumber];
            participants = room["participants"];
            level = room["level"].ToString();
            numberOfPlayer = room["numberOfPlayer"].ToString();
            numberOfHazmat=room["numberOfHazmat"].ToString();
            numberOfHotspot=room["numberOfHotspot"].ToString();

            List<string> p = participants.keys;
            foreach (var v in p)
            {
                //Debug.Log(participants[v]);
                var o = participants[v];
                players[v] = o;
                Debug.Log("debug location");
                Debug.Log(players[v]["Location"]);
            }
        }

        fireman = initializeFireman();
        amB = initializeAmbulance();
        enG = initializeEngine();
        operationManager = new OperationManager(this);
        wallManager = new WallManager(wallTypes,this);
        doorManager = new DoorManager(doorTypes,this);
    //    vehicleManager = new VehicleManager(vehicleTypes,this);
        tileMap = new TileMap(tileTypes,this, fireman, enG, amB);
		fireManager = new FireManager(this, tileMap, mapSizeX, mapSizeZ);
        pOIManager = new POIManager(this);
        hazmatManager=new HazmatManager(this);
		vicinityManager = new VicinityManager(this, tileMap.tiles);

        //displayAP(Convert.ToInt32(players[StaticInfo.name]["AP"].ToString()),fireman.remainingSpecAp);
        displayAP();
     //   vehicleManager.StartvehicleManager();

        tileMap.GenerateFiremanVisual(players);
        registerNewFireman(fireman);
        checkTurn();	//initialize isMyTurn variable at start
        Debug.Log(level);
        roles.text="";
        if (!level.Equals("\"Family\"")||(StaticInfo.level!=null)&&!StaticInfo.level.Equals("Family"))
        {
            displayRole();
            changeRoleButton.SetActive(true);
        }
        else
        {
            changeRoleButton.SetActive(false);
            ambulance.SetActive(false);
            engine.SetActive(false);
        }

        Debug.Log("staticinfo "+StaticInfo.numberOfPlayer);
        selectRolePanel.SetActive(false);

    }

    public void displayAP(){
        Debug.Log(fireman.role.ToString());
        nameAP.text= StaticInfo.name + " : " + fireman.FreeAP + " AP" ;
        if (fireman.role == Role.Captain)
        {
            nameAP.text += "\n"+ nameLengthSpace() + fireman.remainingSpecAp + " Command AP";
        }
        if (fireman.role == Role.CAFS)
        {
            nameAP.text += "\n" + nameLengthSpace() + fireman.remainingSpecAp + " Extinguish AP";
        }
        if (fireman.role == Role.RescueSpec)
        {
            nameAP.text += "\n" + nameLengthSpace() + fireman.remainingSpecAp + " Movement AP";
        }
    }

    public void displayRole()
    {
        Debug.Log("staticinfo "+StaticInfo.name);
        if(StaticInfo.level.Equals("Family")){
            return;
        }
        Debug.Log("displaying role");
        roles.text = StaticInfo.name+": "+roleToString(StaticInfo.role);
        if (players != null)
        {
            foreach (string name in players.Keys)
            {
                if(!name.Equals(StaticInfo.name)) roles.text += "\n" + name + ": " + roleToString((Role)Int32.Parse(players[name].ToDictionary()["Role"]));
            }
        }

    }

    public string roleToString(Role role)
    {
        switch (role)
        {
            case Role.Paramedic:
                return "Paramedic";
            case Role.Captain:
                return "Fire Captain";
            case Role.ImagingTech:
                return "Imaging Technician";
            case Role.CAFS:
                return "CAFS Firefighter";
            case Role.HazmatTech:
                return "Hazmat Technician";
            case Role.Generalist:
                return "Generalist";
            case Role.RescueSpec:
                return "Rescue Specialist";
            case Role.Driver:
                return "Driver";
            case Role.Veteran:
                return "Veteran";
            case Role.Dog:
                return "Rescue Dog";
            default:
                return "";
        }
    }

    String nameLengthSpace()
    {
        string s = "";

        for(int i = 0; i < StaticInfo.name.Length;i++)
        {
            s += " ";
        }
        return s;
    }

    void revealPOI_SUCCESS(SocketIOEvent obj)
    {
        Debug.Log("reveal POI successful");
        var x = Convert.ToInt32(obj.data.ToDictionary()["x"]);
        var z = Convert.ToInt32(obj.data.ToDictionary()["z"]);

        pOIManager.reveal(x, z);
    }


    void WallUpdate_Success(SocketIOEvent obj)
    {
        Debug.Log("wall update successful");
        var x = Convert.ToInt32(obj.data.ToDictionary()["x"]);
        var z = Convert.ToInt32(obj.data.ToDictionary()["z"]);
        var type = Convert.ToInt32(obj.data.ToDictionary()["type"]);
        var horizontal = Convert.ToInt32(obj.data.ToDictionary()["horizontal"]);

        //Debug.Log(x);
        //Debug.Log(z);
        //Debug.Log(type);
        Debug.Log(obj.data);
        Debug.Log(obj.data.ToDictionary()["x"]);
        Debug.Log(obj.data.ToDictionary()["z"]);
        Debug.Log(obj.data.ToDictionary()["type"]);
        Debug.Log(obj.data.ToDictionary()["horizontal"]);

		// Bottom is temporarily commented out:
		wallManager.BreakWall(x, z, type, horizontal, false);
	}

	void DoorUpdate_Success(SocketIOEvent obj)
    {
        Debug.Log("door update successful");
        var x = Convert.ToInt32(obj.data.ToDictionary()["x"]);
        var z = Convert.ToInt32(obj.data.ToDictionary()["z"]);
        var type = Convert.ToInt32(obj.data.ToDictionary()["type"]);
        var toType = Convert.ToInt32(obj.data.ToDictionary()["toType"]);

        //Debug.Log(x);
        //Debug.Log(z);
        //Debug.Log(type);
        Debug.Log(obj.data);
        Debug.Log(obj.data.ToDictionary()["x"]);
        Debug.Log(obj.data.ToDictionary()["z"]);
        Debug.Log(obj.data.ToDictionary()["type"]);
        Debug.Log(obj.data.ToDictionary()["toType"]);

        doorManager.ChangeDoor(x, z, toType, type);
    }

    void TileUpdate_Success(SocketIOEvent obj)
    {
        Debug.Log("tile update successful");
        var x = Convert.ToInt32(obj.data.ToDictionary()["x"]);
        var z = Convert.ToInt32(obj.data.ToDictionary()["z"]);
        var type = Convert.ToInt32(obj.data.ToDictionary()["type"]);

        //Debug.Log(x);
        //Debug.Log(z);
        //Debug.Log(type);
        Debug.Log(obj.data.ToDictionary()["x"]);
        Debug.Log(obj.data.ToDictionary()["z"]);
        Debug.Log(obj.data.ToDictionary()["type"]);

		// Bottom is temporarily commented out:
		tileMap.buildNewTile(x, z,type);
    }

    void LocationUpdate_SUCCESS(SocketIOEvent obj)
    {
        Debug.Log("Location update successful");

        //update with latest objects
        room = obj.data[StaticInfo.roomNumber];
        participants = room["participants"];
        level = room["level"].ToString();
        numberOfPlayer = room["numberOfPlayer"].ToString();

        List<string> p = participants.keys;
        foreach (var v in p)
        {
            var o = participants[v];
            players[v] = o;
            Debug.Log(v);
            Debug.Log(players[v]);
        }
        tileMap.UpdateFiremanVisual(players);
    }

//for vehicles
    public void UpdateAmbulanceLocation(int newx,int newz, int origx, int origz)
    {
        Dictionary<String, string> location = new Dictionary<string, string>();
        location["newx"] = newx.ToString();
        location["newz"] = newz.ToString();
        location["origx"]=origx.ToString();
        location["origz"]=origz.ToString();
        location["room"]=StaticInfo.roomNumber;

        socket.Emit("UpdateAmbulanceLocation", new JSONObject(location));
        Debug.Log("update ambulance location");

    }

    public void AskForRide(int origx, int origz)
    {
        Dictionary<String, string> location = new Dictionary<string, string>();
        location["origx"]=origx.ToString();
        location["origz"]=origz.ToString();
        location["room"]=StaticInfo.roomNumber;
        location["name"]=StaticInfo.name;
        socket.Emit("AskForRide", new JSONObject(location));
    }

    public void AskForRide_Success(SocketIOEvent obj){
        Debug.Log("AskForRide_Success");
        // Debug.Log(obj.data["targetNames"]);
        List<string> names=parseJsonArray(obj.data["targetNames"]);
        Debug.Log("names count:" + names.Count);
        foreach(string n in names){
            if (n.Equals(StaticInfo.name)&& StaticInfo.level!="Family"){
                Debug.Log("same name");
                opPanel.SetActive(true);
                Operation op = new Operation(operationManager, OperationType.Ride);
                Button newObject = this.instantiateOp(op.prefab, options[0].transform, true);
                newObject.onClick.AddListener(ride);
                operationManager.askingForRide=true;
            }
        }
        Debug.Log("ask for ride succeed!!!!!!!");
        this.confirmed=Int32.Parse(StaticInfo.numberOfPlayer)-names.Count;

        Debug.Log("confirmed:" + this.confirmed);

    }

    void ride(){
        operationManager.ride();
    }

    public void startRide(int type){
        Dictionary<string,string> ride=new Dictionary<string,string>();
        ride["name"]=StaticInfo.name;
        ride["room"]=StaticInfo.roomNumber;
        ride["type"]=type.ToString();

        socket.Emit("StartRide",new JSONObject(ride));
    }

    public void ConfirmRide(SocketIOEvent obj){
        if(obj.data.ToString().Equals("true")){

            confirmed++;
        }
        operationManager.askingForRide=false;

    }

    public void UpdateAmbulanceLocation_Success(SocketIOEvent obj)
    {
        Debug.Log("hello,update ambulance");
        int newx = Convert.ToInt32(obj.data.ToDictionary()["newx"]);
        int newz = Convert.ToInt32(obj.data.ToDictionary()["newz"]);

        List<string> names=parseJsonArray(obj.data["names"]);
        foreach(var name in names){
            if(name.Equals(StaticInfo.name)){
                fireman.s.transform.position=new Vector3(newx*6,0.2f,newz*6);
            }
        }

        tileMap.ambulance.moveNextStation(newx/6, newz/6);
        confirmed=0;
    }

    public void UpdateEngineLocation(int newx,int newz, int origx, int origz)
    {
        Dictionary<String, string> location = new Dictionary<string, string>();
        location["newx"] = newx.ToString();
        location["newz"] = newz.ToString();
        location["origx"]=origx.ToString();
        location["origz"]=origz.ToString();
        location["room"]=StaticInfo.roomNumber;

        socket.Emit("UpdateEngineLocation", new JSONObject(location));
        Debug.Log("update eng location");
        
    }
    
    public void UpdateEngineLocation_Success(SocketIOEvent obj)
    {
        int newx = Convert.ToInt32(obj.data.ToDictionary()["newx"]);
        int newz = Convert.ToInt32(obj.data.ToDictionary()["newz"]);

        List<string> names=parseJsonArray(obj.data["names"]);
        foreach(var name in names){
            if(name.Equals(StaticInfo.name)){
                fireman.s.transform.position=new Vector3(newx*6,0.2f,newz*6);
            }
        }

        tileMap.engine.moveNextStation(newx/6, newz/6);
        confirmed=0;
    }


    void checkingTurn_Success(SocketIOEvent obj)
    {
        //accept value here
        var result = obj.data.ToDictionary()["status"];
        Debug.Log(result);

        if (result.Equals("True"))
        {
            isMyTurn = true;
        }
        else
        {
            isMyTurn = false;
        }
    }



    void changingTurn_Success(SocketIOEvent obj)
    {
        Debug.Log("in changingTurn_Success");
        var name = obj.data.ToDictionary()["Turn"];
        Debug.Log(name);

        if (name.Equals(StaticInfo.name))
        {
            isMyTurn = true;
			Debug.Log("It is now your turn! Refreshing AP");
			fireman.refreshAP();
		}
        else
        {
            isMyTurn = false;
			Debug.Log("It is now someone else's turn!");
            fireman.refreshAP();
		}

    }

    void isMyTurnUpdate(SocketIOEvent obj)
    {
        Debug.Log("in isMyTurnUpdate");
        var name = obj.data.ToDictionary()["Turn"];
        Debug.Log(name);

        if (name.Equals(StaticInfo.name))
        {
            isMyTurn = true;
            sendNotification(". It's your turn.");
            // fireman.refreshAP();
        }
        else
        {
            isMyTurn = false;
        }
    }

    void sendChat_Success(SocketIOEvent obj)
    {
        Debug.Log("in sendChat_Success");

        var name = obj.data.ToDictionary()["name"];
        var chat = obj.data.ToDictionary()["chat"];

        var chatString = name + " : " + chat;
        if(chatLog.Count>10){
            Destroy(chatLog[0].textObject.gameObject);
            chatLog.Remove(chatLog[0]);
        }

        Notification notification=new Notification();
        notification.msg=chatString;
        GameObject newText=Instantiate(notificationText,notificationPanel.transform);
        notification.textObject=newText.GetComponent<Text>();
        notification.textObject.text=notification.msg;

        chatLog.Add(notification);
        Debug.Log(chatString);
        // chatLog.Add(chatString);
    }

    IEnumerator ConnectToServer()
    {
        yield return new WaitForSeconds(0.5f);

        socket.Emit("USER_CONNECT");

        yield return new WaitForSeconds(0.5f);

    }

    public Fireman initializeFireman()
    {
        var location = players[StaticInfo.name]["Location"].ToString();
        location = location.Substring(1, location.Length - 2);
        //Debug.Log(location);

        var cord = location.Split(',');
        int x = Convert.ToInt32(cord[0]);
        int z = Convert.ToInt32(cord[1]);

        int ap = Convert.ToInt32(players[StaticInfo.name]["AP"].ToString());
		Debug.Log("Created '" + StaticInfo.name + "' with AP =" + ap);
        Fireman f = new Fireman(StaticInfo.name, Colors.Blue, firemanObject, firemanplusObject, x, z, ap, this, StaticInfo.role,pOIManager, hazmatManager);

        return f;
    }

    public Ambulance initializeAmbulance()
    {
        Ambulance amb = new Ambulance(ambulance, 54, 21, this);

        return amb;
    }

    public Engine initializeEngine()
    {
        Engine eng = new Engine(engine, 0, 33, this);

        return eng;
    }

    public void registerNewFireman(Fireman f)
    {
        Debug.Log("let other user know a new fireman has been created");
        UpdateLocation(f.currentX, f.currentZ, StaticInfo.name);//let other user know a new fireman has been created
    }

    public GameObject instantiateObject(GameObject w, Vector3 v, Quaternion q)
    {
        GameObject objectW = (GameObject)Instantiate(w, v, q);
        return objectW;
    }

    public Button instantiateOp(Button w, Transform parent, bool b)
    {
        //Button objectW = (Button)Instantiate(w, parent, b);
        Button button = Instantiate(w, parent.position, parent.rotation, parent);
        return button;
    }

    public void DestroyButtons()
    {
        for(int i = 0; i < options.Count; i++)
        {
            for(int j = 0; j < options[i].transform.childCount; j++)
            {
                Destroy(options[i].transform.GetChild(j).gameObject);
            }
        }
    }

    public void cancel()
    {
        Debug.Log("cancel");
        operationManager.cancel();
    }

    public void DestroyObject(GameObject w)
    {
        Destroy(w);
    }

    public void UpdateLocation(int x, int z, string name)
    {
        Debug.Log("Update Location");
        StaticInfo.Location = new int[] { x, z };
        Dictionary<String, String> update = new Dictionary<string, string>();
        update["room"] = StaticInfo.roomNumber;
        update["name"] = StaticInfo.name;
        if (!name.Equals(StaticInfo.name)) update["name"] = name;
        update["Location"] = StaticInfo.Location[0] + "," + StaticInfo.Location[1];
        update["role"] = ((int)StaticInfo.role).ToString();

        socket.Emit("Location", new JSONObject(update));
    }

    public void UpdateTile(int x, int z, int type)
    {
        Debug.Log("Update tile");
        Dictionary<String, string> updateTile = new Dictionary<string, string>();
        updateTile["x"] = x.ToString();
        updateTile["z"] = z.ToString();
        updateTile["type"] = type.ToString();

        socket.Emit("UpdateTile", new JSONObject(updateTile));
    }

    public void UpdateWall(int x, int z, int type, int horizontal)
    {
        Debug.Log("Update wall");
        Dictionary<String, string> updateWall = new Dictionary<string, string>();
        updateWall["x"] = x.ToString();
        updateWall["z"] = z.ToString();
        updateWall["type"] = type.ToString();
        updateWall["horizontal"] = horizontal.ToString();

        socket.Emit("UpdateWall", new JSONObject(updateWall));
    }

    public void UpdateDoor(int x, int z, int toType, int type)
    {
        Debug.Log("Update door");
        Dictionary<String, string> updateDoor = new Dictionary<string, string>();
        updateDoor["x"] = x.ToString();
        updateDoor["z"] = z.ToString();
        updateDoor["toType"] = toType.ToString();
        updateDoor["type"] = type.ToString();

        socket.Emit("UpdateDoor", new JSONObject(updateDoor));
    }


    public void updateRevealPOI(int x,int z)
    {
        Debug.Log("update after reveal POI");
        Dictionary<String, string> revealPOI = new Dictionary<string, string>();
        revealPOI["x"] = x.ToString();
        revealPOI["z"] = z.ToString();

        socket.Emit("RevealPOI", new JSONObject(revealPOI));
    }


	// ---------------
	// --	DODGE	--
	// ---------------

	// Next 4 are checks for dodging:
	public void leftDodge()
	{
		if (isDodging && canLeft)
		{
			Debug.Log("LEFT DODGE");
			leftDodgeDown = true;
		}
		else leftDodgeDown = false;
	}
	public void upDodge()
	{
		if (isDodging && canUp)
		{
			Debug.Log("UP DODGE");
			upDodgeDown = true;
		}
		else upDodgeDown = false;
	}
	public void rightDodge()
	{
		if (isDodging && canRight)
		{
			Debug.Log("RIGHT DODGE");
			rightDodgeDown = true;
		}
		else rightDodgeDown = false;
	}
	public void downDodge()
	{
		if (isDodging && canDown)
		{
			Debug.Log("DOWN DODGE");
			downDodgeDown = true;
		}
		else downDodgeDown = false;
	}
	
	// Called after dodge decisions are made to reset global variables
	public void resetDodge()
	{
		downDodgeDown = false;
		rightDodgeDown = false;
		upDodgeDown = false;
		leftDodgeDown = false;
		isDodging = false;
		confirmDodgeDown = false;
	}

	// Used for a WaitUntil coroutine
	public bool buttonDown()
	{
		return (leftDodgeDown || upDodgeDown || rightDodgeDown || downDodgeDown);
	}

	// Check if player can dodge in a direction
	public bool canDodge(int in_x, int in_z)
	{
		// Can dodge up
		if (tileMap.tiles[in_x, in_z + 1] != 2)
		{
			Debug.Log("No fire above!");

			// Check if a door is between fireman and safety. If its open, allow dodge
			if (doorManager.checkIfHDoor(in_x, in_z + 1))
			{
				if (doorManager.checkIfOpenHDoor(in_x, in_z + 1))
				{
					canUp = true;
				}
				else canUp = false;
			}
			// If an unbroken wall is between fireman and safety then they cannot dodge that way
			else if (wallManager.checkIfHWall(in_x, in_z + 1))
			{
				canUp = false;
			}
			else canUp = true;
		}

		// Can dodge right
		if (tileMap.tiles[in_x + 1, in_z] != 2)
		{
			Debug.Log("No fire right!");

			// Check if a door is between fireman and safety. If its open, allow dodge
			if (doorManager.checkIfVDoor(in_x + 1, in_z))
			{
				if (doorManager.checkIfOpenVDoor(in_x + 1, in_z))
				{
					canRight = true;
				}
				else canRight = false;
			}
			// If an unbroken wall is between fireman and safety then they cannot dodge that way
			else if (wallManager.checkIfVWall(in_x + 1, in_z))
			{
				canRight = false;
			}
			else canRight = true;
		}

		// Can dodge down
		if (tileMap.tiles[in_x, in_z - 1] != 2)
		{
			Debug.Log("No fire down!");

			if (doorManager.checkIfHDoor(in_x, in_z))
			{
				if (doorManager.checkIfOpenHDoor(in_x, in_z))
				{
					canDown = true;
				}
				else canDown = false;
			}
			else if (wallManager.checkIfHWall(in_x, in_z))
			{
				canDown = false;
			}
			else canDown = true;
		}

		// Can dodge left
		if (tileMap.tiles[in_x - 1, in_z] != 2)
		{
			Debug.Log("No fire left!");

			if (doorManager.checkIfVDoor(in_x, in_z))
			{
				if (doorManager.checkIfOpenVDoor(in_x, in_z))
				{
					canLeft = true;
				}
				else canLeft = false;
			}
			else if (wallManager.checkIfVWall(in_x, in_z))
			{
				canLeft = false;
			}
			else canLeft = true;
		}

		// Print to console
		Debug.Log("canLeft: " + canLeft);
		Debug.Log("canDown: " + canDown);
		Debug.Log("canRight: " + canRight);
		Debug.Log("canUp: " + canUp);

		// Return true iff at least one direction is available
		return (canUp || canRight || canDown || canLeft);
	}

	// Used for the player to decide whether they want to dodge or not
	public void confirmDodgeYes()
	{
		wantDodge = true;
		confirmDodgeDown = true;
	}
	public void confirmDodgeNo()
	{
		wantDodge = false;
		confirmDodgeDown = true;
	}

	// Called from below in case Fireman chooses not to dodge or cannot
	public void knockDown(int x_elem, int z_elem)
	{
		// If firefighter is on the tile knock them out & send them to lower ambulance unit
		if (tileMap.selectedUnit.carryingVictim || tileMap.selectedUnit.ledPOI != null)
		{
			pOIManager.kill(x_elem, z_elem);
		}

        // Northern parking spot
		if (z_elem <= 3)
		{
			tileMap.selectedUnit.s.transform.position = new Vector3(0 * 6, 0.2f, 3 * 6);
			//tileMap.selectedUnit.s.transform.position = new Vector3(54, 0.2f, 18);
			UpdateLocation(0 * 6, 3 * 6, fireman.name);
			fireman.currentX = 0;
			fireman.currentZ = 18;
		}
		else // Southern/second parking spot
		{
			tileMap.selectedUnit.s.transform.position = new Vector3(54, 0.2f, 24);
		}
	}

	// Victims and POIs in spaces with Fire markers are 'Lost' (killed/destroyed)
	public IEnumerator knockDown()
	{
		for (int x_elem = 0; x_elem < mapSizeX; x_elem++)
		{
			for (int z_elem = 0; z_elem < mapSizeZ; z_elem++)
			{
				if (tileMap.tiles[x_elem, z_elem] == 2 &&
					tileMap.selectedUnit.currentX == (x_elem * 6) && tileMap.selectedUnit.currentZ == (z_elem * 6))
				{
					Debug.Log("Reached knockdown - savedAP = " + Math.Min(fireman.FreeAP, 4));

					// Check if the player is able to dodge:
					if (fireman.role == Role.Veteran && canDodge(x_elem, z_elem) && Math.Min(fireman.FreeAP, 4) >= 1)
					{
						// Allow the active player to choose/begin trying to dodge etc.
						isDodging = true;


						// Check if player wants to dodge
						Debug.Log("    VET (1) Please decide if you'd like to dodge or not! " + Time.time);
						yield return new WaitUntil(() => confirmDodgeDown == true);

						// Player has chosen to dodge
						if(wantDodge)
						{
							Debug.Log("    VET (2) Please press a dodge button: " + Time.time);
							yield return new WaitUntil(() => buttonDown() == true);
							fireman.setAP(fireman.FreeAP - 1);      // Spending the 1AP
							Debug.Log("    VET (3) Finished!" + Time.time);

							// Need to drop Victim or Hazmat. NB Fireman can dodge if leading treated victim
							if (fireman.carryingVictim == true)
							{
								operationManager.dropeV();
							}
							if (fireman.carriedHazmat != null)
							{
								operationManager.dropHazmat();
							}

							// Player has chosen to move to the left:
							if (leftDodgeDown)
							{
								Debug.Log("Moving left");
								tileMap.selectedUnit.s.transform.position = new Vector3(fireman.currentX - 6, 0.2f, fireman.currentZ);
								UpdateLocation(fireman.currentX - 6, fireman.currentZ, fireman.name);
								fireman.currentX = fireman.currentX - 6;
							}
							else if (downDodgeDown)
							{
								Debug.Log("Moving down");
								tileMap.selectedUnit.s.transform.position = new Vector3(fireman.currentX, 0.2f, fireman.currentZ - 6);
								UpdateLocation(fireman.currentX, fireman.currentZ - 6, fireman.name);
								fireman.currentZ = fireman.currentZ - 6;
							}
							else if (rightDodgeDown)
							{
								Debug.Log("Moving right");
								tileMap.selectedUnit.s.transform.position = new Vector3(fireman.currentX + 6, 0.2f, fireman.currentZ);
								UpdateLocation(fireman.currentX + 6, fireman.currentZ, fireman.name);
								fireman.currentX = fireman.currentX + 6;

							}
							else if (upDodgeDown)
							{
								Debug.Log("Moving up");
								tileMap.selectedUnit.s.transform.position = new Vector3(fireman.currentX, 0.2f, fireman.currentZ + 6);
								UpdateLocation(fireman.currentX, fireman.currentZ + 6, fireman.name);
								fireman.currentZ = fireman.currentZ + 6;
							}
						}
						else
						{
							Debug.Log("    VET (1.5) You have decided to not dodge. " + Time.time);
							knockDown(x_elem, z_elem);
						}
					}
					// Player is unable to dodge or has chosen not to dodge:
					else
					{
						knockDown(x_elem, z_elem);
					}
				}
			}
		}

		Debug.Log("Resetting dodge");
		resetDodge();
		yield return 0;
	}

	// Function to end/pass the turn - launches a coroutine to check for knockdowns
	public void EndTurn()
    {
		Debug.Log("Ending Turn");

		// BEGIN OF WIP

		// advanceFire, n.b parameters only matter for testing
		fireManager.advanceFire(1, 3, true);
		StartCoroutine(knockDown());
		Debug.Log("Finished advFire, redistributing AP");

		// END OF WIP

		operationManager.commandMoves = 1;
        operationManager.controlled = null;
        operationManager.inCommand = false;

        pOIManager.replenishPOI();
        operationManager.DestroyAll();


		checkTurn();
        //do stuff here...

        //if (isMyTurn)
        //{
		changeTurn();
        //}
        //else
        //{
        //    Debug.Log("This not your turn! Don't click end turn!");
        //}
    }

    public void checkTurn()
    {
        Debug.Log("checking turn");
        Dictionary<String, String> checkingTurn = new Dictionary<string, string>();
        checkingTurn["room"] = StaticInfo.roomNumber;
        checkingTurn["name"] = StaticInfo.name;

        socket.Emit("checkingTurn", new JSONObject(checkingTurn));
        //System.Threading.Thread.Sleep(2000);
    }

    public void changeTurn()
    {
        Debug.Log("changing turn");
        Dictionary<String, String> changingTurn = new Dictionary<string, string>();
        changingTurn["room"] = StaticInfo.roomNumber;
        changingTurn["name"] = StaticInfo.name;

        socket.Emit("changingTurn", new JSONObject(changingTurn));
    }

    public void SendChat()
    {
        Debug.Log(chat.text);
        Dictionary<String, String> sendChat = new Dictionary<string, string>();
        sendChat["name"] = StaticInfo.name;
        sendChat["chat"] = chat.text;

        var chatString = StaticInfo.name + " : " + chat.text;
        if(chatLog.Count>20){
            Destroy(chatLog[0].textObject.gameObject);
            chatLog.Remove(chatLog[0]);
        }

        Notification notification=new Notification();
        notification.msg=chatString;
        GameObject newText=Instantiate(notificationText,notificationPanel.transform);
        notification.textObject=newText.GetComponent<Text>();
        notification.textObject.text=notification.msg;

        chatLog.Add(notification);
        // chatLog.Add(chatString);

        socket.Emit("sendChat", new JSONObject(sendChat));
    }

    public void sendNotification(string msg){
        Dictionary<string, string> message = new Dictionary<string, string>();
        message["name"]=StaticInfo.name;
        message["text"]=msg;
        socket.Emit("sendNotification",new JSONObject(message));
    }

    void sendNotification_SUCCESS(SocketIOEvent obj){
        var name = obj.data.ToDictionary()["name"];
        var text = obj.data.ToDictionary()["text"];

        var chatString = name + " " + text;
        if(chatLog.Count>20){
            Destroy(chatLog[0].textObject.gameObject);
            chatLog.Remove(chatLog[0]);
        }

        Notification notification=new Notification();
        notification.msg=chatString;
        GameObject newText=Instantiate(notificationText,notificationPanel.transform);
        notification.textObject=newText.GetComponent<Text>();
        notification.textObject.text=notification.msg;

        chatLog.Add(notification);
        Debug.Log(chatString);
    }

    public void UpdateTreatV(int x,int z)
    {
        Debug.Log("update after treat victim");
        Dictionary<String, string> treat = new Dictionary<string, string>();
        treat["x"] = x.ToString();
        treat["z"] = z.ToString();

        socket.Emit("TreatV", new JSONObject(treat));
    }

    public void UpdateTreatV_Success(SocketIOEvent obj)
    {
        int x = Convert.ToInt32(obj.data.ToDictionary()["x"]);
        int z = Convert.ToInt32(obj.data.ToDictionary()["z"]);

        pOIManager.treat(x, z);
    }

    public void UpdatePOILocation(int origx,int origz, int newx, int newz)
    {
        Debug.Log("update poi location");
        Dictionary<String, string> location = new Dictionary<string, string>();
        location["origx"] = origx.ToString();
        location["origz"] = origz.ToString();
        location["newx"] = newx.ToString();
        location["newz"] = newz.ToString();

        socket.Emit("UpdatePOILocation", new JSONObject(location));
    }

    public void UpdatePOILocation_Success(SocketIOEvent obj)
    {
        int origx = Convert.ToInt32(obj.data.ToDictionary()["origx"]);
        int origz = Convert.ToInt32(obj.data.ToDictionary()["origz"]);
        int newx = Convert.ToInt32(obj.data.ToDictionary()["newx"]);
        int newz = Convert.ToInt32(obj.data.ToDictionary()["newz"]);

        pOIManager.movePOI(origx, origz, newx, newz);
    }

    public void UpdateTreatedLocation(int origx, int origz, int newx, int newz)
    {
        Debug.Log("update treated location");
        Dictionary<String, string> location = new Dictionary<string, string>();
        location["origx"] = origx.ToString();
        location["origz"] = origz.ToString();
        location["newx"] = newx.ToString();
        location["newz"] = newz.ToString();

        socket.Emit("UpdateTreatedLocation", new JSONObject(location));
    }

    public void UpdateTreatedLocation_Success(SocketIOEvent obj)
    {
        int origx = Convert.ToInt32(obj.data.ToDictionary()["origx"]);
        int origz = Convert.ToInt32(obj.data.ToDictionary()["origz"]);
        int newx = Convert.ToInt32(obj.data.ToDictionary()["newx"]);
        int newz = Convert.ToInt32(obj.data.ToDictionary()["newz"]);

        pOIManager.moveTreated(origx, origz, newx, newz);
    }

    public void RemoveHazmat(int x,int z)
    {
        Debug.Log("RemovingHazmat");
        Dictionary<String, string> hazmat = new Dictionary<string, string>();
        hazmat["x"] = x.ToString();
        hazmat["z"] = z.ToString();

        socket.Emit("RemoveH", new JSONObject(hazmat));
    }

    public void RemoveHazmat_Success(SocketIOEvent obj)
    {
        int x = Convert.ToInt32(obj.data.ToDictionary()["x"]);
        int z = Convert.ToInt32(obj.data.ToDictionary()["z"]);

        hazmatManager.removeHazmat(x, z);
    }

    public void UpdateHazmatLocation(int origx, int origz, int newx, int newz)
    {
        Debug.Log("update hazmat location");
        Dictionary<String, string> location = new Dictionary<string, string>();
        location["origx"] = origx.ToString();
        location["origz"] = origz.ToString();
        location["newx"] = newx.ToString();
        location["newz"] = newz.ToString();

        socket.Emit("UpdateTreatedLocation", new JSONObject(location));
    }

    public void UpdateHazmatLocation_Success(SocketIOEvent obj)
    {
        int origx = Convert.ToInt32(obj.data.ToDictionary()["origx"]);
        int origz = Convert.ToInt32(obj.data.ToDictionary()["origz"]);
        int newx = Convert.ToInt32(obj.data.ToDictionary()["newx"]);
        int newz = Convert.ToInt32(obj.data.ToDictionary()["newz"]);

        hazmatManager.moveHazmat(origx, origz, newx, newz);
    }

    public void startDrive(int type)
    {
        Dictionary<string, string> update = new Dictionary<string, string>();
        update["room"] = StaticInfo.roomNumber;
        update["name"] = StaticInfo.name;
        update["Location"] = StaticInfo.Location[0] + "," + StaticInfo.Location[1];
        update["driving"] = type.ToString();

        socket.Emit("StartDrive", new JSONObject(update));
    }

    public void startCarryV(int x, int z)
    {
        Dictionary<string, string> update = new Dictionary<string, string>();
        update["room"] = StaticInfo.roomNumber;
        update["name"] = StaticInfo.name;
        update["Location"] = StaticInfo.Location[0] + "," + StaticInfo.Location[1];
        update["carryV"] = true.ToString();
        update["x"]=x.ToString();
        update["z"]=z.ToString();

        socket.Emit("StartCarryV", new JSONObject(update));
    }

    public void startLeadV(int x, int z){
        Dictionary<string, string> update = new Dictionary<string, string>();
        update["room"] = StaticInfo.roomNumber;
        update["name"] = StaticInfo.name;
        update["Location"] = StaticInfo.Location[0] + "," + StaticInfo.Location[1];
        update["carryV"] = true.ToString();
        update["x"]=x.ToString();
        update["z"]=z.ToString();

        socket.Emit("StartLeadV", new JSONObject(update));
    }

    public void StartCarryV_Success(SocketIOEvent obj){
        room = obj.data["Games"][StaticInfo.roomNumber];
        participants = room["participants"];
        level = room["level"].ToString();
        numberOfPlayer = room["numberOfPlayer"].ToString();

        List<string> p = participants.keys;
        foreach (var v in p)
        {
            var o = participants[v];
            players[v] = o;
            // Debug.Log(v);
            // Debug.Log(players[v]);
        }

        int x=Convert.ToInt32(obj.data.ToDictionary()["x"]);
        int z=Convert.ToInt32(obj.data.ToDictionary()["z"]);
        pOIManager.carryPOI(x,z);

    }

    public void StartLeadV_Success(SocketIOEvent obj){
        room = obj.data["Games"][StaticInfo.roomNumber];
        participants = room["participants"];
        level = room["level"].ToString();
        numberOfPlayer = room["numberOfPlayer"].ToString();

        List<string> p = participants.keys;
        foreach (var v in p)
        {
            var o = participants[v];
            players[v] = o;
            // Debug.Log(v);
            // Debug.Log(players[v]);
        }

        int x=Convert.ToInt32(obj.data.ToDictionary()["x"]);
        int z=Convert.ToInt32(obj.data.ToDictionary()["z"]);
        pOIManager.leadPOI(x,z);

    }

    public void startCarryHazmat(int x, int z){
        Dictionary<string, string> update = new Dictionary<string, string>();
        update["room"] = StaticInfo.roomNumber;
        update["name"] = StaticInfo.name;
        update["Location"] = StaticInfo.Location[0] + "," + StaticInfo.Location[1];
        update["carryV"] = true.ToString();
        update["x"]=x.ToString();
        update["z"]=z.ToString();

        socket.Emit("StartCarryHazmat", new JSONObject(update));
    }

    public void StartCarryHazmat_Success(SocketIOEvent obj){
        room = obj.data["Games"][StaticInfo.roomNumber];
        participants = room["participants"];
        level = room["level"].ToString();
        numberOfPlayer = room["numberOfPlayer"].ToString();

        List<string> p = participants.keys;
        foreach (var v in p)
        {
            var o = participants[v];
            players[v] = o;
            // Debug.Log(v);
            // Debug.Log(players[v]);
        }

        int x=Convert.ToInt32(obj.data.ToDictionary()["x"]);
        int z=Convert.ToInt32(obj.data.ToDictionary()["z"]);
        hazmatManager.carryHazmat(x,z);

    }

    public void StopCarry(int x, int z){
        Dictionary<string,string> carry=new Dictionary<string, string>();
        carry["room"]=StaticInfo.roomNumber;
        carry["name"]=StaticInfo.name;
        carry["x"]=x.ToString();
        carry["z"]=z.ToString();
        socket.Emit("StopCarry",new JSONObject(carry));
    }

    public void StopCarry_Success(SocketIOEvent obj){
        int x=Convert.ToInt32(obj.data.ToDictionary()["x"]);
        int z=Convert.ToInt32(obj.data.ToDictionary()["z"]);

        room = obj.data["Games"][StaticInfo.roomNumber];
        participants = room["participants"];
        level = room["level"].ToString();
        numberOfPlayer = room["numberOfPlayer"].ToString();

        List<string> p = participants.keys;
        foreach (var v in p)
        {
            var o = participants[v];
            players[v] = o;
            // Debug.Log(v);
            // Debug.Log(players[v]);
        }
        pOIManager.dropPOI(x,z);
    }

    public void StopLead(int x, int z){
        Dictionary<string,string> carry=new Dictionary<string, string>();
        carry["room"]=StaticInfo.roomNumber;
        carry["name"]=StaticInfo.name;
        carry["x"]=x.ToString();
        carry["z"]=z.ToString();
        socket.Emit("StopLead",new JSONObject(carry));
    }

    public void StopLead_Success(SocketIOEvent obj){
        int x=Convert.ToInt32(obj.data.ToDictionary()["x"]);
        int z=Convert.ToInt32(obj.data.ToDictionary()["z"]);

        room = obj.data["Games"][StaticInfo.roomNumber];
        participants = room["participants"];
        level = room["level"].ToString();
        numberOfPlayer = room["numberOfPlayer"].ToString();

        List<string> p = participants.keys;
        foreach (var v in p)
        {
            var o = participants[v];
            players[v] = o;
            // Debug.Log(v);
            // Debug.Log(players[v]);
        }
        pOIManager.dropPOI(x,z);
    }
    

    public void AddPOI(int x, int z,int type)
    {
        Dictionary<string, string> poi = new Dictionary<string, string>();
        poi["x"] = x.ToString();
        poi["z"] = z.ToString();
        poi["type"] = type.ToString();

        socket.Emit("AddPOI", new JSONObject(poi));
    }

    public void AddPOI_Success(SocketIOEvent obj)
    {
        int x = Convert.ToInt32(obj.data.ToDictionary()["x"].ToString());
        int z = Convert.ToInt32(obj.data.ToDictionary()["z"].ToString());
        int type = Convert.ToInt32(obj.data.ToDictionary()["type"].ToString());

        pOIManager.addPOI(x, z, type);
    }

    public void AddHazmat(int x, int z,int type)
    {
        Dictionary<string, string> poi = new Dictionary<string, string>();
        poi["x"] = x.ToString();
        poi["z"] = z.ToString();
        poi["type"] = type.ToString();

        socket.Emit("AddHazmat", new JSONObject(poi));
    }

    public void AddHazmat_Success(SocketIOEvent obj)
    {
        int x = Convert.ToInt32(obj.data.ToDictionary()["x"].ToString());
        int z = Convert.ToInt32(obj.data.ToDictionary()["z"].ToString());
        int type = Convert.ToInt32(obj.data.ToDictionary()["type"].ToString());

        hazmatManager.addHazmat(x, z, type);
    }

    public void stopDrive(string name)
    {
        Dictionary<string, string> stop = new Dictionary<string, string>();
        stop["name"] = name;
        stop["room"] = StaticInfo.roomNumber;

        socket.Emit("StopDrive", new JSONObject(stop));
    }

    public void stopDrive_Success(SocketIOEvent obj)
    {
        room = obj.data["Games"][StaticInfo.roomNumber];
        participants = room["participants"];
        level = room["level"].ToString();
        numberOfPlayer = room["numberOfPlayer"].ToString();

        List<string> p = participants.keys;
        foreach (var v in p)
        {
            var o = participants[v];
            players[v] = o;
            // Debug.Log(v);
            // Debug.Log(players[v]);
        }
    }

    public void StopRide_Success(SocketIOEvent obj){
        room = obj.data["Games"][StaticInfo.roomNumber];
        participants = room["participants"];
        level = room["level"].ToString();
        numberOfPlayer = room["numberOfPlayer"].ToString();

        List<string> p = participants.keys;
        foreach (var v in p)
        {
            var o = participants[v];
            players[v] = o;
            // Debug.Log(v);
            // Debug.Log(players[v]);
        }

        List<string> names=parseJsonArray(obj.data["ToStop"]);
        foreach(string n in names){
            if(n.Equals(StaticInfo.name)){
                this.fireman.riding=false;
            }
        }
    }

    public void stopRide(string name)
    {
        Dictionary<string, string> stop = new Dictionary<string, string>();
        stop["name"] = name;
        stop["room"] = StaticInfo.roomNumber;

        socket.Emit("StopRide", new JSONObject(stop));
    }

    public List<string> parseJsonArray(JSONObject obj)
    {
        string result = obj.ToString().TrimStart('[');
        result = result.TrimEnd(']');
        if(result.Equals("")){
            return new List<string>();
        }
        string[] words = result.Split(',');
        List<string> final = new List<string>();
        foreach(string w in words)
        {
            string a = w.TrimStart('\"');
            a = a.TrimEnd('\"');
            final.Add(a);
        }
        return final;
    }

    public void changeRole()
    {
        if (fireman.FreeAP < 2)
        {
            return;
        }
        selectRolePanel.SetActive(true);
        possibleRoles.ClearOptions();

        List<string> roles = new List<string>();

        List<string> allRoles = parseJsonArray(room["selectedRoles"]);

        foreach(string s in allRoles)
        {
            Debug.Log(s);
        }
        if (!allRoles.Contains("3"))
        {
            roles.Add(roleToString(Role.CAFS));
        }
        if (!allRoles.Contains("7"))
        {
            roles.Add(roleToString(Role.Driver));
        }
        if (!allRoles.Contains("1"))
        {
            roles.Add(roleToString(Role.Captain));
        }
        if (!allRoles.Contains("5"))
        {
            roles.Add(roleToString(Role.Generalist));
        }
        if (!allRoles.Contains("4"))
        {
            roles.Add(roleToString(Role.HazmatTech));
        }
        if (!allRoles.Contains("0"))
        {
            roles.Add(roleToString(Role.Paramedic));
        }
        if (!allRoles.Contains("8"))
        {
            roles.Add(roleToString(Role.Dog));
        }
        if (!allRoles.Contains("6"))
        {
            roles.Add(roleToString(Role.RescueSpec));
        }
        if (!allRoles.Contains("9"))
        {
            roles.Add(roleToString(Role.Veteran));
        }
        if (!allRoles.Contains("2"))
        {
            roles.Add(roleToString(Role.ImagingTech));
        }

        possibleRoles.AddOptions(roles);
    }

    public void selectRole()
    {
        for(int i = 0; i < 10; i++)
        {
            if (roleToString((Role)i).Equals(selectedRole.text))
            {
                Role oldRole = StaticInfo.role;
                fireman.setRole((Role)i);
                StaticInfo.role=(Role)i;
                fireman.setAP(fireman.FreeAP - 2);
                displayRole();
                Dictionary<string, string> change = new Dictionary<string, string>();
                change["room"] = StaticInfo.roomNumber;
                change["name"] = StaticInfo.name;
                change["role"] = ((int)StaticInfo.role).ToString();
                change["oldRole"] = ((int)oldRole).ToString();
                socket.Emit("changeRole", new JSONObject(change));
                break;
            }
        }
        selectRolePanel.SetActive(false);
    }

    public void changeRole_Success(SocketIOEvent obj){
        room = obj.data["Games"][StaticInfo.roomNumber];
        participants = room["participants"];
        level = room["level"].ToString();
        numberOfPlayer = room["numberOfPlayer"].ToString();

        List<string> p = participants.keys;
        foreach (var v in p)
        {
            var o = participants[v];
            players[v] = o;
            // Debug.Log(v);
            // Debug.Log(players[v]);
        }

        if(!level.Equals("Family")){
            displayRole();
        }
    }

    public void initializePOI(){
        socket.Emit("InitializePOI");
    }

    public void initializePOI_Success(SocketIOEvent obj){
        pOIManager.refreshPOI();
    }

    public void initializeHazmat(){
        socket.Emit("InitializeHazmat");
    }

    public void initializeHazmat_Success(SocketIOEvent obj){
        hazmatManager.refreshHazmat();
    }

    public void rescueCarried(int x, int z){
        Dictionary<string,string> rescue=new Dictionary<string, string>();
        rescue["x"]=x.ToString();
        rescue["z"]=z.ToString();
        socket.Emit("RescueCarried",new JSONObject(rescue));
    }

    public void rescueTreated(int x, int z){
        Dictionary<string,string> rescue=new Dictionary<string, string>();
        rescue["x"]=x.ToString();
        rescue["z"]=z.ToString();
        socket.Emit("RescueTreated",new JSONObject(rescue));
    }

    public void rescueCarried_Success(SocketIOEvent obj){
        int x=Convert.ToInt32(obj.data.ToDictionary()["x"]);
        int z=Convert.ToInt32(obj.data.ToDictionary()["z"]);

        pOIManager.rescueCarried(x,z);
    }

    public void rescueTreated_Success(SocketIOEvent obj){
        int x=Convert.ToInt32(obj.data.ToDictionary()["x"]);
        int z=Convert.ToInt32(obj.data.ToDictionary()["z"]);

        pOIManager.rescueTreated(x,z);
    }

    //check for victory and defeat
    public void victory_Success(SocketIOEvent obj)
    {
        Debug.Log("Update victory");
    }

    public void defeat_Success(SocketIOEvent obj)
    {
        Debug.Log("Update defeat");
    }

    public void victory()
    {
        Debug.Log("You win!");
        socket.Emit("victory");
        SceneManager.LoadScene("Win");
    }

    public void defeat()
    {
        Debug.Log("Game Over!");
        socket.Emit("defeat");
        SceneManager.LoadScene("gameOver");
    }

}

public class Notification{
    public string msg;
    public Text textObject;
}

