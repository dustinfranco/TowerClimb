using UnityEngine;
using System.Collections;

public class EnemyAi : MonoBehaviour {
	private int closerScore = 1;
	private int nextScore = 5;
	private int dieScore = -20;

	private float[] plyFactorArray = new float[6];
	private int plyMax = 3;

	public int takeScore = 30;
	public int defendScore = 20;

	public int undefendedScore = -40;
	public int defendedScore = 30;

	ArrayList playerUnits = new ArrayList ();
	ArrayList enemyUnits = new ArrayList ();
	void Start() {
		for (int x = 0; x < plyFactorArray.Length; x++) {
			plyFactorArray [x] = 1.0f / (float) x;
		}	
	}

	private ArrayList getPlayerUnits(){
		return new ArrayList ();
	}
		

	public void decideMove(){

	}

	private Hashtable surveyField (){
		Hashtable returnHash = GetComponent<PlayField> ().GetBoardInformation ();
		if (false) {
			Debug.Log (returnHash["playerUnits"]);
			Debug.Log (returnHash ["enemyUnits"]);
			Debug.Log (returnHash ["cylinderCircumference"]);
			Debug.Log (returnHash ["activeFieldHigh"]);
			Debug.Log (returnHash ["activeFieldLow"]);
			Debug.Log (returnHash ["occupiedSpaces"]);
			Hashtable occupado = (Hashtable) returnHash ["occupiedSpaces"];
			foreach (DictionaryEntry unit in occupado) {
				Debug.Log ((string) unit.Key + ((GameObject)unit.Value).name);
			}
		}
		return returnHash;
	}

	public void test(){
		Vector2 scores = getBoardScore (surveyField ());
		Debug.Log ("BOARD SCORE PLAYER " + scores.x);
		Debug.Log ("BOARD SCORE ENEMY " + scores.y);
		chooseMove ();
	}
	private ArrayList getMovementScores(ArrayList playerUnits, ArrayList enemyUnits, Hashtable occupiedSpaces, ArrayList moves){
		for (int i = 0; i < playerUnits.Count; i++) {
			GameObject currentObject = (GameObject)playerUnits [i];
			getUnitScore (currentObject, occupiedSpaces, enemyUnits, playerUnits);
		}
		for (int i = 0; i < enemyUnits.Count; i++) {
			GameObject currentObject = (GameObject)enemyUnits [i];
			getUnitScore (currentObject, occupiedSpaces, enemyUnits, playerUnits);
		}
		return new ArrayList ();
	}


	//calculate the board score
	private Vector2 getBoardScore(Hashtable fieldInformation){
		ArrayList playerUnits = (ArrayList) fieldInformation ["playerUnits"];
		ArrayList enemyUnits = (ArrayList) fieldInformation ["enemyUnits"];
		Hashtable occupiedSpaces = (Hashtable) fieldInformation ["occupiedSpaces"];
		int boardScoreEnemy = 0;
		int boardScorePlayer = 0;
		for (int i = 0; i < playerUnits.Count; i++) {
			GameObject currentObject = (GameObject)playerUnits [i];
			boardScorePlayer += getUnitScore (currentObject, occupiedSpaces, enemyUnits, playerUnits);
		}
		for (int i = 0; i < enemyUnits.Count; i++) {
			GameObject currentObject = (GameObject)enemyUnits [i];
			boardScoreEnemy += getUnitScore (currentObject, occupiedSpaces, enemyUnits, playerUnits);
		}
		return new Vector2 (boardScorePlayer, boardScoreEnemy);
	}


	//calculate individual unit score
	//pass it moves
	public int getUnitScore(GameObject targetUnit, Hashtable occupiedSpaces, ArrayList enemyUnits, ArrayList playerUnits){
		int unitScore = 0;
		/*
		 * MOVES UNIT CAN MAKE
		 * if it can kill, that's good
		 * if it can defend that's good
		 * DEFENSIBILITY OF SQUARE
		 * if it's defended, that's good
		 * if it can be attacked, that's bad
		*/

		//MOVES UNIT CAN MAKE
		Hashtable targetUnitMoves = getMovesSingleUnit (targetUnit);
		foreach (DictionaryEntry move in targetUnitMoves) {
			Vector2 currentMove = (Vector2)move.Value;
			string currentMoveString = currentMove.x.ToString () + "_" + currentMove.y.ToString ();
			if(occupiedSpaces.ContainsKey(currentMoveString)){
				GameObject unitInSpace = (GameObject) occupiedSpaces[currentMoveString]; 
				if(unitInSpace.GetComponent<UnitScript>().playerOwned == targetUnit.GetComponent<UnitScript>().playerOwned){
					//defending other unit score
					unitScore += defendScore;
				} else {
					//can take other unit score
					unitScore += takeScore;
				}
			}
		}

		//DEFENSIBILITY OF SQUARE
		Vector2 unitPosition = new Vector2(targetUnit.GetComponent<UnitScript>().boardLocationX, targetUnit.GetComponent<UnitScript>().boardLocationY);
		for (int i = 0; i < playerUnits.Count; i++){
			GameObject playerUnit = (GameObject) playerUnits [i];
			Hashtable playerUnitMoves =  getMovesSingleUnit(playerUnit);
			if(playerUnitMoves.ContainsValue(unitPosition)){
				if (targetUnit.GetComponent<UnitScript> ().playerOwned) {
					//if target unit is player owned, the unit move is a defensive one
					unitScore += defendedScore;
				} else {
					//if target unit is enemy owned, the unit move is an offensive one
					unitScore += undefendedScore;
				}
			}
		}
			
		for (int i = 0; i < enemyUnits.Count; i++){
			GameObject enemyUnit = (GameObject) enemyUnits [i];
			Hashtable enemyUnitMoves =  getMovesSingleUnit(enemyUnit);
			if(enemyUnitMoves.ContainsValue(unitPosition)){
				if (targetUnit.GetComponent<UnitScript> ().playerOwned) {
					//if target unit is player owned, the unit move is an offensive one
					unitScore += undefendedScore;
				} else {
					//if target unit is enemy owned, the unit move is a defensive one
					unitScore += defendedScore;
				}
			}
		}
		//Debug.Log (targetUnit.name + " SCORE: " +  unitScore.ToString ());
		return unitScore;
	}
	/*
	private ArrayList getAllMovesAllUnits(){
		
		return new ArrayList ();
	}
	*/
	private Hashtable getMovesSingleUnit(GameObject targetUnit) {
		ArrayList validMovements = targetUnit.GetComponent<UnitScript> ().returnValidMovements ();
		Hashtable hashMovements = new Hashtable ();
		string unitName = targetUnit.name;
		for (int j = 0; j < validMovements.Count; j++){
			hashMovements.Add (unitName + "_" + j.ToString (), (Vector2)validMovements [j]);
		}
		return hashMovements;
	}

	private ArrayList findMoveTypes(ArrayList moves){
		//findd same team - not available
		//find enemy team - mark as taking
		//find random movements
		return new ArrayList();
	}

	private Hashtable applyHypotheticalMove(Hashtable inputField, Hashtable moveInput, bool playerTurn){
		Hashtable tempField = inputField;
		ArrayList tempPlayerUnits = (ArrayList) tempField ["playerUnits"];
		ArrayList tempEnemyUnits = (ArrayList) tempField ["enemyUnits"];
		Hashtable tempOccupiedSpaces = (Hashtable) tempField ["occupiedSpaces"];
		string currentlyMovingUnit = (string) moveInput ["name"];
		Vector2 currentMove = (Vector2)moveInput ["movement"];
		string currentMoveString = currentMove.x.ToString () + "_" + currentMove.y.ToString ();
		if (playerTurn) {
			return new Hashtable ();
		} else {
			//if you're taking a unit
			if (moveInput.ContainsKey ("taking")) {
				for (int i = 0; i < tempPlayerUnits.Count; i++) {
					string unitName = ((GameObject)tempPlayerUnits [i]).name;
					if ((string)moveInput ["taking"] == unitName) {
						tempOccupiedSpaces.Remove (currentMoveString);
						tempPlayerUnits.Remove (i);
					}
				}
			} else {
				for(int i = 0; i < tempEnemyUnits.Count; i++){
					string unitName = ((GameObject)tempEnemyUnits[i]).name;
					if ((string) moveInput ["unit"] == unitName) {
						GameObject tempUnit = (GameObject)tempEnemyUnits [i];
						string currentSpaceString = tempUnit.GetComponent<UnitScript> ().boardLocationX.ToString () + "_" + tempUnit.GetComponent<UnitScript> ().boardLocationX.ToString ();
						tempOccupiedSpaces.Remove(currentSpaceString);
						tempOccupiedSpaces [currentMoveString] = tempUnit;
					}
				}
			}
		}
		return tempField;
	}

	private ArrayList findValidMoves(Hashtable inputField, bool playerTurn){
		ArrayList allValidMoves = new ArrayList ();
		if (playerTurn) {
			return new ArrayList ();
		} else {
			ArrayList playerUnits = (ArrayList) inputField ["playerUnits"];
			ArrayList enemyUnits = (ArrayList) inputField ["enemyUnits"];
			Hashtable occupiedSpaces = (Hashtable) inputField ["occupiedSpaces"];
			for (int i = 0; i < enemyUnits.Count; i++) {
				GameObject targetUnit = (GameObject)enemyUnits [i];
				Hashtable targetUnitMoves = getMovesSingleUnit (targetUnit);
				foreach (DictionaryEntry move in targetUnitMoves) {
					bool isValid = true;
					Hashtable currentMove = new Hashtable ();
					currentMove ["unit"] = targetUnit.name;
					currentMove ["movement"] = (Vector2)move.Value;
					currentMove ["score"] = new Vector2 (0, 0);
					//CHANGE OCCUPIEDSPACES TO WORK WITH MOVES
					Vector2 tempVect = (Vector2)move.Value;
					string currentMoveString = tempVect.x.ToString () + "_" + tempVect.y.ToString ();
					if (occupiedSpaces.ContainsKey (currentMoveString)) {
						GameObject unitInSpace = (GameObject)occupiedSpaces [currentMoveString]; 
						if (unitInSpace.GetComponent<UnitScript> ().playerOwned == targetUnit.GetComponent<UnitScript> ().playerOwned) {
							//defending other unit score
							isValid = false;
						} else { //is valid
							isValid = true;
							currentMove ["unit"] = targetUnit.name;
							allValidMoves.Add (currentMove);
						}
					}
					if (isValid) {
						allValidMoves.Add (currentMove);
					}
				}
			}
			return allValidMoves;
		}
	}

	private ArrayList addScoresToMoves(Hashtable inputField, ArrayList inputMoves){
		Hashtable tempField = new Hashtable (inputField);
		ArrayList tempMoves = inputMoves;
		ArrayList playerUnits = new ArrayList((ArrayList) inputField ["playerUnits"]);
		ArrayList enemyUnits = new ArrayList((ArrayList) inputField ["enemyUnits"]);
		Hashtable occupiedSpaces = new Hashtable((Hashtable) inputField ["occupiedSpaces"]);
		//foreach (DictionaryEntry move in inputMoves) {
		for(int i = 0; i < tempMoves.Count; i++){
			Hashtable currentMove = (Hashtable) tempMoves [i];
			Hashtable temporaryField = applyHypotheticalMove ((Hashtable)inputField, currentMove, false);
			Vector2 boardscore = getBoardScore (temporaryField);
			((Hashtable) tempMoves[i]) ["boardScore"] = boardscore;
			//Debug.Log (((Hashtable) tempMoves [i]) ["boardScore"]);
		}
		return tempMoves;
	}

	//return an arrayList of moves
	private ArrayList ply(Hashtable inputField, bool playerTurn){
		ArrayList playerUnits = (ArrayList) inputField ["playerUnits"];
		ArrayList enemyUnits = (ArrayList) inputField ["enemyUnits"];
		Hashtable occupiedSpaces = (Hashtable) inputField ["occupiedSpaces"];
		ArrayList returnMoves = new ArrayList ();

		if (playerTurn) {
			return new ArrayList ();
		} else {
			//FINDVALIDMOVES (for enemy)
			returnMoves = findValidMoves(inputField, false);
			returnMoves = addScoresToMoves (inputField, returnMoves);
		}
		return returnMoves;
	}

	public ArrayList chooseMove(){
		Hashtable initialField = surveyField ();
		Vector2 initialScores = getBoardScore (initialField);
		ArrayList InitialScoredMoves = ply (initialField, false);
		Hashtable bestMove = new Hashtable ();
		float highScore = 0;
		bestMove = (Hashtable) InitialScoredMoves [0];
		Vector2 initialBestMove = (Vector2) bestMove ["boardScore"];
		highScore = initialBestMove.y - initialBestMove.x;
		for (int i = 1; i < InitialScoredMoves.Count; i++) {
			Hashtable considerMove = (Hashtable)InitialScoredMoves [i];
			Vector2 scores = (Vector2)((Hashtable)InitialScoredMoves [i]) ["boardScore"];
			float tempScore = scores.y - scores.x;
			if (tempScore > highScore) {
				//Debug.Log ("new best score " + tempScore);
				highScore = tempScore;
				bestMove = considerMove;
			}
		}
		Debug.Log ("BEST MOVE CHOSEN");
		Debug.Log (bestMove["unit"]);
		Debug.Log (bestMove["movement"]);
		Debug.Log (bestMove["boardScore"]);

		//WHAT IS A MOVE?
		//A MOVE IS A HASHTABLE:
		//
		//pseudomove
		//move = {};
		//move.name <- unitName;
		//move.movement <- Vector2;
		//move.boardScore <- Vector2;
		//
		//WHAT IS A PLY?
		//GIVE IT A FIELD
		//SINGLE PLY:
			//FIND MOVES
			//FOR EACH MOVE
				//HYPOTHETICALLY APPLY MOVE
				//CALCULATE BOARD SCORE OF SINGLE MOVE
				//SAVE MOVE IN ARRAYLIST (AS HASH)
			//RETURN ARRAYLIST OF MOVES (HASH TABLES)
		//DETERMINE HIGHEST MOVE SCORE
		//CHECK EACH CHAIN OF SCORES WITH PLY FACTOR
		


		return new ArrayList ();
	}



}
