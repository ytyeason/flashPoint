using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ActiveStateToggler : MonoBehaviour {

	public void ToggleActive () {
		gameObject.SetActive (!gameObject.activeSelf);
	}
}
