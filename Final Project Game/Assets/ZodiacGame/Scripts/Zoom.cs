using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour {
	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


	
	}

	void OnTriggerEnter () {
		if (Input.GetMouseButtonDown(0)) {

			transform.localScale = new Vector3 (1, 1, 0);
			transform.localPosition = new Vector3 (0, 0, 0);
		}

		if (Input.GetMouseButtonDown (1)) {

			transform.localScale = new Vector3 (.2f, .25f, 0);
			transform.localPosition = new Vector3 (-8.79f, 5.25f, 0);

		}
	}




}
