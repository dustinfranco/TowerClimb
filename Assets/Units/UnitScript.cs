using UnityEngine;
using System.Collections;

public class UnitScript : MonoBehaviour {
	public string typeA = "pawn";
	public string typeB = "nothing";
	public ArrayList validMovements = new ArrayList();
	public ArrayList validAttacks = new ArrayList();
	private float currentHeight;
	private float nextHeight;
	private float currentAngle;
	private float nextAngle;
	private Vector3 veryFirstPosition;
	private Quaternion veryFirstRotation;
	public GameObject spookyGhostObjectUnreal;
	private Transform spookyGhostUnit;
	public int boardLocationX = 0;
	public int boardLocationY = 0;
	private int cylinderCircumference;
	private float activeFieldLow = 0;
	private float activeFieldHigh = 999;
	public GameObject tileOn;
	bool unitKilled = false;
	private Vector3 unitKilledPosition = Vector3.zero;
	private float gravity;
	public bool playerOwned = true;
	private float unitKilledLerpTime = 0f;
	private float lerpCounter = 1.0f;
	// Update is called once per frame
	private float lerpIteration = 0.01f;
	//private float CurrentRotation = 0.0f;
	//private float NextRotation = 0.0f;
	// Use this for initialization
	public Material[] unitMaterials;
	void Start () {
		//tileOn = new GameObject ();
		GameObject spookyGhostObjectReal = (GameObject)Instantiate (spookyGhostObjectUnreal, Vector3.zero, Quaternion.identity);
		spookyGhostUnit = spookyGhostObjectReal.transform;
		gravity = GetComponentInParent<PlayField> ().gravity;
		cylinderCircumference = GetComponentInParent<PlayField>().cylinderCircumference;
		activeFieldLow = GetComponentInParent<PlayField>().activeFieldLow;
		activeFieldHigh = GetComponentInParent<PlayField>().activeFieldHigh;
		float veryFirstX = (float) cylinderCircumference / (2 * Mathf.PI);
		veryFirstPosition = new Vector3 (veryFirstX, 0f, 0f); 
		veryFirstRotation = Quaternion.identity;
		 
	}

	private void updateActiveField(){

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

	public void setBoardLocation (GameObject tileAt){
		GameObject lastTile = tileOn;
		tileOn = tileAt;
		tileAt.GetComponent<tileScript> ().setObjectOnTile (gameObject);
		if (lastTile != null) {
			lastTile.GetComponent<tileScript> ().setObjectOnTile (null);
		}
		boardLocationX = tileOn.GetComponent<tileScript>().boardPositionX;
		boardLocationY = tileOn.GetComponent<tileScript>().boardPositionY;
	}

	public void killUnit(){
		unitKilled = true;
		unitKilledPosition = transform.position;
	}

	public string getClass(string whichClass){
		if (whichClass == "a") {
			return typeA;
		}	
		return typeB;
		
	}

	public void setClass(string replaceClass, string classReplacement){
		if (replaceClass == "a") {
			typeA = classReplacement;
			GetComponent<movementLUT> ().changeClass (typeA, typeB);
			return;
		}
		typeB = classReplacement;
		GetComponent<movementLUT> ().changeClass (typeA, typeB);
	}

	public ArrayList returnValidMovements(){
		ArrayList allMovements = GetComponent<movementLUT>().returnAllMovements ();
		ArrayList validMovements = new ArrayList ();
		activeFieldLow = GetComponentInParent<PlayField>().activeFieldLow;
		activeFieldHigh = GetComponentInParent<PlayField>().activeFieldHigh;
		for (int i = 0; i < allMovements.Count; i++) {
			Vector2 currentMovement = (Vector2)allMovements [i];
			int offsetX = (int)currentMovement.x;
			int offsetY = (int)currentMovement.y;
			//valid X movements:
			if (boardLocationX + offsetX < 0) {
				currentMovement.x = boardLocationX + offsetX + cylinderCircumference;
			} else if (boardLocationX + offsetX >= cylinderCircumference) {
				currentMovement.x = boardLocationX + offsetX - cylinderCircumference;
			} else {
				currentMovement.x = boardLocationX + offsetX;
			}
			//valid Y Movements
			currentMovement.y = boardLocationY + offsetY;
			if ((currentMovement.y > activeFieldLow) && (currentMovement.y < activeFieldHigh)) {
				validMovements.Add (currentMovement);
			}

		}
		return validMovements;

	}

	void Update () {
		Texture2D whatever = GetComponent<movementLUT>().findClassTexture("Spear","nothing", false);
		GetComponent<Renderer> ().material.mainTexture = GetComponent<movementLUT>().findClassTexture(typeA, typeB, playerOwned);
		//spookyGhostUnit = spookyGhostObject.transform;
		if (unitKilled) {
			unitKilledLerpTime += 1f;
			transform.position = transform.position - new Vector3 (-unitKilledPosition.x * 0.01f, gravity * unitKilledLerpTime, -unitKilledPosition.z * 0.01f);
			if (unitKilledLerpTime > 50f) {
				Destroy (spookyGhostUnit.gameObject);
				Destroy (gameObject);
				GetComponentInParent<PlayField> ().unitDead (transform);
			}
		} else if (lerpCounter < 1.0f) {
			lerpCounter += lerpIteration;
			transform.position = new Vector3 (transform.position.x, Mathf.Lerp (currentHeight, nextHeight, lerpCounter), transform.position.z);
			transform.RotateAround (Vector3.zero, Vector3.up, (nextAngle - currentAngle) * lerpIteration);
		} else if (currentHeight != nextHeight) {
			transform.position = spookyGhostUnit.position;
			transform.rotation = spookyGhostUnit.rotation;
		}
	}
}
