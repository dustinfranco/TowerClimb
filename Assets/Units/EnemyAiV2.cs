using UnityEngine;
using System.Collections;
using System.IO;

public class EnemyAiV2 : MonoBehaviour {
	private bool debug = false;

	public int movesCalculated = 0;
	//this is all from the point of view of the enemy
	public int defendScore = 1000;
	public int attackScore = 1;
	public int safeScore = 100;
	public int threatScore = 10;

	public static movementLUT aimoveLUT = new movementLUT(); 
	public int plyPlus = 0;

	public Hashtable test(bool debug){
		Hashtable returnMove = decideMove (3);
		return returnMove;

	}

	//could be changed to a hashtable later?
	public Hashtable decideMove(int initialPlyNumber){
		board initialField = flattenedInitialField (initialPlyNumber);

		//start here
		//object b = t["key"];
		//Type typeB = b.GetType();

		// If ID is a property
		//object value = b.GetType().GetProperty("PScore").GetValue(b, null);
		//end here

		Hashtable returnMove = new Hashtable ();
		returnMove.Add("score", -1000000);
		//returnMove.Add ("unit", "");
		returnMove.Add("move", "");
		foreach (string unitName in initialField.OS.Keys) {
			unit currentUnit = (unit)initialField.OS [unitName];
			foreach (string moveKey in currentUnit.moves.Keys) {
				Hashtable movesHash = (Hashtable) currentUnit.moves;
				if (movesHash [moveKey].GetType () == "s".GetType ()) {
					Debug.Log ("String");
				} else {
					board currentMoveBoard = ((board)movesHash [moveKey]);
					if (currentMoveBoard.PScore > (int)returnMove["score"]) {
						returnMove["score"] = (int)currentMoveBoard.PScore;
						returnMove ["move"] = (string)currentMoveBoard.sequence;
					}
					Debug.Log(((board) movesHash [moveKey]).PScore);
				}
			}
		}
		//initialField.printUnitInfo ();
		//initialField.printBoardInfo ();
		return returnMove;
	}



	/// 
	/// UNIT CLASS
	///

	class unit {
		public int x = 0;
		public int y = 0;
		public string xys = "";
		public bool playerOwned = true;
		public string classA = "nothing";
		public string classB = "nothin1g";
		public Hashtable moves = new Hashtable ();

		public unit(int inX, int inY, bool inTeam, string inClassA, string inClassB) {
			x = inX;
			y = inY;
			xys = x.ToString() + "_" + y.ToString();
			playerOwned = inTeam;
			classA = inClassA;
			classB = inClassB;
		}

		public void populateMoves(Hashtable occupiedIn) {
			///Debug.Log("populateMoves x : " + x.ToString());
			///Debug.Log("populateMoves y : " + y.ToString());
			///Debug.Log("populateMoves xys : " + xys);
				
			moves = (Hashtable) aimoveLUT.determineFlatMoves(x,y,classA,classB);
		}

		public unit Clone(){
			unit newUnit = new unit (x, y, playerOwned, classA, classB);
			return newUnit;
		}

		public void clearMoves(){
			moves = new Hashtable ();
		}
		//DEBUG STUFF UNIT

		public void printMoves(){
			if (playerOwned) {
				///Debug.Log ("PLAYER UNIT AT: " + xys + " HAS " + moves.Count.ToString () + " MOVES:");
				int counterInt = 0;
				foreach(string key in moves.Keys){
					///Debug.Log("PLAYER UNIT AT: " + xys  + " MOVE #: " + counterInt.ToString() + " MOVE: " + key);
				}
			}
			if (!playerOwned) {
				///Debug.Log ("ENEMY UNIT AT: " + xys + " HAS " + moves.Count.ToString () + " MOVES:");
				int counterInt = 0;
				foreach(string key in moves.Keys){
					///Debug.Log("ENEMY UNIT AT: " + xys  + " MOVE #: " + counterInt.ToString() + " MOVE: " + key);
				}
			}
		}
	}

	///
	/// BOARD CLASS
	///

	class board {
		public int PScore = 0;
		public int EScore = 0;
		public int activeHigh = 0;
		public int activeLow = 0;
		public int Circumference = 0;
		private static int defendScore = 1;
		private static int attackScore = 1000;
		public Hashtable OS = new Hashtable();
		public string sequence = "";

		public board(int inHeight, int inBottom, int inCircumference, Hashtable inOS, int plyNumber, string applyUnitString, string applyLocationString, string currentName, bool playerTurn){
			activeHigh = inHeight;
			activeLow = inBottom;
			sequence = currentName;
			Circumference = inCircumference;
			OS = inOS;
			if(applyUnitString != "" && applyLocationString != ""){
				applyMove(applyUnitString, applyLocationString);
			}
			popMoves();
			calculateCurrentScores();
			if(plyNumber > 0){
				int plyNumberMinusMinus = plyNumber - 1;
				popMoveBoards (plyNumberMinusMinus, playerTurn);
			}
			//This logs all moves 
			printSingleBoard(plyNumber);
		}

		public void applyMove(string currentUnitString, string targetLocationString){

			sequence += currentUnitString + "-" + targetLocationString + "~";
			Hashtable clonedOS = (Hashtable)OS.Clone ();
			if(OS.ContainsKey(targetLocationString)){
				clonedOS.Remove(targetLocationString);
			}
			unit currentUnit = (unit) clonedOS[currentUnitString];
			string[] xy = targetLocationString.Split('_');
			int newX = int.Parse((string) xy[0]);
			int newY = int.Parse((string) xy[1]);
			clonedOS.Add (targetLocationString, new unit (newX, newY, currentUnit.playerOwned, currentUnit.classA, currentUnit.classB));
			clonedOS.Remove(currentUnitString);
			OS = new Hashtable ((Hashtable) clonedOS.Clone ());
		}

		//this should fill the moves boards and all boards in those unitl ply is 0
		//this should no longer be 
		void popMoveBoards(int plyNumber, bool playerPly){
			Hashtable clonedOS = (Hashtable)OS.Clone ();
			foreach (string unitKey in clonedOS.Keys) {
				unit currentUnit = (unit) clonedOS[unitKey];
				//if the unit is on the team currently making the move
				if (currentUnit.playerOwned == playerPly) {
					Hashtable currentUnitMovesClone = (Hashtable)currentUnit.moves.Clone ();
					//for each move in unit
					foreach (string moveKey in currentUnit.moves.Keys) {
						//move is to a location with a unit
						currentUnitMovesClone [moveKey] = new board (activeHigh, activeLow, Circumference, (Hashtable)OS.Clone (), plyNumber, (string)unitKey.Clone (), (string)moveKey.Clone (), sequence, !playerPly);
						currentUnit.moves = (Hashtable)currentUnitMovesClone;
					}
				}
			}
			OS = clonedOS;
		}

		public void addUnit(int inX, int inY, bool inTeam, string inClassA, string inClassB){
			OS [inX.ToString () + "_" + inY.ToString ()] = new unit (inX, inY, inTeam, inClassA, inClassB);
		}

		public void popMoves(){
			foreach(string key in OS.Keys){
				unit currentUnit = (unit) OS[key];
				currentUnit.clearMoves ();
				currentUnit.populateMoves (OS);
			}
		}

		public void ply(bool playerTeam, int plyLayer, float keepTopPercent){
			

		}

		//

		//for each layer in plyNumber
			//populate the opposite team of the last ply's moves

		void plyMoveBoards(bool playerTeam, int plyNumber, float keepTopPercent){
			Hashtable clonedOS = (Hashtable)OS.Clone ();
			foreach (string unitKey in clonedOS.Keys) {
				unit currentUnit = (unit) clonedOS[unitKey];
				Hashtable currentUnitMovesClone = (Hashtable) currentUnit.moves.Clone();
				//for each move in unit
				foreach (string moveKey in currentUnit.moves.Keys) {
					//move is to a location with a unit
					currentUnitMovesClone[moveKey] = new board(activeHigh,activeLow,Circumference,(Hashtable) OS.Clone(), plyNumber, (string) unitKey.Clone(), (string)moveKey.Clone(), sequence, true);
				}
				currentUnit.moves = (Hashtable)currentUnitMovesClone;
			}
			OS = clonedOS;
		}

		public int getScore(){
			return PScore;
		}

		public void calculateCurrentScores(){
			//for all unit moves, check OS for key,
				//if you find the key in OS, check the teams of the unit and the key
				//score appropriately
			PScore = 0;
			EScore = 0;
			//for each unit in board
			foreach (string unitKey in OS.Keys) {
				unit currentUnit = (unit) OS[unitKey];
				//for each move in unit
				foreach (string moveKey in currentUnit.moves.Keys) {
					//move is to a location with a unit
					if(OS.ContainsKey(moveKey)){
						unit targetUnit = (unit) OS[moveKey];
						if(currentUnit.playerOwned){
							//we are looking at an enemy unit move
							if(targetUnit.playerOwned){
								//player unit can defend an ally
								PScore += defendScore;
							} else {
								//player unit can attack an enemy
								PScore += attackScore;
							}
						} else {
							//we are looking at an enemy unit move
							if(targetUnit.playerOwned){
								//enemy unit can attack player unit
								EScore += attackScore;
							} else {
								//enemy unit can defend other unit
								EScore += defendScore;
							}
						}
					}
				}
			}
		}

		public void applyMove(string moveIn){
			
		}

	
		//DEBUG STUFF BOARD

		public void printSingleBoard(int layerIn){
			if(layerIn == 0){
				int layer = 0 + layerIn;
				string printString = "";
				string layerAdd = "   ";
				string LA = "\n";
				for (int i = 0; i < layer; i++){
					LA += layerAdd;
				}
				layer += 1;
				printString += LA + sequence;
				printString += LA + layerAdd + "PSCORE: " + PScore;
				printString += LA + layerAdd + "ESCORE: " + EScore;
				printString += LA + layerAdd + "UNITS: ";
				foreach (string key in OS.Keys) {
					layer += 1;
					unit currentUnit = (unit)OS [key];
					printString += LA + layerAdd + layerAdd + "UNIT AT: " + key;
					layer += 1;
					printString += LA + layerAdd + layerAdd + layerAdd + key;
					printString += LA + layerAdd + layerAdd + layerAdd + "PLAYER OWNED: " + currentUnit.playerOwned;
					printString += LA + layerAdd + layerAdd + layerAdd + "MOVES:";
					foreach (string moveKey in currentUnit.moves.Keys) {
						layer += 1;
						printString += LA + layerAdd + layerAdd + layerAdd + layerAdd + moveKey;
						//printString += LA + layerAdd + layerAdd + layerAdd + layerAdd + layerAdd + "PSCORE: " + ((board) currentUnit.moves [moveKey]).PScore.ToString();
						//printString += LA + layerAdd + layerAdd + layerAdd + layerAdd + layerAdd + "ESCORE: " + ((board)currentUnit.moves [moveKey]).EScore.ToString ();

					}

				}
				FileStream createFile = new FileStream ("Logs/" + sequence + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
				createFile.WriteByte (1);
				createFile.Close ();
				StreamReader reader = new StreamReader ("Logs/" + sequence + ".txt");
				string textSoFar = "";
				textSoFar += reader.ToString ();
				reader.Close ();
				//string FileName = "Logs/" + sequence + ".txt"; // This contains the name of the file. Don't add the ".txt"
				// Assign in inspector
				//TextAsset asset; // Gets assigned through code. Reads the file.
				//StreamWriter writer; // This is the writer that writes to the file
				StreamWriter writer = new StreamWriter("Logs/" + sequence + ".txt"); // Does this work?
				writer.WriteLine(textSoFar + printString);
				writer.Flush ();
				writer.Close ();
				//Debug.Log (printString);
				//System.IO.File.WriteAllText("Logs/UnityLog.txt", printString);
			}
		}

		public void printBoardInfo(){
			///Debug.Log ("PScore: " + PScore.ToString ());
			///Debug.Log ("EScore: " + EScore.ToString ());
			///Debug.Log ("activeHigh: " + activeHigh.ToString ());
			///Debug.Log ("activeLow: " + activeLow.ToString ());
			///Debug.Log ("Circumference: " + Circumference.ToString ()); 

		}

		public void printAllUnitsOneLine(){
			string logout = sequence + " UNITS IN THIS BOARD : ";
			foreach (string key in OS.Keys) {
				logout += key;
				logout += " AND ";
			}
			///Debug.Log (logout);
					
		}

		public void printUnitInfo(){
			foreach (string key in OS.Keys) {
				unit val = (unit)OS [key];
				if (val.playerOwned) {
					///Debug.Log ("PLAYER UNIT AT " + val.xys + " CLASSES: " + val.classA + " AND " + val.classB);
				} else {
					///Debug.Log ("ENEMY UNIT AT " + val.xys + " CLASSES: " + val.classA + " AND " + val.classB);
				}
				val.printMoves ();
			}
		}
	}

	//PRETTY MUCH FINALIZED BELOW HERE:

	private board flattenedInitialField(int initialPlyNumber){
		Hashtable initialSurvey = surveyField ();

		//Get lists together and cloned
		//unit lists:
		ArrayList tempPU = (ArrayList) initialSurvey["playerUnits"];
		ArrayList clonedPU = (ArrayList) tempPU.Clone ();
		ArrayList tempEU = (ArrayList) initialSurvey["enemyUnits"];
		ArrayList clonedEU = (ArrayList) tempEU.Clone ();
		//Hashtable tempOS = (Hashtable) initialSurvey ["occupiedSpaces"];
		//Hashtable clonedOS = (Hashtable) tempOS.Clone ();
		int circumference = (int) initialSurvey["cylinderCircumference"];
		int activeHigh = (int) initialSurvey["activeFieldHigh"];
		int activeLow = (int) initialSurvey["activeFieldLow"];
		Hashtable OS = new Hashtable ();

		//get units into hash table OS to put into new board
		for (int x = 0; x < clonedPU.Count; x++) {
			GameObject currentUnit = (GameObject)clonedPU [x];
			int tempX = currentUnit.GetComponent<UnitScript> ().boardLocationX;
			int tempY = currentUnit.GetComponent<UnitScript> ().boardLocationY;
			bool tempTeam = currentUnit.GetComponent<UnitScript> ().playerOwned;
			string tempClassA = currentUnit.GetComponent<UnitScript> ().getClass ("a");
			string tempClassB = currentUnit.GetComponent<UnitScript> ().getClass ("b");

			unit currentUnitAsUnit = new unit (tempX, tempY, tempTeam, tempClassA, tempClassB);
			OS [tempX.ToString () + "_" + tempY.ToString ()] = currentUnitAsUnit;
		}

		for (int y = 0; y < clonedEU.Count; y++) {
			GameObject currentUnit = (GameObject)clonedEU [y];
			int tempX = currentUnit.GetComponent<UnitScript> ().boardLocationX;
			int tempY = currentUnit.GetComponent<UnitScript> ().boardLocationY;
			bool tempTeam = currentUnit.GetComponent<UnitScript> ().playerOwned;
			string tempClassA = currentUnit.GetComponent<UnitScript> ().getClass ("a");
			string tempClassB = currentUnit.GetComponent<UnitScript> ().getClass ("b");

			unit currentUnitAsUnit = new unit (tempX, tempY, tempTeam, tempClassA, tempClassB);
			OS [tempX.ToString () + "_" + tempY.ToString ()] = currentUnitAsUnit;
		}

		board InitialBoard = new board(activeHigh, activeLow, circumference, (Hashtable) OS.Clone(), initialPlyNumber, "","","", true);
		return InitialBoard;
	}

	private Hashtable surveyField (){
		Hashtable returnHash = GetComponent<PlayField> ().GetBoardInformation ();
		return returnHash;
	}
		

}
