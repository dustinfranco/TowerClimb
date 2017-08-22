using UnityEngine;
using System.Collections;
using System.IO;




public class EnemyAiV3 : MonoBehaviour {
	public static movementLUT aimoveLUT = new movementLUT(); 


	/* Prioritize:
		x1. can take a unit next and is defended
		x2. can take a unit and is undefended
		3. can take a unit next turn and is defended
		4. can take a unit next turn but is undefended
		
		5. move towards enemy and is defended
		6. move towards enemy and is undefended
		7. random? serious lack of options at this point
	*/

	/*
	the board is defined by spaces that have 3 attributes:
	the enemy has 0, 1, or 2+ moves to it, AND THE TYPE OF UNITS THAT HAVE THIS MOVE
	the player has 0, 1, or 2+ moves to it
	it is already occupied by the enemy (making it an invalid move but a defended spot - ignoring for now)
	it is already occupied by the player
	*/
	/*
	Create a grid like this (enemy valid moves grid)
	0,0,0,0,0,0
	0,0,0,0,0,0
	0,0,2,0,2,0
	0,0,0,1,0,0
	0,0,0,0,0,0
	0,0,0,0,0,0

	Create a grid like this (player valid moves grid):
	0,0,0,0,0,0
	0,0,0,0,0,0
	0,0,2,0,2,0
	0,0,0,1,0,0
	0,0,0,0,0,0
	0,0,0,0,0,0

	Create a list of player locations like x_y
	playerUnitLocations = [x_y, z_w, s_t]
		enemyUnitLocations = [a_b, c_d, e_d]
		enemyMoves = [x_y, c_d, z_w, t_y]
		playerMoves = [z_w]

		-> ENEMY MOVES INCLUDE PLAYER LOCATIONS
		since x_y AND z_w are player locations we should choose one of them
		since z_w is defended by the player, let's pick x_y




	*/

	/*How to do this:
	1. find all spaces that 2 + enemies can move to that are occupied by a player piece
	- find all moves with minimum of player movements to them and return one
		2. find all spaces that only 1 enemy can move to that are occupied by a player piece
		- find all moves with minimum of player movements to them and return one

			//SECTION 3 MIGHT DEFEAT THE PURPOSE
			3. find enemy units that are immediately threatened
			BASICALLY SETTING A TRAP:
			- find spaces where the the player can move one or more, and the enemy can move more than one
			REMOVE YOURSELF FROM DANGER:
			-find spaces where the player can't move AT ALL or else skip
			4. moving towards enemy
			//threatening
			-find spots nearer the closest player that 2+ enemies can move to that offer a threatening position
			-find spots nearer the closest player that 2+ enemies can move to that they can move to
			-find spots nearer the closest player that offer a threatening position and they cannot move to
			-find spots nearer the closest player that offer a threatening position and they can move to
			-find spots nearer the closest player that 2+ enemies can move to that they cannot move to
			-find spots nearer the closest player that they cannot move to
			-suicide (move to an undefended spot closer to them that they can move to)
			-make a random move

			*/

			/*

			THIS SECTION COVERS 1. and 2.
			get a list of every enemy move and every player move
			add them together into two hash tables:
			one for enemy moves one for player {"space" : number of units that can move to the space}
	for each move in enemy moves
		if it exists in the enemy moves table add 1
		if it doesn't, add it
			for each move in player moves
				if it exists in the player moves table add 1
				if it doesn't, add it
					search list of player units:
					collect all enemy moves that are in player units
					(find strings that are in both)
				if there is one:
					find the highest number of enemy that can move to it
				if theres a tie:
					find the lowest number of players that can move to it
					THIS SECTION COVERS 3.
					search list of enemy units:
				if a player move is in this list,

					*/


	//DECIDE MOVE
	public string decideMove(){
		string returnMove = flattenInitialField ();
		return returnMove;
	}

	private string findMoveInUnits(Hashtable inputOS, string inputMove){
		foreach (string unitName in inputOS.Keys) {
			Hashtable currentUnit = (Hashtable) inputOS [unitName];
			ArrayList currentMoves = (ArrayList) currentUnit ["moves"];
			if (currentMoves.Contains(inputMove)){
				Debug.Log("unit " + unitName + " contains move  " + inputMove);
				return unitName;
			}
		}
		return "";
	}

	private ArrayList returnAllPlayerMoves(ArrayList inputPU, Hashtable playerOS){
		ArrayList allPlayerMoves = new ArrayList ();

		//get units into hash table
		for (int x = 0; x < inputPU.Count; x++) {
			GameObject currentUnit = (GameObject)inputPU [x];
			int tempX = currentUnit.GetComponent<UnitScript> ().boardLocationX;
			int tempY = currentUnit.GetComponent<UnitScript> ().boardLocationY;
			string tempClassA = currentUnit.GetComponent<UnitScript> ().getClass ("a");
			string tempClassB = currentUnit.GetComponent<UnitScript> ().getClass ("b");
			string playerName = tempX.ToString () + "_" + tempY.ToString ();

			playerOS [playerName] = new Hashtable ();
			Hashtable playerOSName = (Hashtable) playerOS [playerName];
			playerOSName.Add ("class", tempClassA + "/" + tempClassB);
			ArrayList moves = (ArrayList) aimoveLUT.determineFlatMovesArray(tempX, tempY, tempClassA, tempClassB);
			playerOSName.Add ("moves", moves);
			foreach (string movementName in moves) {
				allPlayerMoves.Add (movementName);
			}
		}
		return allPlayerMoves;
	}

	private ArrayList returnAllEnemyMoves(ArrayList inputEU, Hashtable enemyOS){
		ArrayList allEnemyMoves = new ArrayList ();
		for (int y = 0; y < inputEU.Count; y++) {
			GameObject currentUnit = (GameObject)inputEU [y];
			int tempX = currentUnit.GetComponent<UnitScript> ().boardLocationX;
			int tempY = currentUnit.GetComponent<UnitScript> ().boardLocationY;
			string tempClassA = currentUnit.GetComponent<UnitScript> ().getClass ("a");
			string tempClassB = currentUnit.GetComponent<UnitScript> ().getClass ("b");
			string enemyName = tempX.ToString () + "_" + tempY.ToString ();

			enemyOS [enemyName] = new Hashtable ();
			Hashtable enemyOSName = (Hashtable) enemyOS [enemyName];
			enemyOSName.Add ("class", tempClassA + "/" + tempClassB);
			//this returns all the moves available for this unit
			ArrayList moves = (ArrayList) aimoveLUT.determineFlatMovesArray(tempX, tempY, tempClassA, tempClassB);
			//I don't think that enemy needs to have moves unique to each unit? but why not?
			enemyOSName.Add ("moves", moves);
			foreach (string movementName in moves) {
				allEnemyMoves.Add (movementName);
			}
		}
		return allEnemyMoves;
	}

	private ArrayList trimInvalidMoves(){
		return new ArrayList();
	}

	private ArrayList returnInvalidMoves(){
		return new ArrayList();
	}

	private ArrayList returnKillMoves(){
		return new ArrayList();
	}


	private int returnDangerOfMove(){
		return 0;
	}

	private int returnThreatOfMove(){
		return 0;
	}


	private string flattenInitialField(){
		Hashtable initialSurvey = GetComponent<PlayField> ().GetBoardInformation ();
		//Get lists together and cloned
		//unit lists:
		ArrayList tempPU = (ArrayList)initialSurvey ["playerUnits"];
		ArrayList clonedPU = (ArrayList)tempPU.Clone ();
		ArrayList tempEU = (ArrayList)initialSurvey ["enemyUnits"];
		ArrayList clonedEU = (ArrayList)tempEU.Clone ();
		//Hashtable tempOS = (Hashtable) initialSurvey ["occupiedSpaces"];
		//Hashtable clonedOS = (Hashtable) tempOS.Clone ();
		int circumference = (int)initialSurvey ["cylinderCircumference"];
		int activeHigh = (int)initialSurvey ["activeFieldHigh"];
		int activeLow = (int)initialSurvey ["activeFieldLow"];
		Hashtable playerOS = new Hashtable ();
		Hashtable enemyOS = new Hashtable ();
		Hashtable movesList = new Hashtable ();

		ArrayList allPlayerMoves = new ArrayList ();

		//get units into hash table

		allPlayerMoves = returnAllPlayerMoves (clonedPU, playerOS);

		ArrayList allEnemyMoves = new ArrayList ();

		allEnemyMoves = returnAllEnemyMoves (clonedEU, enemyOS);

		//finds invalid moves
		int numberOfMoves = allEnemyMoves.Count;
		Debug.Log ("Number of moves: " + numberOfMoves.ToString ());
		for(int x = numberOfMoves - 1; x > -1; x--){
			string movementName = (string) allEnemyMoves [x];
			bool isInEnemyOS = enemyOS.ContainsKey (movementName); 
			if(isInEnemyOS){
				Debug.Log(movementName + " is a an invalid move");
				allEnemyMoves.RemoveAt (x);
			}

		}
		numberOfMoves = allEnemyMoves.Count;
		Debug.Log ("Number of moves after removing invalid: " + numberOfMoves.ToString ());

		//finds kill moves
		Debug.Log("Beginning check for kill moves");
		Hashtable scoredMoves = new Hashtable();
		string bestMove = "";
		foreach (string movementName in allEnemyMoves) {
			bool isInPlayerOS = playerOS.ContainsKey (movementName); 
			if(isInPlayerOS){
				Debug.Log(movementName + " is a kill move");
				if (scoredMoves.ContainsKey (movementName)) {
					int score = (int) scoredMoves [movementName];
					scoredMoves[movementName] = score + 1;
				} else {
					scoredMoves.Add (movementName, 1);
				}
			}
			//if there is a kill move, otherwise go on to the next step
			if (scoredMoves.Count > 0) {
				int highestScore = 0;
				foreach (string move in scoredMoves.Keys) {
					int currentScore = (int)scoredMoves [move];
					if (currentScore > highestScore) {
						bestMove = move;
						highestScore = currentScore;
					}
				}
			}
		}
		if (bestMove != "") {
			bestMove = findMoveInUnits (enemyOS, bestMove) + "-" + bestMove;
			return bestMove;
		} else {
			Debug.Log ("No Kill Moves, checking for threatening moves");
		}



		Debug.Log ("UNSURE WHAT MOVE TO MAKE, making a random move");
		return "";
	}




	//this is "does input threaten the input OS
	//inputMove is like "A_B", input class is the class, inputDefensiveOS is the opposite team of the input move
	private bool determineThreat(Hashtable inputLocation, string inputClass, Hashtable inputDefensiveOS){
		
		return false;

	}



}
