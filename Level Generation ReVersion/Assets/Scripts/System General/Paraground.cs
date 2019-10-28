using UnityEngine;
using System.Collections;

public class Paraground : MonoBehaviour {

	// Publics
	public GameObject player;
	public float useSpeed;
	public float useSpeedX;
	public float useSpeedY;
	public bool control;


	// Update is called once per frame
	void Update () {

		if (control) {
			if (player.GetComponent<Rigidbody2D>().velocity.x > 0f) {
				Move (1);
			} else if (player.GetComponent<Rigidbody2D>().velocity.x < 0f) {
				Move (-1);
			}
		} else if (!control) {
			Move (0);
		}
	}

	private void Move (short s)
	{
		if (s != 0) {
			GetComponent<Renderer>().material.mainTextureOffset =
				new Vector2 (GetComponent<Renderer>().material.mainTextureOffset.x + useSpeed * s, 0);
		} else if (s == 0) {
			GetComponent<Renderer>().material.mainTextureOffset = 
				new Vector2 (GetComponent<Renderer>().material.mainTextureOffset.x + useSpeedX, GetComponent<Renderer>().material.mainTextureOffset.y + useSpeedY);
		}
	}
}
