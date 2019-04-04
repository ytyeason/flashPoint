using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
using UnityEngine.SceneManagement;
//using GameObjects;
using UnityEngine.UI;

public class SelectRole : MonoBehaviour {

    public SocketIOComponent socket;

    public Text error;

    public Image selected;



    void Start ()
    {

        StartCoroutine(ConnectToServer());
        socket.On("USER_CONNECTED", OnUserConnected );
        socket.On("selectRole_SUCCESS", selectRole_SUCCESS );

    }

    void selectRole_SUCCESS(SocketIOEvent obj)//change scene
    {
        var result = obj.data.ToDictionary()["result"];
        var role = obj.data.ToDictionary()["role"];
        if(result=="false"){
            error.text="Role is NOT available!";
            return;
        }
        error.text="";
        switch(role){
            case "Cafs_Firefighter":
                StaticInfo.role=Role.CAFS;
                break;
            case "Driver_Operation":
                StaticInfo.role=Role.Driver;
                break;
            case "Fire_Captain":
                StaticInfo.role=Role.Captain;
                break;
            case "Generalist":
                StaticInfo.role=Role.Generalist;
                break;
            case "Hazmat_Technician":
                StaticInfo.role=Role.HazmatTech;
                break;
            case "Paramedic":
                StaticInfo.role=Role.Paramedic;
                break;
            case "rescue_dog":
                StaticInfo.role=Role.Dog;
                break;
            case "Rescue_Specialist":
                StaticInfo.role=Role.RescueSpec;
                break;
            case "veteran":
                StaticInfo.role=Role.Veteran;
                break;
        }
        Debug.Log("selectRole successful");
        SceneManager.LoadScene("Room");

    }

    IEnumerator ConnectToServer()
    {
        yield return new WaitForSeconds(0.5f);

        socket.Emit("USER_CONNECT");
		
        yield return new WaitForSeconds(0.5f);
		
    }

    void OnUserConnected (SocketIOEvent obj)
    {
        Debug.Log( "all user born on this client" );
    }

    public void Next()
    {
        String role = selected.overrideSprite.name;
        // StaticInfo.role=role;
        
        Debug.Log(role);
        
        Dictionary<String, String> selectedRole = new Dictionary<string, string>();
        selectedRole["role"] = role;
        selectedRole["room"] = StaticInfo.roomNumber;
        socket.Emit("SelectRole",new JSONObject(selectedRole));
    }


}
