using UnityEngine;
using System.Collections;

/*
 * Just the basic data storing script
 * Date Created: 24/06/15 Version: 0.1 Last Update: 24/06/15
*/

public class _GameData : MonoBehaviour {

	//Publics
	public bool doubleJumpOn; 			//Has the player unlocked double jumping
	public bool runOn;					//Has the player unlocked running

	//To be used in a later version
	public float score;					//Player's score
	public float curLevel;				//Last completed level (which levels are unlocked)
	public short achieved;				//Last achievement completed (which ahievements are unlocked)
	public short livesLeft;				//How many lives the player has left

	//Privates
	public bool hasReset = false;

	//Initialise 
	void Awake()
	{
		//If the application has not reset it's game data ... do it.
		if (!hasReset) {
			Reset ();
		}

		DontDestroyOnLoad (this);
	}

	//Reset the game's saved data
	private void Reset()
	{
		doubleJumpOn = true;
		runOn = true;
		score = 0;
		curLevel = 0;
		achieved = 0;
		livesLeft = 3;

		hasReset = true;
	}
}
