using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleActiveDodge : MonoBehaviour {
	// Backdrops
	public GameObject backdropL;	// For the dodge GameObjects
	public GameObject backdropS;	// For dodge confirmations

	// Dodge GameObjects
	public GameObject leftDodge;
	public GameObject upDodge;
	public GameObject rightDodge;
	public GameObject downDodge;

	// Confirm buttons:
	public GameObject confirmDodge;
	public GameObject declineDodge;

	// Connect to GameManager
	public GameManager gm;

	public ToggleActiveDodge(GameManager in_gm, GameObject in_backdropL, GameObject in_backdropS,
								GameObject in_leftDodge, GameObject in_upDodge, GameObject in_downDodge, GameObject in_rightDodge,
								GameObject in_confirmDodge, GameObject in_declineDodge)
	{
		// Init:
		backdropL = in_backdropL;
		backdropS = in_backdropS;
		leftDodge = in_leftDodge;
		upDodge = in_upDodge;
		rightDodge = in_rightDodge;
		downDodge = in_downDodge;
		confirmDodge = in_confirmDodge;
		declineDodge = in_declineDodge;
		gm = in_gm;

		// Make inactive:
		deactivateGUI();
	}

	// Make active:
	public void activateGUI(){
		backdropL.SetActive(true);
		backdropS.SetActive(true);
		leftDodge.SetActive(true);
		upDodge.SetActive(true);
		rightDodge.SetActive(true);
		downDodge.SetActive(true);
		confirmDodge.SetActive(true);
		declineDodge.SetActive(true);
	}

	// Deactive again:
	public void deactivateGUI()
	{
		backdropL.SetActive(false);
		backdropS.SetActive(false);
		leftDodge.SetActive(false);
		upDodge.SetActive(false);
		rightDodge.SetActive(false);
		downDodge.SetActive(false);
		confirmDodge.SetActive(false);
		declineDodge.SetActive(false);
	}

}
