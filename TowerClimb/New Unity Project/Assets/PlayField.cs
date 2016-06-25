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

	// Use this for initialization
	void Start () {
		//generate the playfield
		activeTiles = new GameObject[10, 10];
		int activeX = 10;
		int activeY = 10;
		for (int x = 0; x < activeX; x++) {
			for (int y = 0; y < activeY; y++) {
				activeTiles [x, y] = (GameObject)Instantiate (tile, new Vector3 (x, 0f, y), Quaternion.Euler (0f, 0f, 0f));
				activeTiles [x, y].transform.SetParent (transform);
			}
		}
		activePlayerUnits = new GameObject[6];
		activeEnemyUnits = new GameObject[6];
		activePlayerUnits [0] = (newUnit (unit, 0, 1));
		activeEnemyUnits [0] = (newUnit (unit, 0, 8));

		activePlayerUnits [1] = (newUnit (unit, 0, 2));
		activeEnemyUnits [1] = (newUnit (unit, 0, 9));

		activePlayerUnits [2] = (newUnit (unit, 1, 1));
		activeEnemyUnits [2] = (newUnit (unit, 1, 8));

		activePlayerUnits [3] = (newUnit (unit, 1, 2));
		activeEnemyUnits [3] = (newUnit (unit, 2, 8));

		activePlayerUnits [4] = (newUnit (unit, 2, 2));
		activeEnemyUnits [4] = (newUnit (unit, 2, 9));

		activePlayerUnits [5] = (newUnit (unit, 2, 1));
		activeEnemyUnits [5] = (newUnit (unit, 3, 8));
		/*
		for (int i = 0; i < 4; i++){
			activePlayerUnits[i] = (GameObject) Instantiate (unit, new Vector3 (i, 0f, 0f), Quaternion.Euler(0f,0f,0f));
			activeEnemyUnits[i] = (GameObject) Instantiate (unit, new Vector3 (i, 0f, 20), Quaternion.Euler(0f,0f,0f));
		}
		*/
		currentlySelectedUnit = fetchUnitGameObject (true, 0);
		Debug.Log (currentlySelectedUnit);
		CameraControl.GetComponent<cameraScript>().initCamera();
	}

	GameObject newUnit(GameObject typeOfUnit, int xPosition, int yPosition){
		GameObject freshUnit = (GameObject) Instantiate (typeOfUnit, new Vector3 (xPosition, 0, yPosition), Quaternion.Euler(0f,0f,0f));
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
