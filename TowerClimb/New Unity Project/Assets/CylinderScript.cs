using UnityEngine;
using System.Collections;
/*
public class CylinderScript : MonoBehaviour {

	public int numSides;
	public GameObject Ring;
	public Transform Cylinder;
	private ArrayList RingList = new ArrayList ();
	private string relativePath;

	void createRing(int numberOfSides, float height, string name){
		GameObject toInit = (GameObject)Instantiate (Ring, new Vector3 (0f, height, 0f), Quaternion.identity);
		//TODO: change the name to something useful!
		toInit.name = name;
		//Instantiate (toInstantiate, new Vector3 (0f, (float)i, 0f), Quaternion.identity);
		int ringSize = numberOfSides;
		toInit.GetComponent < CreateRing> ().initRing (ringSize);
		toInit.transform.SetParent (Cylinder);
		RingList.Add(toInit);
		numSides = numberOfSides;
	}

	Texture2D getCylinderTextures(int tileNumber){
		Texture2D tex = Resources.Load ("cylinderData/CylinderA/Materials/grass" + tileNumber.ToString(), typeof(Texture2D)) as Texture2D;
		return tex;
	}

	// Use this for initialization
	public void init (float cylinderLength, float cylinderHeight, int numberOfSides, string name) {
		Cylinder = new GameObject ("Cylinder").transform;
		for (int i = 0; i < cylinderLength; i++) {
			createRing (numberOfSides, (float)i + (float)cylinderHeight, name + "_" + i.ToString ());
		}
	}

}
*/