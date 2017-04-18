using UnityEngine;
using System.Collections;

public class AddCameraTargetWhenTouched : MonoBehaviour {
	public CameraController2D cameraController;
	public Transform[] targets;
	public float moveSpeed;
	public bool removeTargetAfterDelay;
	public float delay = 5;
	public float revertMoveSpeed;

	public bool triggerSlideAtTarget;
	public GameObject slideTarget;
	public Vector3 slideDistance;
	public float slideTime;
	
	void Start() {
		if(cameraController == null) {
			cameraController = Camera.main.GetComponent<CameraController2D>();
		}
	}

	void OnTriggerEnter() {
		if(removeTargetAfterDelay) {
			if(triggerSlideAtTarget) {
				cameraController.AddTarget(targets, moveSpeed, delay, revertMoveSpeed, StartSlide);
			}
			else {
				cameraController.AddTarget(targets, moveSpeed, delay, revertMoveSpeed);
			}
		}
		else {
			if(triggerSlideAtTarget) {
				cameraController.AddTarget(targets, moveSpeed, StartSlide);
			}
			else {
				cameraController.AddTarget(targets, moveSpeed);
			}
		}
	}

	void StartSlide() {
		StartCoroutine(Slide());
	}

	IEnumerator Slide() {
		var originalPosition = slideTarget.transform.position;
		var finalPosition = slideTarget.transform.position + slideDistance;
		var elapsedTime = 0f;
		while(elapsedTime < slideTime) {
			elapsedTime += Time.deltaTime;
			slideTarget.gameObject.transform.position = Vector3.Lerp(originalPosition, finalPosition, elapsedTime / slideTime);
			yield return new WaitForEndOfFrame();
		}
		slideTarget.gameObject.transform.position = finalPosition;
	}
}