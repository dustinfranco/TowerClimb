using UnityEngine;
using System.Collections;

public class UnitScript : MonoBehaviour {
	public string typeA = "pawn";
	public string typeB = "nothing";
	public int[,] validMovements = new int[10,10];
	public int[,] validAttacks = new int[10,10];
	private float currentHeight;
	private float nextHeight;
	private float currentAngle;
	private float nextAngle;
	private Vector3 veryFirstPosition;
	private Quaternion veryFirstRotation;
	public GameObject spookyGhostObjectUnreal;
	private Transform spookyGhostUnit;



	private float lerpCounter = 1.0f;
	// Update is called once per frame
	private float lerpIteration = 0.01f;
	//private float CurrentRotation = 0.0f;
	//private float NextRotation = 0.0f;
	// Use this for initialization
	void Start () {
		Debug.Log (spookyGhostObjectUnreal);
		GameObject spookyGhostObjectReal = (GameObject)Instantiate (spookyGhostObjectUnreal, Vector3.zero, Quaternion.identity);
		spookyGhostUnit = spookyGhostObjectReal.transform;
		updateValidTables (typeA, typeB);
		veryFirstPosition = new Vector3 (1.53f, 0f, 0f); 
		veryFirstRotation = Quaternion.identity;
	}

	void updateValidTables(string unitTypeA, string unitTypeB){
		//populate the movement table
		validMovements = GetComponent<movementLUT> ().returnValidMovements (unitTypeA, unitTypeB);
		validAttacks = GetComponent<movementLUT> ().returnValidAttacks (unitTypeA, unitTypeB);
	}

	public void setNewTransform(Transform TargetLocation){
		currentHeight = transform.position.y;
		nextHeight = TargetLocation.transform.position.y;
		currentAngle = transform.rotation.eulerAngles.y;
		nextAngle = TargetLocation.rotation.eulerAngles.y;
		spookyGhostUnit.position = veryFirstPosition;
		spookyGhostUnit.rotation = veryFirstRotation;
		spookyGhostUnit.RotateAround (Vector3.zero, Vector3.up, TargetLocation.rotation.eulerAngles.y);
		lerpCounter = 0f;
		lerpIteration = GetComponentInParent<PlayField> ().returnLerpIteration ();
		spookyGhostUnit.position = new Vector3 (spookyGhostUnit.position.x, nextHeight, spookyGhostUnit.position.z);
	}


	void Update () {
		
		//spookyGhostUnit = spookyGhostObject.transform;
		if (lerpCounter < 1.0f) {
			lerpCounter += lerpIteration;
			transform.position = new Vector3 (transform.position.x, Mathf.Lerp (currentHeight, nextHeight, lerpCounter), transform.position.z);
			transform.RotateAround (Vector3.zero, Vector3.up, (nextAngle-currentAngle) * lerpIteration);
		} else if (currentHeight != nextHeight) {
			transform.position = spookyGhostUnit.position;
			transform.rotation = spookyGhostUnit.rotation;
		}
	}
}
