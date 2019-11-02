using UnityEngine;
using System.Collections;

/*
 * Righty so, bro ...
 * There's cheese and there's dough
 * That's all you need to know 
 * This pizza dough, the pizza though...
 * Wow!	- Le bored and le bad.
 * 
 * Side note:
 * This here wee script's goal is to provide my summer personal project
 * with, during the early stages, a basic AI for the enemy characters
 * and later on with a more complex and well working decision making,
 * dream crushing and heart breaking monster. See you in-game ^^.
 * Date Created: 29/06/15 Current Version: 0.1 Last Update: 29/06/15
*/
public class SM4_R7 : MonoBehaviour
{
	// Publics
	public GameObject npcObject;			// The NPC itself
	public Transform[] patrolWaypts; 		// Waypoints to patrol
	public Collider2D aggroRange;			// How close do I start hitting things
	public Transform player;				// The player's current position
	public float movSpeed;					// The speed at which the NPC travels
	public float dmgPHit;					// The damage per hit yield of the NPC
	public float seekNoise;					// The amount of threat needed for the NPC to go into seek mode
	public float chaseSpeed;				// The speed at which the NPC chases after the player
	public float noiseRange;				// How far is the furthest an NPC can hear the player.

	// Privates 
	private bool aggro;						// State trigger
	private bool idle;						// State trigger
	private bool chase;						// State trigger
	private short index;					// Current waypoint
	public float noise;					// Threat amount

	// When initialised / after awake	
	void Start ()
	{
		SetUp ();
	}

	// Once/Frame
	void Update ()
	{
		Patrol ();
		NoiseCheck ();
		Seek ();
		Destroy ();
	}

	// Patrol between two or more waypoints
	private void Patrol ()
	{
		if (idle){
			npcObject.transform.position = Vector3.MoveTowards 
				(npcObject.transform.position, patrolWaypts[index].transform.position, movSpeed * Time.deltaTime);

			if (Vector3.Distance (npcObject.transform.position, patrolWaypts[index].transform.position) < 0.1f){
				if (index == patrolWaypts.Length - 1){
					index = 0;
				}
				else {
					index++;
				}
			}
		}
	}

	// Seek & destroy style movement
	private void Seek ()
	{
		if (!aggro && !chase){
			if (noise > seekNoise)
				chase = true;
			else 
				chase = false;
		}

		// The good stuff
		if (chase){
			/* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			 * Animations //// Seek Behaviour here!! 
			 * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			 */
			npcObject.transform.position = Vector3.MoveTowards 
				(npcObject.transform.position, player.transform.position, chaseSpeed * Time.deltaTime);
		}
	}

	// Seek's best friend
	private void Destroy (){
		if (aggro){
			// Aggro gets triggered by the DestroyCheck script, this is where behaviour specific code goes.
		}
	}

	// Sets up variables 
	private void SetUp ()
	{
		idle = true;
		index = 0;
		noise = 0;
		aggro = false;
		chase = false;
	}
	
	// Threat checker
	private void NoiseCheck ()
	{
		if (Vector3.Distance (npcObject.transform.position, player.transform.position) < noiseRange)
			noise += (noiseRange - Vector3.Distance (npcObject.transform.position, player.transform.position)) * Time.deltaTime;
		else 
			noise -= Time.deltaTime;
	}

	// For the use of LoS noise generation
	public void Noisy (float n)
	{
		Debug.Log ("Noisy");
		noise += n * Time.deltaTime;
	}

	// Getters/Setters
	public void SetIdle (bool c) { idle = c; }
	public bool GetIdle () { return idle; }

	public void SetChase (bool c) { chase = c; }
	public bool GetChase () { return chase; }

	public void SetAggro (bool c) { aggro = c; }
	public bool GetAggro () { return aggro; }
	
}
