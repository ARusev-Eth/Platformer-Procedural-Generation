using UnityEngine;
using System.Collections;

public class DamageScript : MonoBehaviour {

	// Publics
	public bool isPlayer;
	public float dmgAmount;

	// Privates


	private void OnTriggerEnter2D (Collider2D c)
	{
		if (isPlayer) {

			if (c.gameObject.tag == "Enemy"){
				
			}
			
		} else {
			if (c.gameObject.tag == "Player") {
				GameObject.FindGameObjectWithTag("Player").GetComponent<_Player>().Hurt (20, 2);
			}
		}
	}

}
