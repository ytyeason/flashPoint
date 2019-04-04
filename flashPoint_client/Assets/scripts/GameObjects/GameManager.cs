using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
using System.Linq;
using UnityEngine.UI;
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

    // --op
    public List<Button> prefabs = new List<Button>();
    public GameObject opPanel;

    public List<GameObject> options = new List<GameObject>();

    // change role
    public Dropdown possibleRoles;
    public GameObject selectRolePanel;
    public Text selectedRole;
    public GameObject changeRoleButton;


    private JSONObject room;
    private JSONObject participants;
    private String level;
    private String numberOfPlayer;
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
        socket.On("UpdateTreatedLocation_Success", UpdateTreatedLocation_Success);
        socket.On("RemoveHazmat_Success", RemoveHazmat_Success);
        socket.On("UpdateHazmatLocation_Success", UpdateHazmatLocation_Success);
        socket.On("AddPOI_Success", AddPOI_Success);

        if (game_info != null)
        {
            room = game_info[StaticInfo.roomNumber];
            participants = room["participants"];
            level = room["level"].ToString();
            numberOfPlayer = room["numberOfPlayer"].ToString();

            List<string> p = participants.keys;
            foreach (var v in p)
            {
                //Debug.Log(participants[v]);
                var o = participants[v];
                players[v] = o;
                //Debug.Log(players[v]);
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


        //displayAP(Convert.ToInt32(players[StaticInfo.name]["AP"].ToString()),fireman.remainingSpecAp);
        displayAP();
     //   vehicleManager.StartvehicleManager();

        tileMap.GenerateFiremanVisual(players);
        registerNewFireman(fireman);
        checkTurn();	//initialize isMyTurn variable at start
        if (!level.Equals("Family"))
        {
            displayRole();
        }
        else
        {
            changeRoleButton.SetActive(false);
        }

        selectRolePanel.SetActive(false);


    }

    public void displayAP(){
        Debug.Log(fireman);
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
        Debug.Log("displaying role");
        roles.text = StaticInfo.name+": "+roleToString(StaticInfo.role);
        if (players != null)
        {
            foreach (string name in players.Keys)
            {
                if(!name.Equals(StaticInfo.name)) roles.text += "\n" + name + ": " + roleToString((Role)Int32.Parse(players[name]["Role"].ToString()));
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
		//wallManager.BreakWall(x, z, type, horizontal, false);
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
		// tileMap.buildNewTile(x, z,type);
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
        if (!level.Equals("Family")) displayRole();
    }

//for vehicles
    public void UpdateAmbulanceLocation(int newx,int newz)
    {
        Dictionary<String, string> location = new Dictionary<string, string>();
        location["newx"] = newx.ToString();
        location["newz"] = newz.ToString();

        socket.Emit("UpdateAmbulanceLocation", new JSONObject(location));
        Debug.Log("update ambulance location");

    }


    public void UpdateAmbulanceLocation_Success(SocketIOEvent obj)
    {
        Debug.Log("hello,updateambulance");
        int newx = Convert.ToInt32(obj.data.ToDictionary()["newx"]);
        int newz = Convert.ToInt32(obj.data.ToDictionary()["newz"]);
        tileMap.ambulance.moveNextStation(newx, newz);
    }

    public void UpdateEngineLocation(int newx,int newz)
    {
        Dictionary<String, string> location = new Dictionary<string, string>();
        location["newx"] = newx.ToString();
        location["newz"] = newz.ToString();

        socket.Emit("UpdateEngineLocation", new JSONObject(location));
        Debug.Log("update eng location");

    }
    
    public void UpdateEngineLocation_Success(SocketIOEvent obj)
    {
        int newx = Convert.ToInt32(obj.data.ToDictionary()["newx"]);
        int newz = Convert.ToInt32(obj.data.ToDictionary()["newz"]);
        tileMap.engine.moveNextStation(newx, newz);
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
        Ambulance amb = new Ambulance(ambulance, 54, 27, this);

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


    public void EndTurn()
    {
        Debug.Log("Ending Turn");

		// advanceFire, n.b parameters only matter for testing
		fireManager.advanceFire(1, 4, false);
		Debug.Log("Finished advFire, redistributing AP");

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

    public void startCarryV()
    {
        Dictionary<string, string> update = new Dictionary<string, string>();
        update["room"] = StaticInfo.roomNumber;
        update["name"] = StaticInfo.name;
        update["Location"] = StaticInfo.Location[0] + "," + StaticInfo.Location[1];
        update["carryV"] = true.ToString();

        socket.Emit("StartCarryV", new JSONObject(update));
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

    public void stopDrive(string name)
    {
        Dictionary<string, string> stop = new Dictionary<string, string>();
        stop["name"] = name;
        stop["room"] = StaticInfo.roomNumber;

        socket.Emit("StopDrive", new JSONObject(stop));
    }

    public void stopDrive_Success(SocketIOEvent obj)
    {

    }

    public List<string> parseJsonArray(JSONObject obj)
    {
        string result = obj.ToString().TrimStart('[');
        result = result.TrimEnd(']');
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

}

public class Notification{
    public string msg;
    public Text textObject;
}