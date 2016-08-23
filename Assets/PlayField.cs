using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class PlayField : MonoBehaviour {
	public GameObject tile;
	public GameObject unit;
	public GameObject unitB;
	public GameObject CameraControl;
	private GameObject[,] activeTiles;
	public ArrayList activePlayerUnits;
	public ArrayList activeEnemyUnits;
	public GameObject currentlySelectedUnit;
	private int currentlySelectedNumber = 0;
	private bool currentlySelectingPlayerTeam = true;
	public float lerpIteration = 0.1f;
	public int cylinderCircumference = 20;
	public int activeFieldLow = 0;
	public int activeFieldHigh = 10;
	public float gravity = 0.01f;
	public float unitDissapearLength = 5f;
	public string boardLayout = "6v6";
	// Use this for initialization
	void Start () {

		Vector3[] ring = (Vector3[]) GetComponent<CreateRing> ().createRingArrayList (cylinderCircumference);
		Quaternion[] ringAngles = GetComponent<CreateRing> ().createRingAngleArrayList (cylinderCircumference);

		//generate the playfield
		activeTiles = new GameObject[cylinderCircumference, 10];
		int activeX = cylinderCircumference;
		int activeY = 10;
		for (int x = 0; x < activeX; x++) {
			for (int y = 0; y < activeY; y++) {
				activeTiles [x, y] = (GameObject)Instantiate (tile, ring[x] + new Vector3(0f,(float) y, 0f), ringAngles[x]);
				activeTiles [x, y].name = x.ToString () + "," + y.ToString();
				activeTiles [x, y].transform.SetParent (transform);
				activeTiles[x, y].GetComponent<tileScript>().boardPositionX = x;
				activeTiles[x, y].GetComponent<tileScript>().boardPositionY= y;
			}
		}
		activePlayerUnits = new ArrayList();
		activeEnemyUnits = new ArrayList();

		if (boardLayout == "6v6") {
			//Friendly Units
			activePlayerUnits.Add (newUnit (unit, activeTiles [0, 0], "p 0 0", "Dagger", "nothing"));
			activePlayerUnits.Add (newUnit (unit, activeTiles [0, 1], "p 0 1", "Dagger", "nothing"));
			activePlayerUnits.Add (newUnit (unit, activeTiles [0, 2], "p 0 2", "Flail", "nothing"));
			activePlayerUnits.Add (newUnit (unit, activeTiles [1, 0], "p 1 0", "Flail", "nothing"));
			activePlayerUnits.Add (newUnit (unit, activeTiles [1, 1], "p 1 1", "Sword", "nothing"));
			activePlayerUnits.Add (newUnit (unit, activeTiles [1, 2], "p 1 2", "Spear", "nothing"));


			//Enemy Units
			activeEnemyUnits.Add (newUnit (unitB, activeTiles [0, 9], "e 0 9", "Dagger", "nothing"));
			activeEnemyUnits.Add (newUnit (unitB, activeTiles [0, 8], "e 0 8", "Dagger", "nothing"));
			activeEnemyUnits.Add (newUnit (unitB, activeTiles [0, 7], "e 0 7", "Flail", "nothing"));
			activeEnemyUnits.Add (newUnit (unitB, activeTiles [1, 9], "e 1 9", "Flail", "nothing"));
			activeEnemyUnits.Add (newUnit (unitB, activeTiles [1, 8], "e 1 8", "Sword", "nothing"));
			activeEnemyUnits.Add (newUnit (unitB, activeTiles [1, 7], "e 1 7", "Spear", "nothing"));
		} else if (boardLayout == "debugA") {
			//Friendly Units
			activePlayerUnits.Add (newUnit (unit, activeTiles [3, 3], "p 3 3", "Dagger", "nothing"));
			activePlayerUnits.Add (newUnit (unit, activeTiles [4, 3], "p 4 3", "Dagger", "nothing"));

			//Enemy Units
			activeEnemyUnits.Add (newUnit (unitB, activeTiles [3, 5], "e 3 5", "Dagger", "nothing"));
			activeEnemyUnits.Add (newUnit (unitB, activeTiles [4, 5], "e 4 5", "Dagger", "nothing"));
		} else {

			Debug.Log ("loading board other");

			//Friendly Units
			activePlayerUnits.Add (newUnit (unit, activeTiles [3, 3], "p 3 3", "Dagger", "nothing"));
			activePlayerUnits.Add (newUnit (unit, activeTiles [4, 3], "p 4 3", "Dagger", "nothing"));

			//Enemy Units
			activeEnemyUnits.Add (newUnit (unitB, activeTiles [3, 5], "e 3 5", "Dagger", "nothing"));
			activeEnemyUnits.Add (newUnit (unitB, activeTiles [4, 5], "e 4 5", "Dagger", "nothing"));
		}
		currentlySelectedUnit = fetchUnitGameObject (true, 0);
		CameraControl.GetComponent<cameraScript>().initCamera();
	}

	public float returnLerpIteration(){
		return lerpIteration;
	}


	ArrayList getMovements(GameObject targetUnit){
		ArrayList validMovements = new ArrayList();
		targetUnit.GetComponent<UnitScript> ().returnValidMovements ();
		return validMovements;
	}

	ArrayList paintSelectedMovements(){
		return new ArrayList ();
	}

	GameObject newUnit(GameObject typeOfUnit, GameObject spawnAtTile, string toName, string classA, string classB){
		GameObject freshUnit = (GameObject) Instantiate (typeOfUnit, spawnAtTile.GetComponent<Transform>().position, Quaternion.Euler(0f,spawnAtTile.GetComponent<Transform>().eulerAngles.y,0f));
		freshUnit.transform.SetParent (transform);
		freshUnit.name = toName;
		freshUnit.GetComponent<UnitScript> ().setBoardLocation (spawnAtTile);
		freshUnit.GetComponent<UnitScript> ().setClass ("a", classA);
		freshUnit.GetComponent<UnitScript> ().setClass ("b", classB);

		//freshUnit.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", (Texture)(Resources.Load ("/UnitTextures/Sword.png")));
		return freshUnit;
	}


	GameObject fetchUnitGameObject(bool playerOwned, int number){
		GameObject newUnit;
		if (playerOwned) {
			newUnit = (GameObject)activePlayerUnits [number];
		} else {
			newUnit = (GameObject)activeEnemyUnits [number];
		}
		return newUnit;
	}
	public GameObject getSelectedUnit(){
		return currentlySelectedUnit;
	}
	private void toggleMovementTiles (bool toggleActive){
		ArrayList currentlySelectedMovements = currentlySelectedUnit.GetComponent<UnitScript> ().returnValidMovements ();
		for (int i = 0; i < currentlySelectedMovements.Count; i++){
			Vector2 currentMovement = (Vector2)currentlySelectedMovements [i];
			GameObject currentTile = activeTiles [(int)currentMovement.x, (int)currentMovement.y];
			currentTile.GetComponent<tileScript> ().validMove = toggleActive;
		}
	}
		
	void moveUnit(GameObject nextTileGameObj){
		Transform nextTileTransform = nextTileGameObj.GetComponent<Transform> ();
		currentlySelectedUnit.GetComponent<UnitScript> ().setNewTransform (nextTileTransform);
		currentlySelectedUnit.GetComponent<UnitScript> ().setBoardLocation (nextTileGameObj);
	}

	public void unitDead(Transform deadUnit){
		//YOU WERE DOING SOMETHING HERE AND YOU NEED TO CHANGE ACTIVE PLAYER AND ENEMY UNITS TO A FUCKING ARRAY LIST
		for (int i = 0; i < activePlayerUnits.Count; i++) {
			if (((GameObject)activePlayerUnits [i]).transform == deadUnit) {
				//Debug.Log (deadUnit);
				activePlayerUnits.RemoveAt (i);
			}
		}
		for (int i = 0; i < activeEnemyUnits.Count; i++) {
			if (((GameObject)activeEnemyUnits [i]).transform == deadUnit) {
				//Debug.Log (deadUnit);
				activeEnemyUnits.RemoveAt (i);
			}
		}
	}

	public Hashtable GetBoardInformation(){
		Hashtable boardInfo = new Hashtable ();
		boardInfo.Add ("playerUnits", activePlayerUnits);
		boardInfo.Add ("enemyUnits", activeEnemyUnits);	
		boardInfo.Add ("cylinderCircumference", cylinderCircumference);
		boardInfo.Add ("activeFieldHigh", activeFieldHigh);
		boardInfo.Add ("activeFieldLow", activeFieldLow);
		//make a hash for occupied spaces
		Hashtable occupiedSpaces = new Hashtable ();
		for (int i = 0; i < activePlayerUnits.Count; i++){
			int occupiedX = ((GameObject) activePlayerUnits[i]).GetComponent<UnitScript> ().boardLocationX;
			int occupiedY = ((GameObject) activePlayerUnits[i]).GetComponent<UnitScript> ().boardLocationY;
			occupiedSpaces.Add (occupiedX.ToString () + "_" + occupiedY.ToString (), (GameObject)activePlayerUnits [i]);
		}
		for (int i = 0; i < activeEnemyUnits.Count; i++) {
			int occupiedX = ((GameObject) activeEnemyUnits[i]).GetComponent<UnitScript> ().boardLocationX;
			int occupiedY = ((GameObject) activeEnemyUnits[i]).GetComponent<UnitScript> ().boardLocationY;
			occupiedSpaces.Add (occupiedX.ToString () + "_" + occupiedY.ToString (), (GameObject)activeEnemyUnits [i]);
		}
		boardInfo.Add ("occupiedSpaces", occupiedSpaces);
		return boardInfo;
	}

	private void printHashTable(Hashtable inputHash){
		foreach(string key in inputHash.Keys){
			Debug.Log (key + " " + inputHash [key]);
		}
	}

	private void takeEnemyTurn(){
		Hashtable moveDecision = (Hashtable) GetComponent<EnemyAiV2> ().decideMove (3);
		//GameObject currentTile = activeTiles [(int)currentMovement.x, (int)currentMovement.y];
		printHashTable(moveDecision);
		makeMove ((string) moveDecision["move"]);
	}

	private void makeMove (string movementIn){
		string[] splitArray = Regex.Split(movementIn, "-");
		Debug.Log ("1");
		Debug.Log (splitArray [0]);
		Debug.Log (splitArray [1]);
		string[] unitLocStringArray = Regex.Split (splitArray [0], "_");
		string[] movementLocStringArray = Regex.Split (splitArray [1], "_");

		//TODO: DELETE THIS WHEN YOU FIX ENEMYAI ~
		int indexA = movementLocStringArray [1].IndexOf('~');
		movementLocStringArray [1] = movementLocStringArray [1].Remove (indexA);


		//Debug.Log ("LOGGING SPLIT STRINGS!");
		//Debug.Log (unitLocStringArray [0]);
		//Debug.Log (unitLocStringArray [1]);
		//Debug.Log (movementLocStringArray [0]);
		//Debug.Log (movementLocStringArray [1]);

		int unitLocX = int.Parse(unitLocStringArray [0]);
		int unitLocY = int.Parse(unitLocStringArray [1]);
		int movementLocX = int.Parse(movementLocStringArray [0]);
		int movementLocY = int.Parse(movementLocStringArray [1]);

		//TODO: make this more effecient
		for (int i = 0; i < activeEnemyUnits.Count; i++) {
			GameObject unitInspect = (GameObject)activeEnemyUnits [i];
			int occupiedX = unitInspect.GetComponent<UnitScript> ().boardLocationX;
			int occupiedY = unitInspect.GetComponent<UnitScript> ().boardLocationY;
			if (occupiedX == unitLocX && occupiedY == unitLocY) {
				currentlySelectedUnit = (GameObject) activeEnemyUnits [i];
			}
		}

		GameObject tileGameObject = (GameObject)activeTiles [movementLocX, movementLocY];
		Debug.Log ("currently Selected unit" + currentlySelectedUnit.name);
		Debug.Log ("move to tile" + tileGameObject.name);

		GameObject objectOnTile = tileGameObject.GetComponent<tileScript> ().objectOnTile;
		if (objectOnTile != null) {
			if (objectOnTile.GetComponent<UnitScript> ().playerOwned != currentlySelectedUnit.GetComponent<UnitScript> ().playerOwned) {
				toggleMovementTiles (false);
				moveUnit (tileGameObject);
				toggleMovementTiles (true);
				objectOnTile.GetComponent<UnitScript> ().killUnit ();
			}
		} else {
			toggleMovementTiles (false);
			moveUnit (tileGameObject);
			toggleMovementTiles (true);
		}
 
	}

	//private void moveUnitToTile(unitCords, moveCords) {
		
	//}

	/// 
	/// Update
	/// 

	private bool firstUpdate = true;

	private bool frameFreeze = false;
	void Update () {
		if (Time.deltaTime > 0.1f) {

			Debug.Log ("especially long frame: " + Time.deltaTime);
		}
		if (firstUpdate) {
			ArrayList currentlySelectedMovements = currentlySelectedUnit.GetComponent<UnitScript> ().returnValidMovements ();
			toggleMovementTiles (true);
			firstUpdate = false;
		}



		if (Input.GetKey ("s")) {
			if (!frameFreeze) {
				frameFreeze = true;
				currentlySelectedNumber -= 1;
				if (currentlySelectedNumber <= -1) {
					if (currentlySelectingPlayerTeam) {
						currentlySelectedNumber = activePlayerUnits.Count - 1;
					} else {
						currentlySelectedNumber = activeEnemyUnits.Count - 1;
					}
				}
				toggleMovementTiles (false);
				currentlySelectedUnit = fetchUnitGameObject (currentlySelectingPlayerTeam, currentlySelectedNumber);
				toggleMovementTiles (true);
			}
		} else if (Input.GetKey ("w")) {
			if (!frameFreeze) {
				frameFreeze = true;
				currentlySelectedNumber += 1;
				if (currentlySelectingPlayerTeam) {
					if (currentlySelectedNumber >= activePlayerUnits.Count) {
						currentlySelectedNumber = 0;
					}
				} else {
					if (currentlySelectedNumber >= activeEnemyUnits.Count) {
						currentlySelectedNumber = 0;
					}
				}
				toggleMovementTiles (false);
				currentlySelectedUnit = fetchUnitGameObject (currentlySelectingPlayerTeam, currentlySelectedNumber);
				toggleMovementTiles (true);
			}
		} else if (Input.GetKey ("d") || Input.GetKey ("a")) {
			if (!frameFreeze) {
				frameFreeze = true;
				currentlySelectingPlayerTeam = !currentlySelectingPlayerTeam;
				if (currentlySelectingPlayerTeam) {
					if (activePlayerUnits.Count > 0) {
						while (currentlySelectedNumber >= activePlayerUnits.Count) {
							currentlySelectedNumber -= 1;
						}
					} else {
						currentlySelectingPlayerTeam = !currentlySelectingPlayerTeam;
					}

				} else {
					if (activeEnemyUnits.Count > 0) {
						while (currentlySelectedNumber >= activeEnemyUnits.Count) {
							currentlySelectedNumber -= 1;
						}
					} else {
						currentlySelectingPlayerTeam = !currentlySelectingPlayerTeam;
					}
				}
				toggleMovementTiles (false);
				currentlySelectedUnit = fetchUnitGameObject (currentlySelectingPlayerTeam, currentlySelectedNumber);
				toggleMovementTiles (true);
			}
		} else if (Input.GetKey ("q")) {
			GameObject nextTileGameObj = activeTiles [4, 4];
			Transform nextTileTransform = nextTileGameObj.GetComponent<Transform> ();
			currentlySelectedUnit.GetComponent<UnitScript> ().setNewTransform (nextTileTransform);
		} else if (Input.GetKey ("e")) {
			if (!frameFreeze) {
				ArrayList churrentlySelectedMovements = currentlySelectedUnit.GetComponent<UnitScript> ().returnValidMovements ();
				toggleMovementTiles (true);
				frameFreeze = true;
			}

		} else if (Input.GetKey ("r")) {
			if (!frameFreeze) {
				frameFreeze = true;
				//Hashtable decidedMove = GetComponent<EnemyAiV2> ().test (false);
				//Debug.Log (decidedMove["move"]);
				//Debug.Log (decidedMove ["score"]);
				takeEnemyTurn();
			}
		} else if (Input.GetMouseButtonDown (0)) {
			if (Input.GetMouseButtonDown(0))
			{
				//Debug.Log("Mouse is down");

				RaycastHit hitInfo = new RaycastHit();
				bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
				if (hit) 
				{
					GameObject hitGameObject = hitInfo.transform.gameObject;
					if (hitGameObject.tag == "Tile") {
						if (hitGameObject.GetComponent<tileScript> ().validMove) {
							GameObject objectOnTile = hitGameObject.GetComponent<tileScript> ().objectOnTile;
							if (objectOnTile != null) {
								if (objectOnTile.GetComponent<UnitScript> ().playerOwned != currentlySelectedUnit.GetComponent<UnitScript> ().playerOwned) {
									toggleMovementTiles (false);
									moveUnit (hitInfo.transform.gameObject);
									toggleMovementTiles (true);
									objectOnTile.GetComponent<UnitScript> ().killUnit ();
								}
							} else {
								toggleMovementTiles (false);
								moveUnit (hitInfo.transform.gameObject);
								toggleMovementTiles (true);
							}
						} 
					}
					if (hitGameObject.tag == "EnemyUnit") {
						if (hitGameObject.GetComponent<UnitScript> ().tileOn.GetComponent<tileScript>().validMove) {
							if (currentlySelectedUnit.GetComponent<UnitScript> ().playerOwned) {
								toggleMovementTiles (false);
								moveUnit (hitGameObject.GetComponent<UnitScript> ().tileOn);
								hitGameObject.GetComponent<UnitScript> ().killUnit ();
								toggleMovementTiles (true);
							}
						};
					}
					if (hitGameObject.tag == "PlayerUnit") {
						if (hitGameObject.GetComponent<UnitScript> ().tileOn.GetComponent<tileScript>().validMove) {
							if (!currentlySelectedUnit.GetComponent<UnitScript> ().playerOwned) {
								toggleMovementTiles (false);
								moveUnit (hitGameObject.GetComponent<UnitScript> ().tileOn);
								hitGameObject.GetComponent<UnitScript> ().killUnit ();
								toggleMovementTiles (true);
							}
						};
					}
				} else {
					//Debug.Log("No hit");
				}
				//Debug.Log("Mouse is down");
			} 
		}else if(frameFreeze) {
			frameFreeze = false;
		}

	}
}
