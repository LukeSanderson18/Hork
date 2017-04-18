using UnityEngine;
using System.Collections;

public class InstantiatePlayer : MonoBehaviour {
	public GameObject playerObject;
	public Transform positionToInstantiateAt;
	public CameraController2D cameraController;
	public bool snapCameraToInstantiationPoint;

	void Start() {
		var player = Object.Instantiate(playerObject, positionToInstantiateAt.position, positionToInstantiateAt.rotation) as GameObject;
		cameraController.SetTarget(player.transform);
		if(snapCameraToInstantiationPoint) {
			cameraController.JumpToTargetRespectingBumpersAndInfluences();
		}
	}
}