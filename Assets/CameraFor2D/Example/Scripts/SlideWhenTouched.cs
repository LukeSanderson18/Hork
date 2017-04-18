using UnityEngine;
using System.Collections;

public class SlideWhenTouched : MonoBehaviour {
	public GameObject target;
	public float slideTime = 2;
	public Vector3 slideDistance = -Vector3.forward;

	void OnTriggerEnter() {
		StartCoroutine(Slide());
	}

	IEnumerator Slide() {
		var originalPosition = target.transform.position;
		var finalPosition = target.transform.position + slideDistance;
		var elapsedTime = 0f;
		while(elapsedTime < slideTime) {
			elapsedTime += Time.deltaTime;
			target.gameObject.transform.position = Vector3.Lerp(originalPosition, finalPosition, elapsedTime / slideTime);
			yield return new WaitForEndOfFrame();
		}
		target.gameObject.transform.position = finalPosition;
	}
}