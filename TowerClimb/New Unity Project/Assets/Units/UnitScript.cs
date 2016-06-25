using UnityEngine;
using System.Collections;

public class UnitScript : MonoBehaviour {
	public string typeA = "pawn";
	public string typeB = "nothing";
	public int[,] validMovements = new int[10,10];
	public int[,] validAttacks = new int[10,10];
	private Vector3 CurrentLoc = Vector3.zero;
	private Vector3 NextLoc = Vector3.zero;
	//private float CurrentRotation = 0.0f;
	//private float NextRotation = 0.0f;
	// Use this for initialization
	void Start () {
		updateValidTables (typeA, typeB);
		CurrentLoc = transform.position;
		NextLoc = transform.position;
	}

	void updateValidTables(string unitTypeA, string unitTypeB){
		//populate the movement table
		validMovements = GetComponent<movementLUT> ().returnValidMovements (unitTypeA, unitTypeB);
		validAttacks = GetComponent<movementLUT> ().returnValidAttacks (unitTypeA, unitTypeB);
	}

	public void setNewTransform(Transform TargetLocation){
		CurrentLoc = transform.position;
		NextLoc = TargetLocation.transform.position;
	}


	private float lerpCounter = 1.0f;
	// Update is called once per frame
	void Update () {
		if (lerpCounter < 1.0f) {
			lerpCounter += 0.1f;
			transform.position = Vector3.Lerp (CurrentLoc, NextLoc, lerpCounter);
		} else if (transform.position != NextLoc) {
			lerpCounter = 0.0f;
		} else if (CurrentLoc != NextLoc) {
			CurrentLoc = NextLoc;
		}
	}
}
