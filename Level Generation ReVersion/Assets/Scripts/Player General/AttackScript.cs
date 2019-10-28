using UnityEngine;
using System.Collections;

/*
 * First attempt at reworking le black-o magic-o aka LE COMBO
 * Date Created: 29/06/15 Version: 0.1 Last Update: 29/06/15
*/
public class AttackScript : MonoBehaviour {

	// Publics
	[Tooltip("Combo attack range indicators - must be triggers and match number of attacks in combo")]
	public Collider2D[] colliders;
	public GameObject player;
	public float addedDelay;
	public float timeBtwAtks;
	public Animator anim;

	// Privates
	public float curDelay;
	private short index;
	public float temp;
	private bool comboEnded;
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown(KeyCode.Z)){
			if (temp <= 0 && !comboEnded){
				WomboCombo ();
				temp += timeBtwAtks;
			}
		}

		TickTock ();
	}

	// Delay
	private void TickTock ()
	{
		if (temp > 0) {
			temp -= Time.deltaTime;
		} else if (temp < 0) {
			temp = 0;
		}

		if (curDelay > 0) {
			curDelay -= Time.deltaTime;
		} else if (curDelay < 0) {
			curDelay = 0;
		}

		if (curDelay <= 0) {
			index = 0;
			curDelay = 0;
			Idle ();
			if (comboEnded) {
				comboEnded = false;
		
			}
		}
	}

	// Handles player attack mechanics
	private void WomboCombo ()
	{
		curDelay += addedDelay;
	
		if (curDelay > 0) {
			index++;
			Step (index);
		}
	}

	// Handles each part of the combo
	private void Step (short i)
	{
		// Step 1
		if (i == 1) {
			// First combo step stuff here // Animations to be added
			anim.SetBool("FirstBool", true);
			anim.SetBool("SecondBool", false);
			anim.SetBool("ThirdBool", false);
		} else if (i == 2) {
			// Second combo step stuff here // Animations to be added
			anim.SetBool("FirstBool", false);
			anim.SetBool("SecondBool", true);
			anim.SetBool("ThirdBool", false);
		} else if (i == 3) {
			// Third combo step stuff here // Animations to be added
			anim.SetBool("FirstBool", false);
			anim.SetBool("SecondBool", false);
			anim.SetBool("ThirdBool", true);
			comboEnded = true;
		}
	}

	// Returns the player to their original / idle state
	private void Idle ()
	{
		// Insert walking animation here
		anim.SetBool("FirstBool", false);
		anim.SetBool("SecondBool", false);
		anim.SetBool("ThirdBool", false);
		for (short i = 0; i < colliders.Length; i++) {
			colliders[i].enabled = false;
		}
		comboEnded = true;
	}
}
