using UnityEngine;
using System.Collections;

public class cameraScript : MonoBehaviour {
	private Vector3 currentPosition = Vector3.zero;
	private Vector3 nextPosition = Vector3.zero;
	private float lerpTime = 1.0f;
	private bool cameraIsInit = false;


	public void initCamera(){
		Debug.Log ("init camera");
		Debug.Log (GetComponentInParent<PlayField> ().currentlySelectedUnit);
		cameraIsInit = true;
	}

	// Update is called once per frame
	void Update () {
		if (cameraIsInit) {
			Vector3 selectedUnitTransform = GetComponentInParent<PlayField> ().currentlySelectedUnit.transform.position;
			if (selectedUnitTransform != transform.position - new Vector3 (0f, 10f, 0f) && lerpTime >= 1.0f) {
				Debug.Log ("Next Unit");
				currentPosition = transform.position;
				nextPosition = selectedUnitTransform + new Vector3 (0f, 10f, 0f);
				lerpTime = 0f;
			} else {
				if (lerpTime < 1.0f) {
					lerpTime += 0.1f;
					Debug.Log (lerpTime);
					transform.position = Vector3.Slerp (currentPosition, nextPosition, lerpTime); 
				} else {
					transform.position = selectedUnitTransform + new Vector3 (0f, 10f, 0f);
				}
			}
		}
	}
}
