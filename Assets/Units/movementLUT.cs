using UnityEngine;
using System.Collections;

public class movementLUT : MonoBehaviour {
	public static movementLUT Instance;

	void Awake(){
		Instance = this;
	}



	private string typeA = "Spear";
	private string typeB = "Nothing";
	public int activeHigh = 10;
	public int activeLow = 0;
	public int circumference = 20;
	public Texture2D textureNotFound; 
	public ArrayList returnAllMovements(){
		return determineMoves (typeA, typeB);
	}

	public Hashtable determineFlatMoves(int x, int y, string classA, string classB){
		Hashtable returnMoves = new Hashtable ();
		ArrayList determinedMoves = determineMoves (classA, classB);
		for (int z = 0; z < determinedMoves.Count; z++) {
			Vector2 currentMove = (Vector2)determinedMoves [z];
			int newX = x + (int) currentMove.x;
			int newY = y + (int) currentMove.y;
			if (newY < activeHigh && newY > activeLow) {
				if (newX >= circumference) {
					newX = newX - circumference;
				} else if (newX < 0) {
					newX = newX + circumference;
				}
				string keyString = newX.ToString () + "_" + newY.ToString ();
				returnMoves [(string) keyString.Clone()] = "n";
			}
		}
		return (Hashtable) returnMoves.Clone ();
	}

	public ArrayList determineMoves (string classA, string classB){
		ArrayList returnList = new ArrayList ();
		if (classA == "Spear" || classB == "Spear") {
			returnList.Add(new Vector2 (2, 2));
			returnList.Add(new Vector2 (-2, 2));
			returnList.Add(new Vector2 (2, -2));
			returnList.Add(new Vector2 (-2, -2));
			returnList.Add(new Vector2 (0, 2));
		}
		if (classA == "Dagger" || classB == "Dagger") {
			returnList.Add (new Vector2 (1, 0));
			returnList.Add (new Vector2 (0, 1));
			returnList.Add (new Vector2 (-1, 0));
			returnList.Add (new Vector2 (0, -1));
			returnList.Add (new Vector2 (1, 1));
			returnList.Add (new Vector2 (-1, 1));
			returnList.Add (new Vector2 (1, -1));
			returnList.Add (new Vector2 (-1, -1));
		}
		if (classA == "Flail" || classB == "Flail") {
			returnList.Add (new Vector2 (0, 1));
			returnList.Add (new Vector2 (0, -1));
			returnList.Add (new Vector2 (1, 0));
			returnList.Add (new Vector2 (-1, 0));
			returnList.Add (new Vector2 (2, 0));
			returnList.Add (new Vector2 (-2, 0));
			returnList.Add (new Vector2 (3, 0));
			returnList.Add (new Vector2 (-3, 0));
		}
		if (classA == "Sword" || classB == "Sword") {
			returnList.Add (new Vector2 (2, 0));
			returnList.Add (new Vector2 (-2, 0));
			returnList.Add (new Vector2 (2, 1));
			returnList.Add (new Vector2 (-2, 1));
			returnList.Add (new Vector2 (1, 0));
			returnList.Add (new Vector2 (-1, 0));
			returnList.Add (new Vector2 (1, 1));
			returnList.Add (new Vector2 (-1, 1));
		}
		return returnList;
	}

	public ArrayList getMovesForFakeUnit(string classA, string classB, int[] space){

		return new ArrayList ();
	}

	public Texture2D findClassTexture(string classA, string classB, bool playerOwned){
		Texture2D loadedTexture = (Texture2D) Resources.Load ("UnitTextures\\" + classA + "_" + classB);
		if (loadedTexture != null) {
			return loadedTexture;
		} else {
			return textureNotFound;
		}
	}

	public ArrayList returnValidAttacks(){
		ArrayList returnList = new ArrayList();
		if (typeA == "pawn") {
			returnList.Add(new Vector2(0,1));
		}
		return returnList;
	}
		


	public void changeClass(string a, string b){
		typeA = a;
		typeB = b;
	}
}
