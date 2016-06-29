using UnityEngine;
using System.Collections;
//using System;

public class CreateRing : MonoBehaviour {
	public GameObject tile;
	private GameObject centerPoint;
	private int ringSize = 0;
	private ArrayList tileList = new ArrayList();
	private Transform boardHolder;
	private Quaternion LastRotation;
	private Quaternion NextRotation;
	private float startTime = 0f;
	private Transform ghostBoardHolder;
	//private bool initialed = false;
	/************************
	 Angle Centric Functions
	************************/


	//RiemanSin Function
	private float riemannSin(int n, int isides){
		float sides = (float)isides;
		float output = 0.0f;
		float nPlus = n+1;
		for (int p = 0; p < n; p++) {
			output += Mathf.Sin ((float)p * Mathf.PI / (sides / 2.0f));
		}
		return output;
	}
	//RiemanCos Function
	private float riemannCos(int n, int isides){
		float sides = (float)isides;
		float output = 0.0f;
		float nPlus = n+1;
		for (int p = 0; p < n; p++) {
			output += Mathf.Cos ((float)p * Mathf.PI / (sides / 2.0f));
		}
		return output;
	}
	//Average Two Floats Function
	private float avg(float a, float b){
		return (float)((a + b) / 2.0f);
	}

	/******
	 Other
	******/

	public Vector3[] createRingArrayList(int sides){
		Vector3[] tempVectArr = new Vector3[sides];
		for (int i = 0; i < sides; i++) {
			float xVerta = riemannCos (i, sides);
			float xVertb = riemannCos (i+1, sides);
			float zVerta = riemannSin (i, sides);
			float zVertb = riemannSin (i+1, sides);
			tempVectArr[i] = new Vector3 ((xVerta+xVertb)/2.0f - 0.5f, 0f, (zVerta+zVertb)/2.0f);
		}
		int halfVertList = sides / 2;
		Vector3 tempz = tempVectArr[halfVertList];
		float centerz = tempz.z/2.0f;
		for (int i = 0; i < sides; i++) {
			tempVectArr[i] = (Vector3) tempVectArr[i] - new Vector3 (0.0f, 0.0f, centerz);
		}
		return tempVectArr;
	}

	public Quaternion[] createRingAngleArrayList(int sides){
		Quaternion[] tempVectArr = new Quaternion[sides];
		for (int i = 0; i < sides; i++) {
			tempVectArr[i] = Quaternion.Euler (0, 90f - (float)i * 360f / (float)sides,-90f);
		}
		return tempVectArr;
	}


	/*
	 //initialize the ring


	public void initRing(int NumberOfSides){
		ringSize = NumberOfSides;
		//how far in the y direction it should be created.
		float height = transform.position.y;
		//calculate centerz
		float centerz = 0.0f;
		//create the ring array list
		createRingArrayList (NumberOfSides);


		//create the tiles
		float fVertCount = (float) VertList.Count;
		Quaternion rotation = Quaternion.identity;
		boardHolder = new GameObject ("RingCenter").transform;
		boardHolder.position = (new Vector3 (0.0f, height+0.5f, centerz));
		for (int k = 0; k < VertList.Count; k++) {
			rotation.eulerAngles = new Vector3 (0.0f, -90.0f-(float)k * 360.0f/fVertCount, 90);
			//Debug.Log ((Vector3)VertList [k]);
			GameObject tileSpawn = (GameObject)Instantiate (tile, (Vector3) VertList[k], rotation); 
			tileSpawn.transform.SetParent(boardHolder);
			tileSpawn.name = k.ToString ();
			tileSpawn.GetComponent<TileScript>().Init (NumberOfSides);
			tileList.Add (tileSpawn);
		}
		boardHolder.SetParent (gameObject.transform);
		paintRingFromFile ();
		loadObjectsFromFile ();
		LastRotation = (Quaternion)boardHolder.GetComponent<Transform>().rotation;
		NextRotation = LastRotation;

	}

	 //Texture Handling

	//TODO: build this out to try and get a texture based on name
	Texture2D getCylinderTextures(string cylinderName, int x){
		string yValue = name.Substring (name.IndexOf ("_") + 1, name.Length - name.IndexOf ("_") -1);
		string stringBuild = "cylinderData/CylinderTextures/" + cylinderName + "/" + yValue + "_" + x.ToString ();
		Texture2D tex = Resources.Load (stringBuild, typeof(Texture2D)) as Texture2D;

		return tex;
	}

	private ArrayList getRandomTextures(string cylinderName){
		ArrayList texturesArrayList = new ArrayList ();
		int itera = 0;
		string StringBuild = "cylinderData/CylinderTextures/" + cylinderName + "/FloorTiles/FloorTile";
		Texture2D currentTex = Resources.Load (StringBuild + itera.ToString(), typeof(Texture2D)) as Texture2D;
		//Debug.Log (StringBuild + itera.ToString());

		while(currentTex){	
			texturesArrayList.Add(currentTex);
			itera += 1;
			currentTex = Resources.Load (StringBuild + itera.ToString(), typeof(Texture2D)) as Texture2D;
		}
		return texturesArrayList;
	}

	//TODO: build this out into two functions; the main one and one that gets a random floor tile (opposite of getCylinderTextures)
	public void paintRingFromFile(){
		//get the name of the folder
		string nameSubStrY = name.Substring (0, name.LastIndexOf ("_"));
		ArrayList randomTextures = getRandomTextures (nameSubStrY);
		//boardHolder.GetComponentInChildren<Renderer>().material.mainTexture = paint;
		for (int x = 0; x < tileList.Count; x++) {
			GameObject paintTile = (GameObject)tileList [x];
			Texture2D tex = getCylinderTextures (nameSubStrY, x);
			if (tex == null) {
				//TODO: get random tile in nameSubStr
				if (randomTextures.Count > 0) {
					tex = (Texture2D)randomTextures [Random.Range (0, randomTextures.Count - 1)];
				}
				if (tex == null) {
					tex = Resources.Load ("cylinderData/CylinderTextures/TextureNotFound", typeof(Texture2D)) as Texture2D;
				}
			}
			paintTile.GetComponent<Renderer>().material.mainTexture = tex;
		}
	}

	//Items
	
	private GameObject getCylinderStaticItems(string cylinderName, int itemLocation){

		string yValue = name.Substring (name.IndexOf ("_") + 1, name.Length - name.IndexOf ("_") -1);
		string stringBuild = "cylinderData/CylinderObjects/" + cylinderName + "/" + yValue + "_" + itemLocation.ToString ();
		return (GameObject)Resources.Load (stringBuild);
	}


	void loadObjectsFromFile(){
		string nameSubStrY = name.Substring (0, name.LastIndexOf ("_"));
		for (int x = 0; x < tileList.Count; x++) {
			GameObject StaticItem = getCylinderStaticItems (nameSubStrY, x);
			if (StaticItem) {
				Debug.Log (StaticItem.name);
				StaticItem.name = name + x.ToString ();
				Vector3 Location = ((GameObject)tileList [x]).GetComponent<Transform> ().position;
				Vector3 Angle = ((GameObject)tileList [x]).GetComponent<Transform> ().rotation.eulerAngles;
				StaticItem.transform.position = Location;
				StaticItem.transform.eulerAngles = Angle;
				StaticItem = Instantiate (StaticItem);
				StaticItem.transform.SetParent (boardHolder);

			}
		}
	}
	*/
}
