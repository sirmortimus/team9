using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePiece : MonoBehaviour {

	//for the locking
	public string pieceStatus = "idle";
	//for the particles
	public Transform edgeParticles;
	//could put in public variable for sound here

	//for checking correct location
	public string checkPlacement = "";



	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (pieceStatus == "pickedup") {
			Vector2 mousePosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			Vector2 objPosition = Camera.main.ScreenToWorldPoint (mousePosition);
			transform.position = objPosition;
		}

		//need canvas for Use Left Click to grab pieces, right to place

		if ((Input.GetMouseButtonDown(1)) && ( pieceStatus == "pickedup"))
		{

			checkPlacement = "y";
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if ((other.gameObject.name == gameObject.name) && (checkPlacement=="y"))
		{
			other.GetComponent<BoxCollider2D> ().enabled = false;
			GetComponent<BoxCollider2D> ().enabled = false;

			GetComponent<Renderer> ().sortingOrder = 0;

			transform.position = other.gameObject.transform.position;
			pieceStatus = "locked";

			Instantiate(edgeParticles,other.gameObject.transform.position, edgeParticles.rotation);
			checkPlacement = "n";

			GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1);


			//then have command for play sound here
		}

		if ((other.gameObject.name != gameObject.name) && (checkPlacement=="y"))
		{
			GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, .5f);
			checkPlacement = "n";
		}


	}

	void OnMouseDown()
	{
		pieceStatus = "pickedup";
		checkPlacement = "n";
		GetComponent<Renderer> ().sortingOrder = 10;


	}
}


