using UnityEngine;
using System.Collections;

public class tileScript : MonoBehaviour {
	public int boardPositionX = 0;
	public int boardPositionY = 0;
	public bool validMove = false;
	public Texture2D idleTexture;
	public Texture2D validMoveTexture;
	public GameObject objectOnTile;
	/*
	void Start(){
		objectOnTile = new GameObject ();
	}
	*/

	public void setObjectOnTile (GameObject setTo){
		objectOnTile = setTo;
	}

	
	// Update is called once per frame
	void Update () {
		Texture2D v = validMoveTexture;
		Texture2D i = idleTexture;
		if (validMove == true) {
			GetComponent<Renderer> ().material.mainTexture = validMoveTexture;
		} else {
			GetComponent<Renderer> ().material.mainTexture = idleTexture;
		}
	}
}
