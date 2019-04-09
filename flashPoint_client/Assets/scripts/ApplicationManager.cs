using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ApplicationManager : MonoBehaviour {
	

	public void Quit () 
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

    public void newGame ()
    {
        Debug.Log("New game button clicked");
	    SceneManager.LoadScene("Lobby");
        StaticInfo.roomNumber = null;
        StaticInfo.role = Role.None;
        StaticInfo.StartingPosition = false;//change to false after wards
        StaticInfo.LoadGame = false;
	    
    }
}
