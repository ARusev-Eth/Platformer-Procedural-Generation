using UnityEngine;
using System.Collections;

/*
 *	Basic aggro system - using the NPC's vision
 *	range to enter the aggressive state. First
 *	iteration - very basic, to be improved to
 *	take line of sight into consideration.
 *	20/07/2015 Game Version: 0.1f Script Version: 0.1a
 */

public class NewNoiseDetection : MonoBehaviour {


	// Publics
	public float noiseValue;	// Value to increment noise by
	public bool isEnemy;		// Is this object an aggressive NPC
	public Transform npc;		// The NPC object this is a vision range off
	public Transform player;	// The player object


	// Privates
	public bool inLoS;			// Is the target in line of sight
	private int numHits; 		// Number of objects hit by linecast

	// Once per frame
	private void Update ()
	{
		Debug.DrawLine (npc.transform.position, player.transform.position, Color.green);
		inLoS = LoSCheck (player.GetComponent<Collider2D>());
	}

	// Triggers when a game object with a 2D collider enters the trigger collider
	// Parrameter passed is the entering object
	private void OnTriggerStay2D (Collider2D c)
	{
		if (c.gameObject.tag == "Player"){
			Debug.Log ("We've got a hit");
			// If the player enters vision range, check for LoS
			if (inLoS && isEnemy){
				Debug.Log ("Is this noisy on the phone?");
				GameObject.FindGameObjectWithTag("NPC").GetComponent<SM4_R7> ().Noisy (noiseValue);
			}
		}
	}

	private void OnTriggerExit2D (Collider2D c)
	{
		if (c.gameObject.tag == "Player"){
			inLoS = false;
		}
	}

	// Returns whether object hit is in line of sight
	private bool LoSCheck (Collider2D c)
	{
		RaycastHit2D[] hits = new RaycastHit2D[10]; 		// Contains raycast hit info
		// Check for line of sight of the player
		numHits = Physics2D.LinecastNonAlloc (npc.transform.position, c.transform.position, hits, (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Player")));
		if (numHits == 1) {
			Debug.Log ("I see you!");
			return true;
		}
		else {
			Debug.Log ("Where you at?!");
			return false;
		}
	}
}
