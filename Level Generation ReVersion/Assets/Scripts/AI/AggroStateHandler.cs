using UnityEngine;
using System.Collections;

/*
 *  In order to keep everything easy to read
 *  and manage I've split the AI system's 
 *  aggressive state into it's own class
 */
public class AggroStateHandler : MonoBehaviour {

	// Publics
	public Animator anim;
	public Collider2D innerRim;
	public bool killKill;

	// Privates
	private bool[] animate;
	private short temp;

	// Works just like void Start (), with the addition of coroutines
	// which I, by the way, really like for animation and control.
	private IEnumerator Start ()
	{
		yield return StartCoroutine (CoUpdate ());
	}

	private IEnumerator CoUpdate ()
	{
		// Set killKill to false
		killKill = false;

		// The game loop happens inside here
		while (true)
		{
			// Break the loop if escape is pressed
			if (Input.GetKeyDown(KeyCode.Escape)) { break; }

			if (GameObject.FindGameObjectWithTag("NPC").GetComponent<SM4_R7>().GetAggro()){
				yield return StartCoroutine (Animate (RunCheck ()));	 
			}
		}
		yield return null;
	}

	// Check which animation is running just now if any and return it
	private short RunCheck ()
	{
		bool check = false;

		for (short i = 0; i < animate.Length; i++){
			if (animate[i] == true){
				temp = i;
				check = true;
			}
		}

		// Return which animation is running
		if (!check) {
			Debug.Log ("Return dummy value as no animation running");
			return 99;
		} else {
			return temp;
		}
	}

	// Play the animation passed as a parameter (it's index)
	private IEnumerator Animate (short q)
	{
		/*
		 *  Animation code goes in here depending on the NPC's design
		 *  which at this point has not been decided on and will most
		 *  likley feature a wide variety of attack patterns and enemies.
		 *  The goal of the approach taken in this script is to turn this
		 *  into an enemy aggressive state managing system, which does not
		 *  require any extra attachments and allows for the reuse of code
		 *  instead of having a personalised script for each individual foe.
		 */

		// Check for dummy value
		if (q == 99) {
			Debug.Log ("No animation running at the time");
		}

		// This could be melee enemy #1
		else if (q == 0){
			// play out pattern for melee enemy #1
		}

		// This could be ranged enemy #1
		else if (q == 1){
			// play out pattern for ranged enemy #1
		}
		// etc. 
		/* An aditional application of this method is 
		 * that it also allows for an enemy to have multiple
		 * patterns and can greatly improve the challenge the 
		 * game can toss at the player. Easy games aren't fun.
		 */
		yield return null;
	}

	// A setter for the array used to determine current animation to play
	// !Note: for the use of other scripts (it's a setter after all)
	public void SetAnimate(short q)
	{
		// Reset the array
		for (short i = 0; i < animate.Length; i++) {
			animate [i] = false;
		}

		if (q == 99) {
			Debug.Log ("No animation playing keyword passed.");
		} else if (q >= 0) {
			animate [q] = true;
		} else if (q < 0) {
			Debug.Log ("Cannot access negative index variable in an array.");
		}
	}
}
