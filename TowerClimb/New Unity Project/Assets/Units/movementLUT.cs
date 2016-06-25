using UnityEngine;
using System.Collections;

public class movementLUT : MonoBehaviour {
	public int[,] returnValidMovements(string typeA, string typeB){
		int[,] returnTable = new int[11, 11];

		Debug.Log (returnTable [5, 5]);
		if (typeA == "pawn" || typeB == "pawn") {
			returnTable [4, 5] += 1;
			returnTable [5, 4] += 1;
			returnTable [6, 5] += 1;
			returnTable [5, 6] += 1;
		}
		return returnTable;
	}
	public int[,] returnValidAttacks(string typeA, string typeB){
		int[,] returnTable = new int[11, 11];
		Debug.Log (returnTable [9, 0]);
		if (typeA == "pawn") {
			
		}
		return returnTable;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
