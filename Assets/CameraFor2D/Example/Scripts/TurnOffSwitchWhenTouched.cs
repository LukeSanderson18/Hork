using UnityEngine;
using System.Collections;

public class TurnOffSwitchWhenTouched : MonoBehaviour {
	public Material turnedOffMaterial;

	void OnTriggerEnter() {
		GetComponent<Collider>().enabled = false;
		GetComponent<Renderer>().sharedMaterial = turnedOffMaterial;
	}
}
