using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SocketIO;
using System;
using UnityEngine.SceneManagement;
//using GameObjects;
using UnityEngine.UI;

public class gameSetUpManager : MonoBehaviour {

    public SocketIOComponent socket;

    public Text numberOfPlayer;

    public Text level;

    public GameObject dropdown1;
    public Text numOfHazmat;

    public GameObject input1;
    public Text numOfHotspot;

    public GameObject error;
    public GameObject confirm;
    public GameObject errorP;

    private Boolean readyP = false;
    private Boolean readyHot = true;

    void Start ()
    {
        dropdown1.SetActive(false);
        input1.SetActive(false);
        error.SetActive(false);
        confirm.SetActive(false);
        errorP.SetActive(false);

        StartCoroutine(ConnectToServer());
        socket.On("USER_CONNECTED", OnUserConnected );
        socket.On("gameSetUp_SUCCESS", gameSetUpSucessful );
    }

    public void checkLevel()
    {
        if (level.text == "Random")
        {
            dropdown1.SetActive(true);
            input1.SetActive(true);
            confirm.SetActive(false);
            readyHot = false;
        }
        else
        {
            dropdown1.SetActive(false);
            input1.SetActive(false);
            error.SetActive(false);
            readyHot = true;
            if (readyP && readyHot)
            {
                confirm.SetActive(true);
            }
        }
    }

    public void checkInput()
    {
        int numOfHot=0;
        if (Int32.TryParse(numOfHotspot.text,out numOfHot))
        {
            if (numOfHot < 0 || numOfHot > 24)
            {
                error.SetActive(true);
                readyHot = false;
                confirm.SetActive(false);
            }
            else
            {
                readyHot = true;
                if (readyP && readyHot)
                {
                    confirm.SetActive(true);
                }
                error.SetActive(false);
            }
        }
        else
        {
            error.SetActive(true);
            readyHot = false;
            confirm.SetActive(false);
        }
    }

    public void checkInputP()
    {
        int result = 0;
        if (Int32.TryParse(numberOfPlayer.text, out result))
        {
            if (result < 2 || result > 6)
            {
                errorP.SetActive(true);
                readyP = false;
                confirm.SetActive(false);
            }
            else
            {
                readyP = true;
                if (readyP && readyHot)
                {
                    confirm.SetActive(true);
                }
                errorP.SetActive(false);
            }
        }
        else
        {
            errorP.SetActive(true);
            readyP = false;
            confirm.SetActive(false);
        }
    }

    void gameSetUpSucessful(SocketIOEvent obj)//change scene
    {
        var level = obj.data.ToDictionary()["level"];
        Debug.Log("gameSetUp successful");
        if(level=="Family"){
            SceneManager.LoadScene("Room");
            // Debug.Log("Family board");
        }else{
            SceneManager.LoadScene("DragDrop");
            // Debug.Log("Not family");
        }
        
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
        StaticInfo.level = level.text;
        StaticInfo.numberOfPlayer = numberOfPlayer.text;
        StaticInfo.numOfHazmat = numOfHazmat.text;
        StaticInfo.numOfHotspot = numOfHotspot.text;
        
        Debug.Log(StaticInfo.level);
        Debug.Log(StaticInfo.numberOfPlayer);
        
        Dictionary<String, String> gameSetUp = new Dictionary<string, string>();
        gameSetUp["room"] = StaticInfo.roomNumber;
        gameSetUp["name"] = StaticInfo.name;
        gameSetUp["level"] = StaticInfo.level;
        gameSetUp["numberOfPlayer"] = StaticInfo.numberOfPlayer;
        gameSetUp["numberOfHazmat"] = StaticInfo.numOfHazmat;
        gameSetUp["numberOfHotspot"] = StaticInfo.numOfHotspot;

        socket.Emit("gameSetUp",new JSONObject(gameSetUp));
    }


}
