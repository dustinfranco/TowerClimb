using UnityEngine;
using System.Collections;

public class PlayField : MonoBehaviour {
	public GameObject tile;
	public GameObject unit;
	public GameObject CameraControl;
	private GameObject[,] activeTiles;
	private GameObject[] activePlayerUnits;
	private GameObject[] activeEnemyUnits;
	public GameObject currentlySelectedUnit;
	private int currentlySelectedNumber = 0;
	private bool currentlySelectingPlayerTeam = false;
	public float lerpIteration = 0.1f;

	// Use this for initialization
	void Start () {

		Vector3[] ring = (Vector3[]) GetComponent<CreateRing> ().createRingArrayList (10);
		Quaternion[] ringAngles = GetComponent<CreateRing> ().createRingAngleArrayList (10);

		//generate the playfield
		activeTiles = new GameObject[10, 10];
		int activeX = 10;
		int activeY = 10;
		for (int x = 0; x < activeX; x++) {
			for (int y = 0; y < activeY; y++) {
				activeTiles [x, y] = (GameObject)Instantiate (tile, ring[x] + new Vector3(0f,(float) y, 0f), ringAngles[x]);
				activeTiles [x, y].name = x.ToString () + "," + y.ToString();
				activeTiles [x, y].transform.SetParent (transform);
			}
		}
		activePlayerUnits = new GameObject[6];
		activeEnemyUnits = new GameObject[6];
		activePlayerUnits [0] = newUnit (unit, activeTiles[0,0]);
		activePlayerUnits [0].name = "0,0";
		activePlayerUnits [1] = newUnit (unit, activeTiles[6,1]);
		activePlayerUnits [1].name = "6,1";
		activePlayerUnits [2] = newUnit (unit, activeTiles[3,1]);
		activePlayerUnits [2].name = "3,1";
		activeEnemyUnits [0] = newUnit (unit, activeTiles[5,1]);
		activeEnemyUnits [0].name = "5,1";
		/*
		for (int i = 0; i < 4; i++){
			activePlayerUnits[i] = (GameObject) Instantiate (unit, new Vector3 (i, 0f, 0f), Quaternion.Euler(0f,0f,0f));
			activeEnemyUnits[i] = (GameObject) Instantiate (unit, new Vector3 (i, 0f, 20), Quaternion.Euler(0f,0f,0f));
		}
		*/
		currentlySelectedUnit = fetchUnitGameObject (true, 0);
		CameraControl.GetComponent<cameraScript>().initCamera();
	}

	public float returnLerpIteration(){
		return lerpIteration;
	}

	GameObject newUnit(GameObject typeOfUnit, GameObject spawnAtTile){
		GameObject freshUnit = (GameObject) Instantiate (typeOfUnit, spawnAtTile.GetComponent<Transform>().position, Quaternion.Euler(0f,spawnAtTile.GetComponent<Transform>().eulerAngles.y,0f));
		freshUnit.transform.SetParent (transform);
		return freshUnit;
	}

	GameObject fetchUnitGameObject(bool playerOwned, int number){
		GameObject newUnit;
		if (playerOwned) {
			newUnit = activePlayerUnits [number];
		} else {
			newUnit = activeEnemyUnits [number];
		}
		return newUnit;
	}
	public GameObject getSelectedUnit(){
		return currentlySelectedUnit;
	}
	// Update is called once per frame
	private bool frameFreeze = false;
	void Update () {
		if (Input.GetKey ("s")) {
			if (!frameFreeze) {
				frameFreeze = true;
				currentlySelectedNumber += 1;
				if (currentlySelectedNumber == activePlayerUnits.Length) {
					currentlySelectedNumber = 0;
				}
				currentlySelectedUnit = fetchUnitGameObject (currentlySelectingPlayerTeam, currentlySelectedNumber);
			}
		} else if (Input.GetKey ("w")) {
			if (!frameFreeze) {
				frameFreeze = true;
				currentlySelectedNumber += 1;
				if (currentlySelectedNumber == activePlayerUnits.Length) {
					currentlySelectedNumber = 0;
				}
				currentlySelectedUnit = fetchUnitGameObject (currentlySelectingPlayerTeam, currentlySelectedNumber);
			}
		} else if (Input.GetKey ("d") || Input.GetKey ("a")) {
			if (!frameFreeze) {
				frameFreeze = true;
				currentlySelectingPlayerTeam = !currentlySelectingPlayerTeam;
				currentlySelectedUnit = fetchUnitGameObject (currentlySelectingPlayerTeam, currentlySelectedNumber);
			}
		} else if (Input.GetKey ("q")) {
			GameObject nextTileGameObj = activeTiles [4, 4];
			Transform nextTileTransform = nextTileGameObj.GetComponent<Transform> ();
			currentlySelectedUnit.GetComponent<UnitScript> ().setNewTransform (nextTileTransform);
		} else if(frameFreeze) {
			frameFreeze = false;
		}

	}
}
