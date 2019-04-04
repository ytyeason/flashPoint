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
            case "3":
                StaticInfo.role=Role.CAFS;
                break;
            case "7":
                StaticInfo.role=Role.Driver;
                break;
            case "1":
                StaticInfo.role=Role.Captain;
                break;
            case "5":
                StaticInfo.role=Role.Generalist;
                break;
            case "4":
                StaticInfo.role=Role.HazmatTech;
                break;
            case "0":
                StaticInfo.role=Role.Paramedic;
                break;
            case "8":
                StaticInfo.role=Role.Dog;
                break;
            case "6":
                StaticInfo.role=Role.RescueSpec;
                break;
            case "9":
                StaticInfo.role=Role.Veteran;
                break;
            case "2":
                StaticInfo.role = Role.ImagingTech;
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
        Role r = Role.None;
        switch (role)
        {
            case "Cafs_Firefighter":
                r = Role.CAFS;
                break;
            case "Driver_Operation":
                r = Role.Driver;
                break;
            case "Fire_Captain":
                r = Role.Captain;
                break;
            case "Generalist":
                r = Role.Generalist;
                break;
            case "Hazmat_Technician":
                r = Role.HazmatTech;
                break;
            case "Paramedic":
                r = Role.Paramedic;
                break;
            case "rescue_dog":
                r = Role.Dog;
                break;
            case "Rescue_Specialist":
                r = Role.RescueSpec;
                break;
            case "veteran":
                r = Role.Veteran;
                break;
            case "Imaging_Technician":
                r = Role.ImagingTech;
                break;
        }

        Dictionary<String, String> selectedRole = new Dictionary<string, string>();
        selectedRole["role"] = ((int)r).ToString();
        selectedRole["room"] = StaticInfo.roomNumber;
        socket.Emit("SelectRole",new JSONObject(selectedRole));
    }


}
