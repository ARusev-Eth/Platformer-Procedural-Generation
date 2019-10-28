using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	// Publics
	public Transform player; 		// Target to follow

	// Privates
	public float maxLeft;
	public float maxRight;
	public float maxHeight;
	public float minHeight;
		
	public void CamSet () {
		this.transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, transform.position.z);
	}

	// Same as Update, always called after update
	void Update() {
		Follow ();
	}

	// Follow player within limits
	private void Follow () {
		// If the player isn't too far left or too far right - follow em.
		if (player.transform.position.x > maxLeft && player.transform.position.x < maxRight) {
			transform.position = new Vector3 (player.transform.position.x, transform.position.y, transform.position.z);
		} else if (player.transform.position.x < maxLeft) {
			transform.position = new Vector3 (maxLeft, transform.position.y, transform.position.z);
		} else if (player.transform.position.x > maxRight) {
			transform.position = new Vector3 (maxRight, transform.position.y, transform.position.z);
		}

		if (player.transform.position.y < maxHeight && player.transform.position.y > minHeight) {
			transform.position = new Vector3 (transform.position.x, player.transform.position.y, transform.position.z);
		}
	}
}
