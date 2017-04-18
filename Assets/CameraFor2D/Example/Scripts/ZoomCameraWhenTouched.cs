using UnityEngine;
using System.Collections;
using GoodStuff.NaturalLanguage;

public class ZoomCameraWhenTouched : MonoBehaviour {
	public CameraController2D cameraController;
	public float zoomAmount;
	public float zoomTime = 1.5f;

	float disabledUntilTime;

	void Start() {
		if(cameraController == null) {
			cameraController = Camera.main.GetComponent<CameraController2D>();
		}
	}

	void OnTriggerEnter() {
		if(Time.time > disabledUntilTime) {
			StartCoroutine(ZoomOutAndIn());
			disabledUntilTime = Time.time + 3;
		}
	}

	public IEnumerator ZoomOutAndIn() {
		var elapsedTime = 0f;

		while(elapsedTime < zoomTime) {
			elapsedTime += Time.deltaTime;
			cameraController.DistanceMultiplier = Mathf.Lerp(1, zoomAmount, elapsedTime / zoomTime);
			yield return new WaitForEndOfFrame();
		}

		elapsedTime = 0;
		while(elapsedTime < zoomTime) {
			elapsedTime += Time.deltaTime;
			cameraController.DistanceMultiplier = Mathf.Lerp(1, zoomAmount, 1 - (elapsedTime / zoomTime));
			yield return new WaitForEndOfFrame();
		}

		cameraController.DistanceMultiplier = 1;
	}
}