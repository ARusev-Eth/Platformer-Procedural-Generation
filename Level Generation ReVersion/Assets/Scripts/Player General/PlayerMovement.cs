using UnityEngine;
using System.Collections;

/*
 * Just the basic 1,2 movement script for the player character
 * Date Created: 24/06/15 Version: 0.1b Last Update: 29/06/15
*/

public class PlayerMovement : MonoBehaviour {
	
	// Publics
	public Transform[] groundChecks;     // Transforms used for the ground check
	public GameObject player;			 // The player object
	public float Acceleration;			 // How fast does the player reach maximum Velocity
	public float jumpForce;              // How high the player can jump
	public float walkVelocity;			 // The player's maximum walking velocity
	public float runVelocity;			 // The player's maximum running velocity
	public Transform[] wallChecks;		 //Prevent wall sticking
	public bool wallHit;				 //Is a wall atm hit

	// Privates                    
	private float maxJumpHeight;         // How high can the player jump;
	public bool onGround;				 // Bool used for ground check / jumping
	private bool isLeft;			 	 // True of the player is looking left / used for direction changes
	private bool doubleJumpOn;           // Is double jumping unlocked
	public bool doubleJump;              // Can the player double jump just now
	private bool jumping1;				 // Has the player jumped
	private bool jumping2; 				 // Has the player double jumped
	public bool runOn;					 // Is running unlocked
	private float tempVel;				 // Temporary velocity value
	private bool deccelerate; 			 // Used for decellaration (running)
	private bool accelerate;			 // Used for acceleration (running)
	private float horInput;				 // Game Controls related

	// Use this for initialization
	private void Start () 
	{
		SetUp ();
		//UpdateData ();
		ErrorCheck ();
	}
	
	// Update is called once per frame
	private void Update () 
	{
		Inputs ();
		Run ();
	}
	
	// Physics update method
	private void FixedUpdate ()
	{
		Movement ();
		GroundCheck ();
		Jump ();
	}

	// Tracks inputs made by the player
	private void Inputs ()
	{
		// Jump when the player presses the up arrow
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if (onGround)
				jumping1 = true;
			
			
			if (doubleJumpOn) {
				if (!onGround && !doubleJump) {
					jumping2 = true;
					doubleJump = true;
				}
			}
		}
		
		// Run while the player is holding C down
		if (Input.GetKeyDown (KeyCode.C)) {
			if (runOn){
				accelerate = true;
			}
		}

		if (Input.GetKeyUp (KeyCode.C)) {
			deccelerate = true;
		}
	}

	// Yep... it's about running. Good guess.
	private void Run ()
	{
		if (runOn) {
			if (accelerate) {
				if (!onGround) {
				}
				
				if (walkVelocity < runVelocity) {
					walkVelocity += 0.5f;
				}
			}
			
			if (walkVelocity == runVelocity) {
				accelerate = false;
			}

			if (deccelerate) {
				if (!onGround) {
				} else if (walkVelocity > tempVel) {
					walkVelocity -= 0.5f;
				}
			}
			
			if (walkVelocity == tempVel) {
				deccelerate = false;
			}
		}
	}

	// Controls player movement
	private void Movement()
	{
		horInput = Input.GetAxis ("Horizontal");

		if (!wallHit && !onGround || wallHit && onGround || !wallHit && onGround) {
			if (horInput == 0) {
				player.GetComponent<Rigidbody2D>().velocity = new Vector3 (0.0f, player.GetComponent<Rigidbody2D>().velocity.y, 0.0f);
			}

			if (horInput * player.GetComponent<Rigidbody2D>().velocity.x < walkVelocity) {
				player.GetComponent<Rigidbody2D>().AddForce (Vector2.right * horInput * Acceleration);
			}

			if (Mathf.Abs (player.GetComponent<Rigidbody2D>().velocity.x) >= walkVelocity) {
				player.GetComponent<Rigidbody2D>().velocity = new Vector2 (Mathf.Sign (GetComponent<Rigidbody2D>().velocity.x) * walkVelocity,
				                                    GetComponent<Rigidbody2D>().velocity.y);
			}

			if (horInput < 0 && !isLeft) {
				ChangeDirection ();
			}
			
			if (horInput > 0 && isLeft) {
				ChangeDirection ();
			}
		}
	}
	
	// Controls jumping
	private void Jump()
	{
		if (jumping1) {
			player.GetComponent<Rigidbody2D>().AddForce (new Vector2 (0.0f, jumpForce));
			jumping1 = false;
		}

		if (jumping2) {
			player.GetComponent<Rigidbody2D>().velocity = new Vector3 (player.GetComponent<Rigidbody2D>().velocity.x, 0.0f, 0.0f);
			player.GetComponent<Rigidbody2D>().AddForce (new Vector2 (0.0f, jumpForce));
			jumping2 = false;
		}
	}
	
	// Checks if the player is on the ground
	private void GroundCheck ()
	{
		if (Physics2D.Linecast (player.transform.position, groundChecks [0].transform.position, 1 << LayerMask.NameToLayer ("Ground")) || 
			Physics2D.Linecast (player.transform.position, groundChecks [1].transform.position, 1 << LayerMask.NameToLayer ("Ground")) ||
			Physics2D.Linecast (player.transform.position, groundChecks [2].transform.position, 1 << LayerMask.NameToLayer ("Ground")) ||
		    Physics2D.Linecast (player.transform.position, groundChecks [3].transform.position, 1 << LayerMask.NameToLayer ("Ground")) ||
		    Physics2D.Linecast (player.transform.position, groundChecks [4].transform.position, 1 << LayerMask.NameToLayer ("Ground")) ||
		    Physics2D.Linecast (player.transform.position, groundChecks [5].transform.position, 1 << LayerMask.NameToLayer ("Ground"))) {
			onGround = true;
			doubleJump = false;
		} else { 
			onGround = false;
		}
	}

	/*private void WallCheck () {
		if (Physics2D.Linecast (player.transform.position, wallChecks [0].transform.position, 1 << LayerMask.NameToLayer ("Ground")) || 
			Physics2D.Linecast (player.transform.position, wallChecks [1].transform.position, 1 << LayerMask.NameToLayer ("Ground"))) {
			wallHit = true;
		} else {
			wallHit = false;
		}
	}*/

	// Setting up all the bools
	private void SetUp ()
	{
		onGround = false;
		isLeft = false;
		onGround = false;
		doubleJump = false;
		doubleJumpOn = true;
		jumping1 = false;
		jumping2 = false;
		deccelerate = false;
		accelerate = false;
		runOn = true;

		tempVel = walkVelocity;
	}
	
	// Make sure everything's assigned
	private void ErrorCheck ()
	{
		if (!player) {
			Debug.Log ("Player object not assigned!");
		}

		for (int i = 0; i < groundChecks.Length; i++) {
			if (!groundChecks [i]) {
				Debug.Log ("Ground check index: " + i + " not assigned!");
			}
		}
	}

	// Grab relevant data from the _GameData script
	private void UpdateData ()
	{
		_GameData gData = GameObject.FindGameObjectWithTag ("GameData").GetComponent<_GameData> ();
		doubleJumpOn = gData.doubleJumpOn;
		runOn = gData.runOn;
	}
	
	// Change the direction the player is facing
	private void ChangeDirection ()
	{
		isLeft = !isLeft;
		
		Vector3 scale = player.transform.localScale;
		scale.x *= -1;
		player.transform.localScale = scale;
	}

	public short GetDir ()
	{
		if (isLeft) {
			return 0;
		} else {
			return 1;
		}
	}
}
