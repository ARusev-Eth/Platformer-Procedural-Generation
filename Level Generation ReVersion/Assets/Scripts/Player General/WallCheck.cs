using UnityEngine;
using System.Collections;

public class WallCheck : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D c) {
		if (c.tag == "Ground") {
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement> ().wallHit = true;
		}
	}

	void OnTriggerStay2D (Collider2D c) {
		if (c.tag == "Ground") {
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement> ().wallHit = true;
		}
	}

	void OnTriggerExit2D (Collider2D c) {
		GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement> ().wallHit = false;
	}


}
