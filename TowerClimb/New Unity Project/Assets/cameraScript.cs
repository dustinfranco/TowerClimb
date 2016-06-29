using UnityEngine;
using System.Collections;

public class cameraScript : MonoBehaviour {
	private float currentHeight = 0f;
	private float nextHeight = 0f;
	private float currentRotation = 0f;
	private float nextRotation = 0f;
	private float lerpTime = 1.0f;
	private float lerpIteration = 0.01f;
	private bool cameraIsInit = false;
	private Transform targetTransform;
	private Transform currentlySelectedTransform;
	private Vector3 veryFirstPosition;
	private Quaternion veryFirstRotation;
	public Transform spookyGhost;

	// Update is called once per frame
	/*
	void Update () {
		if (cameraIsInit) {
			Vector3 selectedUnitTransform = GetComponentInParent<PlayField> ().currentlySelectedUnit.transform.position;
			if (selectedUnitTransform != transform.position - new Vector3 (0f, 0f, 10f) && lerpTime >= 1.0f) {
				Debug.Log ("Next Unit");
				currentPosition = transform.position;
				nextPosition = selectedUnitTransform + new Vector3 (0f, 0f, 10f);
				lerpTime = 0f;
				transform.LookAt (selectedUnitTransform, Vector3.up);
			} else {
				if (lerpTime < 1.0f) {
					lerpTime += 0.1f;
					Debug.Log (lerpTime);
					transform.position = Vector3.Slerp (currentPosition, nextPosition, lerpTime);
					transform.LookAt (selectedUnitTransform, Vector3.up); 
				} else {
					transform.position = selectedUnitTransform + new Vector3 (0f, 0f, 10f);
					transform.LookAt (selectedUnitTransform, Vector3.up);
				}
			}
		}
	}
	*/
	void Update () {
		if (cameraIsInit) {
			
			float selectedUnitHeight = currentlySelectedTransform.position.y;
			float selectedUnitRotation = currentlySelectedTransform.rotation.eulerAngles.y;

			if (currentlySelectedTransform != GetComponentInParent<PlayField> ().currentlySelectedUnit.transform && lerpTime >= 1.0f) {
				Debug.Log ("A");
				currentlySelectedTransform = GetComponentInParent<PlayField> ().currentlySelectedUnit.transform;
				//Debug.Log ("Next Unit");
				currentRotation = transform.rotation.eulerAngles.y;
				nextRotation = currentlySelectedTransform.rotation.eulerAngles.y;
				currentHeight = transform.position.y;
				nextHeight = currentlySelectedTransform.position.y;
				/*
				targetTransform = transform;
				targetTransform.RotateAround(Vector3.zero,Vector3.up,nextRotation-currentRotation);
				targetTransform.position += new Vector3 (0f, nextHeight - currentHeight, 0f);
				*/
				lerpTime = 0f;
			} else {
				if (lerpTime < 1.0f) {
					Debug.Log (lerpIteration);
					Debug.Log (nextRotation - currentRotation);
					lerpTime += lerpIteration;
					transform.RotateAround(Vector3.zero, Vector3.up, (float)lerpIteration * (nextRotation -90f-currentRotation));
					transform.position = new Vector3(transform.position.x, Mathf.Lerp (currentHeight, nextHeight, lerpTime), transform.position.z);
				} else {
					Debug.Log ("C");
					spookyGhost.position = veryFirstPosition;
					spookyGhost.rotation = veryFirstRotation;
					spookyGhost.RotateAround(Vector3.zero, Vector3.up, currentlySelectedTransform.rotation.eulerAngles.y - 90f);
					transform.rotation = spookyGhost.rotation;
					transform.position = spookyGhost.position;
					transform.position = new Vector3 (transform.position.x, selectedUnitHeight, transform.position.z);

				}
			}
		}
	}

	public void initCamera(){
		veryFirstPosition = transform.position;
		veryFirstRotation = transform.rotation;
		Debug.Log ("init camera");
		Debug.Log (GetComponentInParent<PlayField> ().currentlySelectedUnit);
		currentlySelectedTransform = GetComponentInParent<PlayField> ().currentlySelectedUnit.transform;
		targetTransform = transform;
		lerpIteration = GetComponentInParent<PlayField> ().returnLerpIteration ();
		cameraIsInit = true;
	}

}
