using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
//using Newtonsoft.Json;
//using System.Web.Script.Serialization;


[Serializable]
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
    public int toX;
    public int toZ;
    public int fromX;
    public int fromZ;

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
    int[,] playerLocations; // Holds all player's locations
    int numPlayers;

    // Dodging GUI items:
    public GameObject backdropL;    // For the dodge GameObjects
    public GameObject backdropS;    // For dodge confirmations
    public GameObject leftDodgeButton;
    public GameObject upDodgeButton;
    public GameObject rightDodgeButton;
    public GameObject downDodgeButton;
    public GameObject confirmDodge;
    public GameObject declineDodge;
    public ToggleActiveDodge toggleActiveDodge;
    String[] names;
    String[] rolesArr;

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

    public bool isMyTurn = false;
    public bool isOwner = false;

    public List<Notification> chatLog = new List<Notification>();
    public GameObject notificationPanel, notificationText;

    public Text chat;

    public Text nameAP;

    public Text roles;

    public Text stats;

    public GameObject tooltipPanel;
    public Text tooltip;

    public GameObject startingPositionPanel;
    public GameObject startingAmbulancePositionPanel;
    public GameObject startingEnginePositionPanel;

    private bool endOfTurn=false;


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
        socket.On("StopCarryH_Success",StopCarryH_Success);
        socket.On("StopLead_Success",StopLead_Success);
        socket.On("changeRole_Success",changeRole_Success);
        socket.On("RescueCarried_Success",rescueCarried_Success);
        socket.On("RescueTreated_Success",rescueTreated_Success);
        socket.On("KillPOI_Success",killPOI_Success);
        socket.On("victory_Success",victory_Success);
        socket.On("defeat_Success",defeat_Success);
        socket.On("ResetConfirmed_Success", ResetConfirmed_Success);
        socket.On("SaveGame_Success", SaveGame_Success);
        socket.On("ConfirmPosition_Success",confirmPosition_Success);
        socket.On("JoinGame_Success",JoinGame_Success);
        socket.On("ExplodeHazmat_Success",explodeHazmat_Success);
        socket.On("checkOwner_Success", checkOwner_Success);

        //check owner
        checkOwner(StaticInfo.name);

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
                Debug.Log(participants[v]);
                var o = participants[v];
                players[v] = o;
                Debug.Log("debug location");
                Debug.Log(players[v]["Location"]);
            }
        }
        else
        {
            Debug.Log("game_info is null");
        }

        if (game_info != null)
        {
            if (!StaticInfo.LoadGame)//if creating a new game
            {
                fireman = initializeFireman();
                amB = initializeAmbulance(0);
                enG = initializeEngine(0);
                operationManager = new OperationManager(this);
                wallManager = new WallManager(wallTypes, this,0);
                doorManager = new DoorManager(doorTypes, this,0);
                //    vehicleManager = new VehicleManager(vehicleTypes,this);
                tileMap = new TileMap(tileTypes, this, fireman, enG, amB,0);
                fireManager = new FireManager(this, tileMap, mapSizeX, mapSizeZ);
                pOIManager = new POIManager(this,0);
                hazmatManager = new HazmatManager(this,0);
                // Next 3 are for dodging:
                vicinityManager = new VicinityManager(this, tileMap.tiles);
                toggleActiveDodge = new ToggleActiveDodge(this, backdropL, backdropS, leftDodgeButton, upDodgeButton, downDodgeButton, rightDodgeButton, confirmDodge, declineDodge);
                numPlayers = Convert.ToInt32(numberOfPlayer.Substring(1, 1));
                playerLocations = new int[numPlayers, 2];
                names = new String[numPlayers];
                rolesArr = new String[numPlayers];

                //displayAP(Convert.ToInt32(players[StaticInfo.name]["AP"].ToString()),fireman.remainingSpecAp);
                
                //   vehicleManager.StartvehicleManager();

                tileMap.GenerateFiremanVisual(players);
                registerNewFireman(fireman);
                checkTurn(); //initialize isMyTurn variable at start
                if (!level.Equals("\"Family\""))
                {
                    displayRole();
                    if(StaticInfo.StartingPosition||StaticInfo.StartingAmbulancePosition||StaticInfo.StartingEnginePosition){
                        changeRoleButton.SetActive(false);
                    }
                    
                    // if(!StaticInfo.StartingPosition&&isMyTurn){
                    //     changeRoleButton.SetActive(true);
                    // }
                    // if(!StaticInfo.StartingPosition&&!isMyTurn){
                    //     changeRoleButton.SetActive(false);
                    // }
                }
                else
                {
                    changeRoleButton.SetActive(false);
                    ambulance.SetActive(false);
                    engine.SetActive(false);
                }

                selectRolePanel.SetActive(false);
                if(StaticInfo.StartingPosition){
                    startingPositionPanel.SetActive(true);
                }else{
                    startingPositionPanel.SetActive(false);
                }

                if(StaticInfo.StartingAmbulancePosition){
                    startingAmbulancePositionPanel.SetActive(true);
                }else{
                    startingAmbulancePositionPanel.SetActive(false);
                }

                if(StaticInfo.StartingEnginePosition){
                    startingEnginePositionPanel.SetActive(true);
                }else{
                    startingEnginePositionPanel.SetActive(false);
                }
                
            }
            else//if we're loading a game
            {
                fireman = initializeFireman();
                amB = initializeAmbulance(1);
                enG = initializeEngine(1);
                operationManager = new OperationManager(this);
                wallManager = new WallManager(wallTypes, this,1);// set to 1, generate walls based on staticInfo hWallMemo and vWallMemo
                doorManager = new DoorManager(doorTypes, this,1);
                tileMap = new TileMap(tileTypes, this, fireman, enG, amB,1);
                fireManager = new FireManager(this, tileMap, mapSizeX, mapSizeZ);

                //poi -- not done
                //pOIManager = new POIManager(this,1);
                //pOIManager = StaticInfo.poiManager;

                // Next 3 are for dodging:
                vicinityManager = new VicinityManager(this, tileMap.tiles);
                toggleActiveDodge = new ToggleActiveDodge(this, backdropL, backdropS, leftDodgeButton, upDodgeButton, downDodgeButton, rightDodgeButton, confirmDodge, declineDodge);
                numPlayers = Convert.ToInt32(numberOfPlayer.Substring(1, 1));
                playerLocations = new int[numPlayers, 2];
                names = new String[numPlayers];
                rolesArr = new String[numPlayers];

                //poi -- not done
                pOIManager = new POIManager(this,1);
                //hazmat -- not done
                hazmatManager = new HazmatManager(this,1);

                
                tileMap.GenerateFiremanVisual(players);
                registerNewFireman(fireman);
                checkTurn(); //initialize isMyTurn variable at start
                if (!level.Equals("\"Family\""))
                {
                    displayRole();
                    if(StaticInfo.StartingPosition||StaticInfo.StartingAmbulancePosition||StaticInfo.StartingEnginePosition){
                        changeRoleButton.SetActive(false);
                    }
                    
                    // if(!StaticInfo.StartingPosition&&isMyTurn){
                    //     changeRoleButton.SetActive(true);
                    // }
                    // if(!StaticInfo.StartingPosition&&!isMyTurn){
                    //     changeRoleButton.SetActive(false);
                    // }
                }
                else
                {
                    changeRoleButton.SetActive(false);
                    ambulance.SetActive(false);
                    engine.SetActive(false);
                }

                selectRolePanel.SetActive(false);
                if(StaticInfo.StartingPosition){
                    startingPositionPanel.SetActive(true);
                }else{
                    startingPositionPanel.SetActive(false);
                }

                if(StaticInfo.StartingAmbulancePosition){
                    startingAmbulancePositionPanel.SetActive(true);
                }else{
                    startingAmbulancePositionPanel.SetActive(false);
                }

                if(StaticInfo.StartingEnginePosition){
                    startingEnginePositionPanel.SetActive(true);
                }else{
                    startingEnginePositionPanel.SetActive(false);
                }

                fireman.FreeAP = StaticInfo.freeAP;
                fireman.remainingSpecAp = StaticInfo.remainingSpecAp;
                wallManager.damagedWalls = StaticInfo.damagedWall;
                pOIManager.rescued = StaticInfo.rescued;
                pOIManager.killed = StaticInfo.killed;
                hazmatManager.removedHazmat = StaticInfo.removedHazmat;
                
                fireman.riding = StaticInfo.riding;
                fireman.driving = StaticInfo.driving;
                fireman.carryingHazmat = StaticInfo.carryingHazmat;
                fireman.carryingVictim = StaticInfo.carryingVictim;
                fireman.leadingVictim = StaticInfo.leadingVictim;

            }

        }

        // displayAP();
        displayStats();
        // if(!StaticInfo.StartingPosition&&isMyTurn&&!endOfTurn){
        //     changeRoleButton.SetActive(true);
        // }
        if(!StaticInfo.StartingPosition&&!isMyTurn){
            changeRoleButton.SetActive(false);
        }

    }


    public void displayAP(){
        Debug.Log("im" + StaticInfo.name + "in displayAp in gm, my free AP is:"+ fireman.FreeAP);
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

    public void displayStats()
    {
        Debug.Log("DamagedWall:" + this.damaged_wall_num);
        stats.text = "Damaged Marker" + " : " + wallManager.damagedWalls;
        stats.text+="\nRescued Victims" + " : " + pOIManager.rescued;
        stats.text+= "\nKilled Victims" + " : " + pOIManager.killed;
        if (!StaticInfo.level.Equals("Family"))
        {
            stats.text+= "\nRemoved Hazmat" + " : " + hazmatManager.removedHazmat;
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
        roles.text += " " + "at: " + fireman.currentX/6 + "," + fireman.currentZ/6;
        if (players != null)
        {
            foreach (string name in players.Keys)
            {
                if(!name.Equals(StaticInfo.name))
                {
                  roles.text += "\n" + name + ": " + roleToString((Role)Int32.Parse(players[name].ToDictionary()["Role"]));
                  string location = players[name].ToDictionary()["Location"];
                    var cord = location.Split(',');
                    int cord_x = Convert.ToInt32(cord[0])/6;
                    int cord_z = Convert.ToInt32(cord[1])/6;
                  roles.text += " " + "at: " + cord_x.ToString() + "," + cord_z.ToString();
                }
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


    void SaveGame_Success(SocketIOEvent obj)
    {
        Debug.Log("SaveGame_Success");
        /*
        Debug.Log(obj.data);
        Debug.Log(obj.data[0]); // [{"1,2":0},{"2,2":0}]
        Debug.Log(obj.data[1]);
        Debug.Log(obj.data[0][0]);
        Debug.Log(obj.data[0][1]);
        Debug.Log(obj.data[0].Count);
        Debug.Log(obj.data[0][0].ToDictionary().Keys);
        Debug.Log(obj.data[0][1].ToDictionary().Keys);
        Debug.Log(obj.data[0][2].ToDictionary().Keys);
        foreach (KeyValuePair<string, string> entry in obj.data[0][2].ToDictionary())
        {
            Debug.Log(entry.Key);
            Debug.Log(entry.Value);
        }
        */
        /*
        Dictionary<string,string> s = obj.data.ToDictionary();
        Debug.Log(JsonUtility.FromJson<GameManager>(s["doorManager"]));
        */
        /*
        GameData g = new GameData();
        g.gm = JsonUtility.FromJson<GameManager>(s);
        Debug.Log(g + "------------");
        */
        
        Debug.Log(obj.data.ToString());
        Debug.Log(JsonUtility.FromJson<POIManager>(obj.data.ToString()));
        POIManager p = JsonUtility.FromJson<POIManager>(obj.data.ToString());
        StaticInfo.poiManager = p;
        Debug.Log(p.posY);
        Debug.Log(p.placedPOI);

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
        int x = Convert.ToInt32(obj.data.ToDictionary()["x"]);
        int z = Convert.ToInt32(obj.data.ToDictionary()["z"]);
        int type = Convert.ToInt32(obj.data.ToDictionary()["type"]);
        int horizontal = Convert.ToInt32(obj.data.ToDictionary()["horizontal"]);
        string fromExplosion=obj.data.ToDictionary()["fromExplosion"];
        bool from=true;
        Debug.Log("fromExplosion: "+fromExplosion);
        if(fromExplosion.Equals("True")){
            from=true;
        }else{
            from=false;
        }

        //Debug.Log(x);
        //Debug.Log(z);
        //Debug.Log(type);
        Debug.Log(obj.data);
        Debug.Log(obj.data.ToDictionary()["x"]);
        Debug.Log(obj.data.ToDictionary()["z"]);
        Debug.Log(obj.data.ToDictionary()["type"]);
        Debug.Log(obj.data.ToDictionary()["horizontal"]);

        // Bottom is temporarily commented out:
        wallManager.BreakWall(x, z, type, horizontal, true);
    }

    void DoorUpdate_Success(SocketIOEvent obj)
    {
        Debug.Log("door update successful");
        int x = Convert.ToInt32(obj.data.ToDictionary()["x"]);
        int z = Convert.ToInt32(obj.data.ToDictionary()["z"]);
        int type = Convert.ToInt32(obj.data.ToDictionary()["type"]);
        int toType = Convert.ToInt32(obj.data.ToDictionary()["toType"]);
        string fromExplosion=obj.data.ToDictionary()["fromExplosion"];
        bool det=true;
        Debug.Log("fromExplosion:"+fromExplosion);
        if(fromExplosion.Equals("True")){
            det=true;
        }else{
            det=false;
        }

        //Debug.Log(x);
        //Debug.Log(z);
        //Debug.Log(type);
        Debug.Log(obj.data);
        Debug.Log(obj.data.ToDictionary()["x"]);
        Debug.Log(obj.data.ToDictionary()["z"]);
        Debug.Log(obj.data.ToDictionary()["type"]);
        Debug.Log(obj.data.ToDictionary()["toType"]);

        doorManager.ChangeDoor(x, z, toType, type,true);
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
        displayRole();
    }

//for vehicles
    public void UpdateAmbulanceLocation(int newx,int newz, int origx, int origz)
    {
        Dictionary<String, string> location = new Dictionary<string, string>();
        location["newx"] = newx.ToString();
        location["newz"] = newz.ToString();
        location["origx"]=origx.ToString();
        location["origz"]=origz.ToString();
        location["name"]=StaticInfo.name;
        location["room"]=StaticInfo.roomNumber;

        socket.Emit("UpdateAmbulanceLocation", new JSONObject(location));
        Debug.Log("update ambulance location");
    }

    public void AskForRide(int origx, int origz, int newX, int newZ)
    {
        this.toX = newX;
        this.toZ = newZ;
        this.fromX = origx;
        this.fromZ = origz;
        Dictionary<String, string> location = new Dictionary<string, string>();
        location["origx"]=origx.ToString();
        location["origz"]=origz.ToString();
        location["room"]=StaticInfo.roomNumber;
        location["name"]=StaticInfo.name;
        Debug.Log("I'm at gm.askforride, the toX, toZ, fromx, fromz are: " + toX + " " + toZ + " " + fromX + " " + fromZ);
        socket.Emit("AskForRide", new JSONObject(location));
    }

    public void AskForRide_Success(SocketIOEvent obj){
        string driver = obj.data.ToDictionary()["driver"];
        Debug.Log(driver);
        Debug.Log(StaticInfo.name);
        operationManager.askingForRide=true;
        if (!StaticInfo.name.Equals(driver))
        {
            // Debug.Log(obj.data["targetNames"]);
            List<string> names=parseJsonArray(obj.data["targetNames"]);
            int nRider = Convert.ToInt32(obj.data.ToDictionary()["nRider"]);
            Debug.Log("I'm at AskForRide_Success in gm, " + "names count:" + names.Count + " nRider is : " + nRider);
            if(names.Count>0)
            {
                foreach(string n in names){
                    if (n.Equals(StaticInfo.name)&& !StaticInfo.level.Equals("Family")){
                        Debug.Log("same name");
                        opPanel.SetActive(true);
                        Operation op = new Operation(operationManager, OperationType.Ride);
                        Button newObject = this.instantiateOp(op.prefab, options[0].transform, true);
                        newObject.onClick.AddListener(ride);
                        string tip="Ride";
                        Vector3 position=options[0].transform.position;
                        EventTrigger trigger= newObject.gameObject.GetComponent<EventTrigger>();
                        EventTrigger.Entry entry= new EventTrigger.Entry();
                        EventTrigger.Entry exit=new EventTrigger.Entry();
                        entry.eventID = EventTriggerType.PointerEnter;
                        entry.callback = new EventTrigger.TriggerEvent();
                        exit.eventID = EventTriggerType.PointerExit;
                        exit.callback = new EventTrigger.TriggerEvent();
                        UnityEngine.Events.UnityAction<BaseEventData> l_callback = new UnityEngine.Events.UnityAction<BaseEventData>((eventData)=>operationManager.OnSelectOption(tip,position));
                        entry.callback.AddListener(l_callback);
                        UnityEngine.Events.UnityAction<BaseEventData> exit_callback = new UnityEngine.Events.UnityAction<BaseEventData>((eventData)=>operationManager.OnMouseExit());
                        exit.callback.AddListener(exit_callback);
                        trigger.triggers.Add(entry);
                        trigger.triggers.Add (exit);

                        // Debug.Log("ask for ride succeed!!!!!!!");
                    }
                }
            }
        }
        else
        {
            int nRider = Convert.ToInt32(obj.data.ToDictionary()["nRider"]);
            if (nRider == 0){
                Debug.Log("i'm driver, Im here!!!!!");
                if (tileMap.tiles[fromX/6, fromZ/6]==4){
                    UpdateAmbulanceLocation(toX, toZ, fromX, fromZ);
                }
                if (tileMap.tiles[fromX/6,fromZ/6]==3){
                    UpdateEngineLocation(toX, toZ, fromX, fromZ);
                }
            }
            else{
                confirmed = nRider;
                Debug.Log("I'm at AskForRide_Success in gm, confirmed for" + StaticInfo.name + " who is the driver" + driver + " has confirmed :" + confirmed );
            }
        }
        // Debug.Log("confirmed:" + this.confirmed);
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
        Debug.Log("Im at startRide, the type is " + " " + type + " type 1 is ambulance" + " my name is " + StaticInfo.name + " I'm going to ride" );
    }

    public void ConfirmRide(SocketIOEvent obj){

            confirmed--;
            operationManager.askingForRide=false;
            if(confirmed==0)
            {
                // if(obj.data.ToDictionary()["type"].Equals("0"))
                // {
                //     if(tileMap.selectedUnit.driving==2)
                //     {
                //         UpdateEngineLocation(toX, toZ, fromX, fromZ);
                //         fireman.s.transform.position = new Vector3(toX, 0.2f, toZ);
                //         fireman.currentX=toX;
                //         fireman.currentZ=toZ;
                //         UpdateLocation(toX, toZ,StaticInfo.name);  
                //     }
                //     if(tileMap.selectedUnit.driving==1)
                //     {
                //         UpdateAmbulanceLocation(toX, toZ, fromX, fromZ);
                //         fireman.s.transform.position = new Vector3(toX, 0.2f, toZ);
                //         fireman.currentX=toX;
                //         fireman.currentZ=toZ;
                //         UpdateLocation(toX, toZ,StaticInfo.name);
                //     }
                // }
                if(obj.data.ToDictionary()["type"].Equals("2")){

                    UpdateEngineLocation(toX, toZ, fromX, fromZ);
                    fireman.s.transform.position = new Vector3(toX, 0.2f, toZ);
                    fireman.currentX=toX;
                    fireman.currentZ=toZ;
                    UpdateLocation(toX, toZ,StaticInfo.name);        
                }
                if(obj.data.ToDictionary()["type"].Equals("1")){
                    UpdateAmbulanceLocation(toX, toZ, fromX, fromZ);
                    fireman.s.transform.position = new Vector3(toX, 0.2f, toZ);
                    fireman.currentX=toX;
                    fireman.currentZ=toZ;
                    UpdateLocation(toX, toZ,StaticInfo.name);
                }
                if(obj.data.ToDictionary()["type"].Equals("0")){
                    if(tileMap.tiles[tileMap.selectedUnit.currentX/6, tileMap.selectedUnit.currentZ/6]==3)
                    {
                        UpdateEngineLocation(toX, toZ, fromX, fromZ);
                        fireman.s.transform.position = new Vector3(toX, 0.2f, toZ);
                        fireman.currentX=toX;
                        fireman.currentZ=toZ;
                        UpdateLocation(toX, toZ,StaticInfo.name);  
                    }
                    if(tileMap.tiles[tileMap.selectedUnit.currentX/6, tileMap.selectedUnit.currentZ/6]==4)
                    {
                        UpdateAmbulanceLocation(toX, toZ, fromX, fromZ);
                        fireman.s.transform.position = new Vector3(toX, 0.2f, toZ);
                        fireman.currentX=toX;
                        fireman.currentZ=toZ;
                        UpdateLocation(toX, toZ,StaticInfo.name);
                    }
                }
                Debug.Log(obj.data.ToDictionary()["type"]);
                // Dictionary<String, string> data = new Dictionary<string, string>();
                // data["room"] = StaticInfo.roomNumber.ToString();
                // socket.Emit("ResetConfirmed", new JSONObject(data));
            }
            Debug.Log("Hello, im at confirmride in gm, my name is " + StaticInfo.name + " my confirmed number is " + confirmed);

        
        
    }

    public void ResetConfirmed_Success(SocketIOEvent obj){
        this.confirmed = 0;
        // tileMap.selectedUnit.driving=0;
        Debug.Log(StaticInfo.name + "is resetting confirmed to  " + this.confirmed);
    }

    public void UpdateAmbulanceLocation_Success(SocketIOEvent obj)
    {
        Debug.Log("hello,update ambulance");
        int newx = Convert.ToInt32(obj.data.ToDictionary()["newx"]);
        int newz = Convert.ToInt32(obj.data.ToDictionary()["newz"]);


        List<string> names=parseJsonArray(obj.data["names"]);
        Debug.Log(" wait, the names.count is" + names.Count);
        if(names.Count>0&&!tileMap.ambulance.isRemoted)
        {
            foreach(var name in names){
                if(name.Equals(StaticInfo.name)&&!StaticInfo.StartingAmbulancePosition){
                    tileMap.selectedUnit.s.transform.position=new Vector3(newx,0.2f,newz);
                    Debug.Log("HELLO im driver, im in UpdateAmbulanceLocation_Success, my name is:" + StaticInfo.name);
                    tileMap.selectedUnit.currentX = newx;
                    tileMap.selectedUnit.currentZ = newz;
                    UpdateLocation(newx, newz, StaticInfo.name);
                    tileMap.selectedUnit.riding=false;
                    tileMap.selectedUnit.driving=false;
                }
            }
        }

        if(!StaticInfo.StartingAmbulancePosition){
            tileMap.ambulance.moveNextStation(newx/6, newz/6);
        }
        StaticInfo.StartingAmbulancePosition=false;
        tileMap.ambulance.isRemoted=false;
        confirmed=0;
        // Debug.Log("update ambulance !!!!!");
        Debug.Log(StaticInfo.name + "is going to next station!!!!!!!!!!!!!!!!!" + "names count is:" + names.Count + "!!!!!");
        // confirmed=0;
        Dictionary<String, string> data = new Dictionary<string, string>();
        data["room"] = StaticInfo.roomNumber.ToString();
        socket.Emit("ResetConfirmed", new JSONObject(data));
    }

    public void UpdateEngineLocation(int newx,int newz, int origx, int origz)
    {
        Dictionary<String, string> location = new Dictionary<string, string>();
        location["newx"] = newx.ToString();
        location["newz"] = newz.ToString();
        location["origx"]=origx.ToString();
        location["origz"]=origz.ToString();
        location["name"]=StaticInfo.name;
        location["room"]=StaticInfo.roomNumber;

        socket.Emit("UpdateEngineLocation", new JSONObject(location));
        Debug.Log("update eng location");

    }

    public void UpdateEngineLocation_Success(SocketIOEvent obj)
    {
        int newx = Convert.ToInt32(obj.data.ToDictionary()["newx"]);
        int newz = Convert.ToInt32(obj.data.ToDictionary()["newz"]);


        List<string> names=parseJsonArray(obj.data["names"]);
        if(names.Count>0)
        {
            foreach(var name in names){
                if(name.Equals(StaticInfo.name)&&!StaticInfo.StartingEnginePosition){
                    tileMap.selectedUnit.s.transform.position=new Vector3(newx,0.2f,newz);
                    tileMap.selectedUnit.currentX = newx;
                    tileMap.selectedUnit.currentZ = newz;
                    UpdateLocation(newx, newz, StaticInfo.name);
                    tileMap.selectedUnit.riding=false;
                    tileMap.selectedUnit.driving=false;
                }
            }
        }

        if(!StaticInfo.StartingEnginePosition){
            tileMap.engine.moveNextStation(newx/6, newz/6);
            Debug.Log("im driver?" + isOwner+ " WHY IM HEREEEEEEEEEE" + "StartingEnginePosition is " + StaticInfo.StartingEnginePosition);
        }
        StaticInfo.StartingEnginePosition=false;
        confirmed=0;
        Dictionary<String, string> data = new Dictionary<string, string>();
        data["room"] = StaticInfo.roomNumber.ToString();
        socket.Emit("ResetConfirmed", new JSONObject(data));
    }


    void checkingTurn_Success(SocketIOEvent obj)
    {
        //accept value here
        var result = obj.data.ToDictionary()["status"];
        Debug.Log(result);

        if (result.Equals("True"))
        {
            isMyTurn = true;
            // if(!StaticInfo.StartingPosition&&!StaticInfo.StartingAmbulancePosition&&!StaticInfo.StartingEnginePosition&&isMyTurn){
            //     changeRoleButton.SetActive(true);
            // }
            // if(!StaticInfo.StartingPosition&&!StaticInfo.StartingAmbulancePosition&&!StaticInfo.StartingEnginePosition&&!isMyTurn){
            //     changeRoleButton.SetActive(false);
            // }
        }
        else
        {
            isMyTurn = false;
            changeRoleButton.SetActive(false);
            // if(!StaticInfo.StartingPosition&&!StaticInfo.StartingAmbulancePosition&&!StaticInfo.StartingEnginePosition&&isMyTurn){
            //     changeRoleButton.SetActive(true);
            // }
            // if(!StaticInfo.StartingPosition&&!StaticInfo.StartingAmbulancePosition&&!StaticInfo.StartingEnginePosition&&!isMyTurn){
            //     changeRoleButton.SetActive(false);
            // }
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
			// Extinguish any fire that might be outside (should only trigger on first turn)
			fireManager.extOutFire();

			// Vicinity related checks until just after else:
			if (vicinityManager.checkIfInVicinity(fireman.currentX / 6, fireman.currentZ / 6))
			{
				// Fireman starts turn in vicinity of Veteran
				fireman.vetAPNotYetGiven = false;
				fireman.FreeAP++;
				fireman.inVetZone = true;
			}
			else  // This might be needed after a knockdown:
			{
				fireman.vetAPNotYetGiven = true;
				fireman.inVetZone = false;
			}
			fireman.usedVetAP = false;

			if(!StaticInfo.level.Equals("Family")) changeRoleButton.SetActive(true);
            endOfTurn=false;
        }
        else
        {
            isMyTurn = false;
            changeRoleButton.SetActive(false);
            Debug.Log("It is now someone else's turn!");
            fireman.refreshAP();
            endOfTurn=false;
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
            if(!StaticInfo.level.Equals("Family")){
                changeRoleButton.SetActive(true);
            }
            
            // fireman.refreshAP();
        }
        else
        {
            isMyTurn = false;
            changeRoleButton.SetActive(false);
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
        Dictionary<string,string> join=new Dictionary<string, string>();
        join["room"]=StaticInfo.roomNumber;
        join["name"]=StaticInfo.name;
        socket.Emit("JoinGame",new JSONObject(join));

        yield return new WaitForSeconds(0.5f);

    }

    
    public void JoinGame_Success(SocketIOEvent obj){
        Debug.Log("in join game");
        string owner=obj.data.ToDictionary()["owner"];
        string room=obj.data.ToDictionary()["room"];
        displayAP();
        if(room.Equals(StaticInfo.roomNumber)){
            if(owner.Equals(StaticInfo.name)){
                if (!StaticInfo.LoadGame)//if load game, then dont need to initialize board, just use the board specified in start function
                {
                    InitiateBoard();
                }
                
            }
        }
    }

    public void InitiateBoard(){
        Debug.Log("Initiating Board");
        if(StaticInfo.level.Equals("Family")){ // Family
            //1. Place Fire
            tileMap.InitializeFamily();
            //2. POI
            pOIManager.initiatePOI();
        }else{ // Experienced
            //1. Place Explosion
            tileMap.InitializeExperienced();
            //2. Hazmat/hotspot
            hazmatManager.initiateHazmat();
            //3. poi
            pOIManager.initiatePOI();
        }
        string json=JsonUtility.ToJson(pOIManager);
        Debug.Log(json);
    }

    public Fireman initializeFireman()
    {
        var location = players[StaticInfo.name]["Location"].ToString();
        location = location.Substring(1, location.Length - 2);
        //Debug.Log(location);

        var cord = location.Split(',');
        int x = Convert.ToInt32(cord[0]);
        int z = Convert.ToInt32(cord[1]);

        // int ap = Convert.ToInt32(players[StaticInfo.name]["AP"].ToString());
        // Debug.Log("Created '" + StaticInfo.name + "' with AP =" + ap + "in location " + x + " " + z);
        Fireman f = new Fireman(StaticInfo.name, Colors.Blue, firemanObject, firemanplusObject, x, z, this, StaticInfo.role,pOIManager, hazmatManager);

        return f;
    }

    public Ambulance initializeAmbulance(int load)
    {
        Ambulance amb;
        if (load == 0)
        {
            amb = new Ambulance(ambulance, 54, 21, this);
        }
        else
        {
            Debug.Log(StaticInfo.ambulance.ToString());
            Debug.Log(Convert.ToInt32(StaticInfo.ambulance.ToArray()[0].Value));
            Debug.Log(Convert.ToInt32(StaticInfo.ambulance.ToArray()[1].Value));
            amb = new Ambulance(ambulance, 54, 21, this);
            amb.moveNextStation(Convert.ToInt32(StaticInfo.ambulance.ToArray()[0].Value)/6,Convert.ToInt32(StaticInfo.ambulance.ToArray()[1].Value)/6);
        }
        

        return amb;
    }

    public Engine initializeEngine(int load)
    {
        Engine eng;
        if (load == 0)
        {
            eng= new Engine(engine, 0, 33, this);
        }
        else
        {
            Debug.Log(Convert.ToInt32(StaticInfo.engine.ToArray()[0].Value));
            Debug.Log(Convert.ToInt32(StaticInfo.engine.ToArray()[1].Value));
            eng= new Engine(engine, 0, 33, this);
            eng.moveNextStation(Convert.ToInt32(StaticInfo.engine.ToArray()[0].Value)/6,
                Convert.ToInt32(StaticInfo.engine.ToArray()[1].Value)/6);
        }
            

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
        // update["role"] = ((int)StaticInfo.role).ToString();

        socket.Emit("Location", new JSONObject(update));
    }

    public void UpdateTile(int x, int z, int type)
    {
        Debug.Log("Update tile");
        Dictionary<String, string> updateTile = new Dictionary<string, string>();
        updateTile["x"] = x.ToString();
        updateTile["z"] = z.ToString();
        updateTile["type"] = type.ToString();
        updateTile["room"] = StaticInfo.roomNumber;

        socket.Emit("UpdateTile", new JSONObject(updateTile));
    }

    public void UpdateWall(int x, int z, int type, int horizontal, bool fromExplosion)
    {
        Debug.Log("Update wall");
        Dictionary<String, string> updateWall = new Dictionary<string, string>();
        updateWall["x"] = x.ToString();
        updateWall["z"] = z.ToString();
        updateWall["type"] = type.ToString();
        updateWall["horizontal"] = horizontal.ToString();
        updateWall["room"] = StaticInfo.roomNumber;
        updateWall["fromExplosion"]=fromExplosion.ToString();

        socket.Emit("UpdateWall", new JSONObject(updateWall));
    }

    public void UpdateDoor(int x, int z, int toType, int type, bool fromExplosion)
    {
        Debug.Log("Update door");
        Dictionary<String, string> updateDoor = new Dictionary<string, string>();
        updateDoor["x"] = x.ToString();
        updateDoor["z"] = z.ToString();
        updateDoor["toType"] = toType.ToString();
        updateDoor["type"] = type.ToString();
        updateDoor["room"] = StaticInfo.roomNumber;
        updateDoor["fromExplosion"]=fromExplosion.ToString();

        socket.Emit("UpdateDoor", new JSONObject(updateDoor));
    }


    public void updateRevealPOI(int x,int z)
    {
        Debug.Log("update after reveal POI");
        Dictionary<String, string> revealPOI = new Dictionary<string, string>();
        revealPOI["x"] = x.ToString();
        revealPOI["z"] = z.ToString();
        revealPOI["room"] = StaticInfo.roomNumber;

        socket.Emit("RevealPOI", new JSONObject(revealPOI));
    }


    // ---------------
    // --   DODGE   --
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

    // Fetch players' location:
    public void fetchLocations()
    {
        Debug.Log("Printing locations:");
        int i = 0;

        foreach (string o in players.Keys)
        {
            //Debug.Log("  >" + players[o]["Location"].ToString() + "<");

            // Parse data from players' JSON object
            var location = players[o]["Location"].ToString();
            location = location.Substring(1, location.Length - 2);
            var cord = location.Split(',');

            // Update gm's global arry
            playerLocations[i, 0] = Convert.ToInt32(cord[0]);
            playerLocations[i, 1] = Convert.ToInt32(cord[1]);

            // Parse name
            names[i] = o;

            // Parse role
            rolesArr[i] = players[o]["Role"].ToString();
            rolesArr[i] = rolesArr[i].Substring(1, rolesArr[i].Length - 2);
            
            //Debug.Log("roleArr[" + i + "]  ->  \"" + rolesArr[i] + "\"");
            //Debug.Log("fetchLocations: (x, z)  ->  (" + playerLocations[i, 0] + ", " + playerLocations[i, 1] + ")");
            
            // Iterate
            i++;
        }
    }


    // Called from below in case Fireman chooses not to dodge or cannot: knock them out & send them to lower ambulance unit
    public void knockDown(int x_elem, int z_elem, bool currentPlayer, String name)
    {
        Debug.Log("knockDown -> Looking at '" + name + "'. IsCurrentPlayer  => " + currentPlayer);

        // Kill any POI the fireman is carrying:
        if (tileMap.selectedUnit.carryingVictim || tileMap.selectedUnit.leadingVictim)
        {
            pOIManager.kill(x_elem, z_elem);
        }

        int[] nearestAmbulance = vicinityManager.findAmbulanceSpot(x_elem, z_elem);
        if(StaticInfo.level.Equals("Family")){
            UpdateLocation(nearestAmbulance[0] * 6, nearestAmbulance[1] * 6, name);
        }
        else{
            int x = amB.x/6*6;
            int z = amB.z/6*6;
            UpdateLocation(x, z, name);
        }

        /*
        // Northern parking spot
        if (z_elem <= 3)
        {
            if (currentPlayer)
            {
                //tileMap.selectedUnit.s.transform.position = new Vector3(0 * 6, 0.2f, 3 * 6);
                fireman.currentX = 0;
                fireman.currentZ = 18;
            }
            UpdateLocation(0 * 6, 3 * 6, name);
        }
        else // Southern/second parking spot
        {
            if (currentPlayer)
            {
                //tileMap.selectedUnit.s.transform.position = new Vector3(54, 0.2f, 24);
                fireman.currentX = 54;
                fireman.currentZ = 24;
            }
            UpdateLocation(9 * 6, 4 * 6, name);
        }
        */
    }

    // Victims and POIs in spaces with Fire markers are 'Lost' (killed/destroyed)
    public IEnumerator knockDown()
    {
        Debug.Log("");
        Debug.Log("");

        // Check the whole grid:
        for (int x_elem = 0; x_elem < mapSizeX; x_elem++)
        {
            for (int z_elem = 0; z_elem < mapSizeZ; z_elem++)
            {
                // Check each player:
                for (int p_elem = 0; p_elem < numPlayers; p_elem++) {
                    // Setup local/temp variables to check all players:
                    int x_coord = playerLocations[p_elem, 0] / 6;
                    int z_coord = playerLocations[p_elem, 1] / 6;

                    // Sanity checks:
                    //Debug.Log("p_elem.(x, z)  ->  " + p_elem + ".(" + playerLocations[p_elem, 0]  + ", " + playerLocations[p_elem, 1] +")");
                    //Debug.Log("tileMap.tiles[" + x_elem + ", " + z_elem + "] -> " + tileMap.tiles[x_elem, z_elem]);
                    if (x_coord == x_elem && z_elem == z_coord) Debug.Log("Coordinates are the same: " + x_coord + ", " + z_coord);

                    if (x_coord == x_elem && z_elem == z_coord && tileMap.tiles[x_elem, z_elem] == 2)
                    {
                        // Used later to know which player to move:
                        bool currentPlayer = (x_coord == fireman.currentX / 6 && z_coord == fireman.currentZ / 6) ? true : false;
                        Debug.Log("currentPlayer -> " + currentPlayer);
                        Debug.Log("Can dodge  -> " + canDodge(x_elem, z_elem));
                        Debug.Log("In vicinity  -> " + vicinityManager.checkIfInVicinity(x_coord, z_coord));

                        // Check if the player is able to dodge:
                        if ((rolesArr[p_elem] == "9" || vicinityManager.checkIfInVicinity(x_coord, z_coord)) && canDodge(x_elem, z_elem) && Math.Min(fireman.FreeAP, 4) >= 1)
                        {
                            // Allow the active player to choose/begin trying to dodge etc.
                            isDodging = true;

                            toggleActiveDodge.activateGUI();
                            // Check if player wants to dodge
                            Debug.Log("    VET (1) Please decide if you'd like to dodge or not! " + Time.time);
                            yield return new WaitUntil(() => confirmDodgeDown == true);

                            // Player has chosen to dodge
                            if (wantDodge)
                            {
                                Debug.Log("    VET (2) Please press a dodge button: " + Time.time);
                                yield return new WaitUntil(() => buttonDown() == true);
                                fireman.setAP(fireman.FreeAP - 1);      // Spending the 1AP
                                Debug.Log("    VET (3) Finished!" + Time.time);

                                // Need to drop Victim or Hazmat. NB Fireman can 'fully' dodge if leading treated victim
                                if (fireman.carryingVictim)
                                {
                                    pOIManager.dropPOI(fireman.currentX / 6, fireman.currentZ / 6);
                                    StopCarry(fireman.currentX / 6, fireman.currentZ / 6);
                                }
                                if (fireman.carryingHazmat)
                                {
                                    hazmatManager.dropHazmat(fireman.currentX / 6, fireman.currentZ / 6);
                                    StopCarryH(fireman.currentX / 6, fireman.currentZ / 6);
                                }

                                if(fireman.leadingVictim){
                                    pOIManager.dropPOI(fireman.currentX/6,fireman.currentZ/6);
                                    StopLead(fireman.currentX / 6, fireman.currentZ / 6);
                                }

                                // Player has chosen to move to:
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
                                knockDown(x_elem, z_elem, currentPlayer, names[p_elem]);
                            }

                            toggleActiveDodge.deactivateGUI();
                        }
                        // Player is unable to dodge:
                        else
                        {
                            Debug.Log("Unable to dodge");
                            knockDown(x_elem, z_elem, currentPlayer, names[p_elem]);
                        }
                    }
                }
            }
        }

        // Reset for later use
        Debug.Log("Resetting dodge");
        resetDodge();

        // Update vicinity check if player is playing a Veteran currently
        if (fireman.role == Role.Veteran) {
            vicinityManager.updateVicinityArr(fireman.currentX / 6, fireman.currentZ / 6);
        }

        Debug.Log("");
        Debug.Log("");

        // Kill the thread
        yield return 0;
    }

    // Function to end/pass the turn - launches a coroutine to check for knockdowns
    public void EndTurn()
    {
        Debug.Log("Ending Turn");

        checkTurn();
        if(isMyTurn){
            // BEGIN OF WIP

            // Veteran-given AP cannot be saved:
            if (fireman.inVetZone && !fireman.usedVetAP)
            {
                Debug.Log("Unable to save AP from 'experience'");
                fireman.setAP(fireman.FreeAP - 1);
            }


            System.Random rand=new System.Random();
            int x=rand.Next(1,8);
            int z=rand.Next(1,6);

            // Change the below 'true' to false if you want random. true is used to test specific values
            fireManager.advanceFire(x, z, true);
            //fireman.usedVetAP = false;

            // Update firefighter's current locations
            fetchLocations();

            // Check if someone is knocked down
            StartCoroutine(knockDown());

            // Re-check:
            fetchLocations();

            Debug.Log("Finished advFire, redistributing AP");

            // END OF WIP

            operationManager.commandMoves = 1;
            operationManager.controlled = null;
            operationManager.inCommand = false;

            pOIManager.replenishPOI();
            operationManager.DestroyAll();

            endOfTurn=true;

            changeTurn();

        }

        // BEGIN OF WIP

        // Veteran-given AP cannot be saved:
        // if (fireman.inVetZone && !fireman.usedVetAP)
        // {
        //  Debug.Log("Unable to save AP from 'experience'");
        //  fireman.setAP(fireman.FreeAP - 1);
        // }


        // //System.Random rand=new System.Random();
        // //int x=rand.Next(1,8);
        // //int z=rand.Next(1,6);

        // // Change the below 'true' to false if you want random. true is used to test specific values
        // fireManager.advanceFire(1, 3, true);
        // //fireman.usedVetAP = false;

        // // Update firefighter's current locations
        // fetchLocations();

        // // Check if someone is knocked down
        // StartCoroutine(knockDown());

        // // Re-check:
        // fetchLocations();

        // Debug.Log("Finished advFire, redistributing AP");

        // // END OF WIP

        // operationManager.commandMoves = 1;
        // operationManager.controlled = null;
        // operationManager.inCommand = false;

        // pOIManager.replenishPOI();
        // operationManager.DestroyAll();

        // endOfTurn=true;

        // checkTurn();
        // //do stuff here...

        // if (isMyTurn)
        // {
        //     changeTurn();
        // }
        
        // else
        // {
        //     Debug.Log("This not your turn! Don't click end turn!");
        // }
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
        treat["room"] = StaticInfo.roomNumber;
        
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
        location["name"] = StaticInfo.name;
        location["room"] = StaticInfo.roomNumber;

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
        location["room"] = StaticInfo.roomNumber;

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
        Debug.Log("Removed Hazmat" + hazmatManager.removedHazmat);
        Dictionary<String, string> hazmat = new Dictionary<string, string>();
        hazmat["x"] = x.ToString();
        hazmat["z"] = z.ToString();
        hazmat["room"] = StaticInfo.roomNumber;

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
        location["room"] = StaticInfo.roomNumber;

        socket.Emit("UpdateHazmatLocation", new JSONObject(location));
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
        // tileMap.selectedUnit.driving = type;
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
        carry["room"] = StaticInfo.roomNumber;
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

    public void StopCarryH(int x, int z){
        Dictionary<string,string> carry=new Dictionary<string, string>();
        carry["room"]=StaticInfo.roomNumber;
        carry["name"]=StaticInfo.name;
        carry["x"]=x.ToString();
        carry["z"]=z.ToString();
        socket.Emit("StopCarryH",new JSONObject(carry));
    }

    public void StopCarryH_Success(SocketIOEvent obj){
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
        hazmatManager.dropHazmat(x,z);
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
        poi["room"] = StaticInfo.roomNumber;

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
        poi["room"] = StaticInfo.roomNumber;

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
        // tileMap.selectedUnit.driving = 0;
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
                if(fireman.carryingVictim){
                    pOIManager.dropPOI(fireman.currentX/6,fireman.currentZ/6);
                    this.StopCarry(fireman.currentX/6,fireman.currentZ/6);
                }
                if(fireman.leadingVictim){
                    pOIManager.dropPOI(fireman.currentX/6,fireman.currentZ/6);
                    this.StopLead(fireman.currentX/6,fireman.currentZ/6);
                }
                if(fireman.carryingHazmat){
                    hazmatManager.dropHazmat(fireman.currentX/6,fireman.currentZ/6);
                    this.StopCarryH(fireman.currentX/6,fireman.currentZ/6);
                }
                int vx=tileMap.engine.x;
                int vz=tileMap.engine.z;
                fireman.currentX=vx/6*6;
                fireman.currentZ=vz/6*6;
                fireman.s.transform.position=new Vector3(fireman.currentX, 0.2f, fireman.currentZ);
                UpdateLocation(fireman.currentX,fireman.currentZ,fireman.name);
                displayRole();
                displayAP();
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

        if(!StaticInfo.level.Equals("Family")){
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
        rescue["room"] = StaticInfo.roomNumber;
        rescue["room"] = StaticInfo.roomNumber;
        socket.Emit("RescueCarried",new JSONObject(rescue));
    }

    public void rescueTreated(int x, int z){
        Dictionary<string,string> rescue=new Dictionary<string, string>();
        rescue["x"]=x.ToString();
        rescue["z"]=z.ToString();
        rescue["room"] = StaticInfo.roomNumber;
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

    public void killPOI(int x, int z){
        Dictionary<string,string> kill=new Dictionary<string, string>();
        kill["x"]=x.ToString();
        kill["z"]=z.ToString();
        kill["room"] = StaticInfo.roomNumber;
        socket.Emit("KillPOI",new JSONObject(kill));
    }

    public void killPOI_Success(SocketIOEvent obj){
        int x=Convert.ToInt32(obj.data.ToDictionary()["x"]);
        int z=Convert.ToInt32(obj.data.ToDictionary()["z"]);
        pOIManager.kill(x,z);
    }


    //check for victory and defeat
    public void victory_Success(SocketIOEvent obj)
    {
        Debug.Log("Update victory");
        if (obj.data.ToDictionary()["room"].Equals(StaticInfo.roomNumber))
        {
            SceneManager.LoadScene("Win");
        }
        
    }

    public void defeat_Success(SocketIOEvent obj)
    {
        Debug.Log("Update defeat");
        if (obj.data.ToDictionary()["room"].Equals(StaticInfo.roomNumber))
        {
            SceneManager.LoadScene("gameOver");
        }
    }

    public void victory()
    {
        Debug.Log("You win!");
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["room"] = StaticInfo.roomNumber;
        socket.Emit("victory",new JSONObject(data));
        //
    }


    public void defeat()
    {
        Debug.Log("Game Over!");
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["room"] = StaticInfo.roomNumber;
        socket.Emit("defeat", new JSONObject(data));
        //;
    }

    public void confirmPosition(){

        int x=(int)fireman.s.transform.position.x;
        int z=(int)fireman.s.transform.position.z;

        fireman.currentX=x;
        fireman.currentZ=z;

        StaticInfo.Location=new int[]{x,z};

        Dictionary<string,string> position=new Dictionary<string, string>();
        position["x"]=x.ToString();
        position["z"]=z.ToString();
        position["room"]=StaticInfo.roomNumber;
        position["name"]=StaticInfo.name;


        socket.Emit("ConfirmPosition",new JSONObject(position));
    }

    public void confirmPosition_Success(SocketIOEvent obj){
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
        tileMap.UpdateFiremanVisual(players);
        displayRole();
        if(obj.data.ToDictionary()["room"].Equals(StaticInfo.roomNumber)){
            StaticInfo.StartingPosition=false;
            startingPositionPanel.SetActive(false);

            if(!StaticInfo.level.Equals("Family")){
                if(isOwner){
                    StaticInfo.StartingAmbulancePosition=true;
                    startingAmbulancePositionPanel.SetActive(true);
                    changeRoleButton.SetActive(true);
                }
            }
        }
    }

    public void confirmAmbulancePosition(){

        int x=(int)amB.v.transform.position.x;
        int z=(int)amB.v.transform.position.z;

        UpdateAmbulanceLocation(x, z, -5, -5);
        startingAmbulancePositionPanel.SetActive(false);
        StaticInfo.StartingEnginePosition=true;
        startingEnginePositionPanel.SetActive(true);
        // StaticInfo.StartingAmbulancePosition=false;

        // tileMap.ambulance.x=x;
        // tileMap.ambulance.z=z;

        // StaticInfo.Location=new int[]{x,z};

        // Dictionary<string,string> position=new Dictionary<string, string>();
        // position["x"]=x.ToString();
        // position["z"]=z.ToString();
        // position["room"]=StaticInfo.roomNumber;
        // // position["name"]=StaticInfo.name;

        // socket.Emit("ConfirmAmbulancePosition",new JSONObject(position));
    }

    public void checkOwner(string name){
        Debug.Log("checking owner");
        Dictionary<String, String> checkingOwner = new Dictionary<string, string>();
        checkingOwner["room"] = StaticInfo.roomNumber;
        checkingOwner["name"] = StaticInfo.name;
        socket.Emit("checkingOwner", new JSONObject(checkingOwner));
    }

    public void checkOwner_Success(SocketIOEvent obj){
        //accept value here
        var result = obj.data.ToDictionary()["owner"];
        // Debug.Log(result);

        if (result.Equals("True"))
        {
            isOwner = true;
        }
        else
        {
            isOwner = false;
        }
        Debug.Log(StaticInfo.name + " is the owner? " + isOwner);
    }



    public void confirmEnginePosition(){
        int x = (int)enG.v.transform.position.x;
        int z = (int)enG.v.transform.position.z;

        UpdateEngineLocation(x,z, -5, -5);
        startingEnginePositionPanel.SetActive(false);
    }

    // public void confirmAmbulancePosition_Success(SocketIOEvent obj){

    //     StaticInfo.StartingAmbulancePosition=false;
    //     StaticInfo.StartingEnginePosition=true;
    //     // room = obj.data["Games"][StaticInfo.roomNumber];
    //     // participants = room["participants"];
    //     // level = room["level"].ToString();
    //     // numberOfPlayer = room["numberOfPlayer"].ToString();

    //     // List<string> p = participants.keys;
    //     // foreach (var v in p)
    //     // {
    //     //     var o = participants[v];
    //     //     players[v] = o;
    //     //     // Debug.Log(v);
    //     //     // Debug.Log(players[v]);
    //     // }
    //     // tileMap.UpdateFiremanVisual(players);
    //     // displayRole();
    //     // if(obj.data.ToDictionary()["room"].Equals(StaticInfo.roomNumber)){
    //     //     StaticInfo.StartingPosition=false;
    //     //     startingPositionPanel.SetActive(false);
    //     //     if(!StaticInfo.level.Equals("Family")){
    //     //         changeRoleButton.SetActive(true);
    //     //     }
    //     // }
    // }


    public void explodeHazmat(int x, int z){
        Dictionary<string,string> data=new Dictionary<string, string>();
        data["x"]=x.ToString();
        data["z"]=z.ToString();
        data["room"]=StaticInfo.roomNumber;
        socket.Emit("ExplodeHazmat",new JSONObject(data));
    }

    public void explodeHazmat_Success(SocketIOEvent obj){
        string room=obj.data.ToDictionary()["room"];
        if(room.Equals(StaticInfo.roomNumber)){
            int x=Convert.ToInt32(obj.data.ToDictionary()["x"]);
            int z=Convert.ToInt32(obj.data.ToDictionary()["z"]);
            hazmatManager.explodeHazmat(x,z);
        }
    }
    
    public void saveGame()
    {
        Debug.Log("saveGame");

        Dictionary<string,string> data=new Dictionary<string, string>();
        data["room"]=StaticInfo.roomNumber;
        data["name"] = StaticInfo.name;
        data["freeAP"] = fireman.FreeAP.ToString();
        data["remainingSpecAp"] = fireman.remainingSpecAp.ToString();
        data["damagedWall"] = wallManager.damagedWalls.ToString();
        data["rescued"] = pOIManager.rescued.ToString();
        data["killed"] = pOIManager.killed.ToString();
        data["removedHazmat"] = hazmatManager.removedHazmat.ToString();
        
        data["riding"] = fireman.riding.ToString();
        data["driving"] = fireman.driving.ToString();
        data["carryingHazmat"] = fireman.carryingHazmat.ToString();
        data["carryingVictim"] = fireman.carryingVictim.ToString();
        data["leadingVictim"] = fireman.leadingVictim.ToString();
        
        socket.Emit("saveGame",new JSONObject(data));

    }

}

public class Notification{
    public string msg;
    public Text textObject;
}