using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/*
 * 	This script is the word of Antonio Rusev,
 * 	BSc (Hons) Computer Games Software Development
 * 	Glasgow Caledonian University - S1226502
 *  
 *  Process flow is as follows:
 *	1. Generate solution path.
 *	2. Generate each room along the solution path.
 *	3. Generate "filler" rooms (not on solution path). 
 *	4. Generate obstacles and enemies. 
*/

public class LevelMaster : MonoBehaviour {

	//Assets/Blocks/Prefabs used for generation
	public GameObject solBlock;				//Solid blocks
	public GameObject player;				//The player
	public GameObject exit;					//The goal
	public GameObject Walls;				//The wall's root
	public GameObject Level;				//The level's root
	public int emptyRandoms;				//Number of empty tiles in random rooms

	//General variables
	private List<int> roomLayout;			//Contains numbers representing the type of each room
	private List<int> roomLayoutRtrn;		//The return layout list
	private List<int> solutionPath;			//Contains the solution path - i.e. the room guaranteed to lead to the exit point
	private int[] solPath;					//An array used to convert the solutionPath ArrayList to an array
	private short numRooms;					//How many rooms have been generated so far

	//Room generation varaibles
	private Vector3 position;				//The starting position of each room's generation
	private int[] tileLayout;				//Layout of the tiles in a room
	private int tileCounter; 				//Used for "navigation"
	private int roomCounter;				//Used for even more "navigation"
	private int tileDepth; 					//MORE "navigation"
	private int entryPt;					//Starting point
	private int exitPt;						//Goal point

	//Path generation varaibles
	private	int iniDirectionRNG;			//Initial direction 
	private int solDepth;					//Depth counter for solution path
	private int curRoom;					//Current location of solution generator
	private int donRoom;					//Generation room counter
	private int goalRoom;					//Where the goal will be generated
	private int startRoom;					//Where the starting point will be generated
	private bool firstRun = true;			//First run?

	//Position flag temps
	bool posSet = false;
	bool ePosSet = false;

	
	//Triggers when the script is loaded 
	void Start () {
		roomCounter = 0;
		solutionPath = new List<int> ();
		position = new Vector3 (); 
		GeneratePath ();
		GenerateLevel (roomLayout);
		//GenerateRoo	m (1);
	}

	void Update () {
        EndLevel();
	}
	
	//Generates rooms - taking on	e parameter, representing the type of the room. 
	private void GenerateRoom (int rType) {

		//Determines the room's position
		//short rTemp = solutionPath [roomCounter];
		int depthCounter = (donRoom - (donRoom % 4)) / 4; 
		if (donRoom % 4 == 0) {
			position = new Vector3 (0, depthCounter * -8, 0);
		} else if (donRoom % 4 == 1 ) {
			position = new Vector3 (10, depthCounter * -8, 0);
		} else if (donRoom % 4 == 2) {
			position = new Vector3 (20, depthCounter * -8, 0);
		} else if (donRoom % 4 == 3) {
			position = new Vector3 (30, depthCounter * -8, 0);
		}
		//Selects an appropriate layout and generates the tiles
		int[] temp = layoutSelector (rType); 

		//Select the player's initial position
		if (donRoom + 1 == solutionPath [0]) {
			print ("Compared: " + (donRoom + 1) + " and " + solutionPath [0] + "And it was true");
			for (int i = temp.Length - 10; i --> 0;) {
				if (temp [i] == 0 && temp [i + 10] == 1) {	
					if (!posSet) {
						entryPt = temp [i];
						temp [i] = 3;
						posSet = true;
						print ("The position tile is: " + i);
					}
				}
			}
		}

		//Select the exit point
		if (donRoom + 1 == solutionPath [solutionPath.Count - 1]) {
			for (int i = temp.Length - 10; i --> 0;) {
				if (temp [i] == 0 && temp [i + 10] == 1) {	
					if (!ePosSet) {
						exitPt = temp [i];
						temp [i] = 4;
						ePosSet = true;
					}
				}
			}
		}

		for (int i = 0; i < temp.Length; i++) {
			GameObject tempTile;
			switch (temp[i]) {
				//An empty space
				case 0: 
					if (tileCounter >= 10) {
						tileCounter = 0;
						tileDepth++;
					}
					break; 
				//A solid block - simple as that
				case 1:
					if (tileCounter < 10) {
						tempTile = Instantiate(solBlock, new Vector3(position.x + tileCounter, position.y - tileDepth, 0),Quaternion.identity) as GameObject;
						tempTile.transform.parent = Level.transform;
					} else if (tileCounter >= 10) {
						tileCounter = 0; 
						tileDepth++;
						tempTile = Instantiate(solBlock, new Vector3(position.x + tileCounter, position.y - tileDepth, 0),Quaternion.identity) as GameObject;
						tempTile.transform.parent = Level.transform;
					}
					break;
				//May be solid block or empty space 
				case 2:
					int temp2 = Random.Range (0,2);
					if(temp2 == 0) {
						if (tileCounter >= 10) {
							tileCounter = 0;
							tileDepth++;
						}
					} else if(temp2 == 1) {
						if (tileCounter < 10) {
							tempTile = Instantiate(solBlock, new Vector3(position.x + tileCounter, position.y - tileDepth, 0),Quaternion.identity) as GameObject;
							tempTile.transform.parent = Level.transform;	
						} else if (tileCounter >= 10) {
							tileCounter = 0; 
							tileDepth++;
							tempTile = Instantiate(solBlock, new Vector3(position.x + tileCounter, position.y - tileDepth, 0),Quaternion.identity) as GameObject;
							tempTile.transform.parent = Level.transform;
						}
					}
					break;
				case 3:
					if (tileCounter < 10) {
					player.transform.position = new Vector3(position.x + tileCounter, position.y - tileDepth, 0);
					print (player.transform.position);
					} else if (tileCounter >= 10) {
						tileCounter = 0; 
						tileDepth++;
						print (position);
						player.transform.position = new Vector3(position.x + tileCounter, position.y - tileDepth, 0);
						print (player.transform.position);
					}
					break;
			case 4:
				if (tileCounter < 10) {
					exit.transform.position = new Vector3(position.x + tileCounter, position.y - tileDepth, 0);
				} else if (tileCounter >= 10) {
					tileCounter = 0; 
					tileDepth++;
					exit.transform.position = new Vector3(position.x + tileCounter, position.y - tileDepth, 0);
				}
				break;
				//Generate obstacles in the room
				case 5:

					break; 
			}
			tileCounter++;
		}
		//Which rooms in the solution path have been generated
		donRoom++;
		tileDepth = 0;
		tileCounter = 0;
	}

	private void GenerateBorder () {
		Vector3 tempPos = new Vector3 (-3, 3,0);
		Vector3 tempPos2 = new Vector3 (0, -32, 0);
		Vector3 tempPos3 = new Vector3 (40, 0, 0);
		int counter1 = 1;
		int counter2 = 0;
		int counter3 = 0;
		int counter4 = 0;
		int optCounter1 = 0;
		int optCounter2 = 0;
		int optCounter3 = 0;
		int optCounter4 = 0;

		for (int q = 0; q < 3; q++) {

			for (int i = 0; i < 45 - optCounter1; i++) {
				GameObject tempTile = Instantiate(solBlock, new Vector3(tempPos.x + counter1, tempPos.y, 0), Quaternion.identity) as GameObject;
                tempTile.transform.parent = Walls.transform;
				counter1++;
			}

			for (int n = 0; n < 38 - optCounter2; n++) {
				GameObject tempTile = Instantiate(solBlock, new Vector3(tempPos.x, tempPos.y - counter2, 0), Quaternion.identity) as GameObject;
                tempTile.transform.parent = Walls.transform;
				counter2++;
			}

			for (int m = 0; m < 43; m++) {
				GameObject tempTile = Instantiate(solBlock, new Vector3(tempPos2.x + counter3, tempPos2.y, 0), Quaternion.identity) as GameObject;
				tempTile.transform.parent = Walls.transform;
				counter3++;
			}

			for (int b = 0; b < 32; b++) {
				GameObject tempTile = Instantiate(solBlock, new Vector3(tempPos3.x, tempPos3.y - counter4, 0), Quaternion.identity) as GameObject;
                tempTile.transform.parent = Walls.transform;
				counter4++;
			}

			tempPos = new Vector3 (tempPos.x + 1, tempPos.y - 1, 0);
			tempPos2 = new Vector3 (tempPos2.x, tempPos2.y - 1, 0);
			tempPos3 = new Vector3 (tempPos3.x + 1, tempPos3.y, 0);
			counter1 = 1;
			counter2 = 0;
			counter3 = 0;
			counter4 = 0;
			optCounter1++;
			optCounter2++;
		}
	}

	//Generates the level in it's entirity 
	private void GenerateLevel (List<int> sPath) {
		GenerateBorder ();
		for (int i = 0; i < sPath.Count; i++) {
			GenerateRoom (sPath[i]);
		}
	}

	//Generates the solution path 
	private void GeneratePath () {
		solutionPath = new List<int> ();
		
		//Select the starting room
		startRoom = Random.Range (1, 5);
		curRoom = startRoom;
		solutionPath.Add (startRoom);
		
		//This method picks the direction the solution path will move in, and then calls a method that generates the path
		PickDirection ();
		GenerateLayout (solutionPath);
	}

	//---------------------PSEUDOCODE SECTION------------------------
	/*
	 * Compare i = 1 to a sorted solution path List i.e. :
	 * 			--------Code snippet--------
	 * 	for (int i = 1;i < 17;i++) {
	 * 		if (i != sSolutionPath[counter]) {
	 * 			roomLayout[i - 1].Add(0);
	 * 		} ...
	 * 	}
	 * 
	 * 	Note: i < 17, is due to the rooms always being 16 - look below for layout.
	 * 
	 * if i != solutionPath[counter*] - simply add a 0, suggesting a random room
	 * 
	 * if i == solutionPath[counter] :
	 * 
	 * Check what i's value is:
	 *  a) is it 1, 13, 4, 16 (corner rooms)
	 * 		- If yes, apply special conditions 
	 * 			- 1-4 do not consider the position  -4 (can't go into negatives)
	 * 			- 13-16 do not consider the position +4 (can't go beyond last room)
	 * 
	 * 		- If not, look at all relevant positions** which include:
	 * 			- 4 (the room above)
	 * 			+ 4 (the room below)
	 * 
	 *  b) based on whether rooms exist at considered positions add a number to the roomLayout<int> list
	 * 		- if no room is found at position -4 or +4, generate a room of type 1 (populate list)
	 * 		- if a room is found only at position +4, generate a room of type 2	(populate list)
	 * 		- if a room is found only at position -4, generate a room of type 3 (populate list)
	 * 		- if a room is found at both -4 and +4 (rare), generate a room of type 4 (populate list)
	 * 
	 * Test cases &ß results expectations:
	 * 
	 * solutionPath: 4, 3, 7, 6, 10, 14, 13
	 * sSolutionPath: 3, 4, 6, 7, 10, 13, 14
	 * roomLayout: 0, 0, 2, 1, 0, 2, 3, 0, 0, 4, 0, 0, 1, 3, 0, 0
	 * 
	 * solutionPath: 1, 2, 3, 4, 8, 7, 6, 5, 9, 10, 11, 12, 16, 15, 14, 13
	 * sSolutionPath: 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16
	 * roomLayout: 1, 1, 1, 2, 2, 1, 1, 3, 3, 1, 1, 2, 1, 1, 1, 3
	 * 
	 * solutionPath: 4, 8, 12, 16
	 * sSolutionPath: 4, 8, 12 ,16
	 * roomLayout: 0, 0, 0, 2, 0, 0, 0, 4, 0, 0, 0, 4, 0, 0, 0, 3
	 * 
	 * Room Layout overview: 
	 * 
	 * 	1  2  3  4
	 *  5  6  7  8
	 *  9 10 11 12
	 * 13 14 15 16
	 * 
	 * 
	 * *Note: Counter used as a seprate position tracker, increments on each equivalent value.
	 * **Note: We do not need to consider rooms adjecent on left and right due to all rooms being of type 1 by default.
	 * 
	 * Room types legend: 1 - left/right, 2 - left/right/down, 3 - left/right/up, 4 - left/right/up/down, 0 - random
	*/
	//----------------------------------------------------------------

	//Generes the level's layout based on the solution path
	private void GenerateLayout (List<int> path) {
	
		//Setting lists up
		roomLayout = new List<int> ();
		//roomLayoutRtrn = new List<int> ();
		List<int> sSolutionPath = new List<int> ();
		sSolutionPath.AddRange (solutionPath);
		sSolutionPath.Sort ();
		List<int> tSolutionPath = new List<int> ();
		List<LayoutRoom> indexList = new List<LayoutRoom> ();

		bool firstRun = false;
		int counter = 0;
		/*
		int fakeCounter = 0;

		//First the list is populated with ints matching the room number
		for (int i = 1; i < 17; i++) {
			if (i == sSolutionPath[counter]) {
				roomLayout.Add(i);
				if (sSolutionPath.Count - 1 > counter) {
					counter++;
				}
			} else {
				roomLayout.Add(0);
			}
		}
		*/
			
		//Create a room type solution path equivalent
		for (int i = 0; i < solutionPath.Count; i++) {
			if (!firstRun) {
				if (solutionPath[i + 1] == (solutionPath[i] + 4)) {
					tSolutionPath.Add (2);
					firstRun = true;
				} else {
					print ("Compared: " + solutionPath [i + 1] + " and " + (solutionPath [i] + 4) + " and it was true");
					tSolutionPath.Add (1);
					firstRun = true;
				}
			} else if (i == solutionPath.Count - 1) {
				if (solutionPath[i - 1] == (solutionPath[i] - 4)) {
					tSolutionPath.Add (3);
				} else {
					print ("Compared: " + solutionPath [i - 1] + " and " + (solutionPath [i] - 4) + " and it was false");
					tSolutionPath.Add (1);
				}
			} else {
				if (solutionPath[i + 1] == (solutionPath[i] + 4) && solutionPath[i - 1] == (solutionPath[i] - 4)) {
					tSolutionPath.Add (4);
				} else if (solutionPath[i + 1] == (solutionPath[i] + 4)) {
					tSolutionPath.Add (2);
				} else if (solutionPath[i - 1] == (solutionPath[i] - 4)) {
					tSolutionPath.Add (3);
				} else {
					tSolutionPath.Add (1);
				}
			} 
		}

		for (int i = 0; i < solutionPath.Count; i++) {
			LayoutRoom temp = new LayoutRoom (); 
			temp.SetIndex(solutionPath[i]);
			temp.SetType(tSolutionPath[i]);
			indexList.Add (temp);
		}

		indexList.Sort (delegate(LayoutRoom x, LayoutRoom y) {
			return x.index.CompareTo (y.index);
		});

		for (int i = 0; i < 16; i++) {
			roomLayout.Add (0);
		}

		for (int i = 1; i < roomLayout.Count + 1; i++) {
			if (counter < indexList.Count) {
				if (i == indexList[counter].GetIndex ()) {
					roomLayout[i - 1] = indexList[counter].GetType();
					counter++;
				}
			}
		}


		/*
		//Use the ints to determine the type of room to be generated
		for (int i = 0; i < roomLayout.Count; i++) {
			int above = roomLayout[i] - 4;
			int below = roomLayout[i] + 4;
			int ahead = fakeCounter + 1;
			int behind = fakeCounter - 1;

			if (solutionPath.Count > fakeCounter && roomLayout[i] != 0) { 
				print("Current tile is: " + roomLayout[i] + " Current above is: " + above + " Current below is: " + below + " Current room is: " + solutionPath[fakeCounter]);
			}


			if (roomLayout[i] != 0) {
				if (roomLayout[i] <= 4) {
					if (roomLayout[i + 4] != 0) {
						roomLayoutRtrn[i] = 2;
					} else {
						roomLayoutRtrn[i] = 1;
					}
				} else if (roomLayout[i] >= 13) {
					if (roomLayout[i - 4] != 0) {
						roomLayoutRtrn[i] = 3;
					} else {
						roomLayoutRtrn[i] = 1;
					}
				}
				//Multi-path version
				/* else {
					if(roomLayout[i - 4] == 0 && roomLayout[i + 4] == 0) {
						roomLayout[i] = 1;
					} else if (roomLayout[i - 4] != 0 && roomLayout[i + 4] == 0) {
						roomLayout[i] = 3;
					} else if (roomLayout[i - 4] == 0 && roomLayout[i + 4] != 0) {
						roomLayout[i] = 2;
					} else if (roomLayout[i - 4] != 0 && roomLayout[i + 4] != 0) {
						roomLayout[i] = 4;
					}
				} 

				//My way or the highway
				  else {
					if (roomLayout[i - 4] == 0 && roomLayout[i + 4] == 0) {
						roomLayoutRtrn[i] = 1;
						print ("Both = 0");
					} else if (roomLayout[i - 4] != 0 && roomLayout[i + 4] == 0) {
						if (solutionPath[behind] == above) { 
							print ("Compared: " + solutionPath[behind] + " and " + above + " and it was true");
							roomLayoutRtrn[i] = 3;
						} else {
							print ("Compared: " + solutionPath[behind] + " and " + above + " and it was false");
							roomLayoutRtrn[i] = 1;
						}
					} else if (roomLayout[i - 4] == 0 && roomLayout[i + 4] != 0) {
						if (solutionPath[ahead] == below) {
							print ("Compared: " + solutionPath[ahead] + " and " + below + " and it was true");
							roomLayoutRtrn[i] = 2;
						} else {
							print ("Compared: " + solutionPath[ahead] + " and " + below + " and it was false");
							roomLayoutRtrn[i] = 1;
						}
					} else if (roomLayout[i - 4] != 0 && roomLayout[i + 4] != 0) {
						if (solutionPath[ahead] == below &&
						    solutionPath[behind] == above) {
							roomLayoutRtrn[i] = 4;
						} else if (solutionPath[ahead] == below) {
							roomLayoutRtrn[i] = 2;
						} else if (solutionPath[behind] == above) {
							roomLayoutRtrn[i] = 3;
						} else {
							roomLayoutRtrn[i] = 1;
						}
					}
				} 
				fakeCounter++;
			} 
			else {
				roomLayoutRtrn[i] = 0;
			}
		}*/
	}

	//Picks a direction in which the solution path to head next
	private void PickDirection () {
		//Used for randomising path
		int temp1 = Random.Range (1, 6);

		//Tweeking these if statements will increase/decrease the odds of picking a direction

		//Heading left
		if (temp1 == 1 || temp1 == 2) {
			PathProgress (-1); 
		}
		//Heading Right
		else if (temp1 == 3 || temp1 == 4) {
			PathProgress (1); 
		} 
		//Heading downwards
		else {
			if (solDepth < 4) {
				solDepth++;
				int temp2 = curRoom + (solDepth * 4);
				solutionPath.Add (temp2);
				PickDirection ();
			} else {
				goalRoom = curRoom + 12;
				print ("final room reached");
			}
		}
	}

	//This is where the path gets generated, increment determines the direction the path is moving in -1 = Left, 1 = Right
	private void PathProgress (int increment) {
		while (goalRoom == 0) {
			//In cases of 1 and 2 we move horizontally, otherwise we move downwards
			int temp1 = Random.Range (1, 4);
			//If we're moving horizontally (decided by PickDirection) - check walls and move accordingly.
 			if (temp1 < 3) {
				if (increment == -1) {
					if (!CheckLeftWall ()) {
						curRoom += increment; 
					} else {
						solDepth++;
						increment = -increment;
					}
				} else if (increment == 1) {
					if (!CheckRightWall ()) {
						curRoom += increment;
					} else {
						solDepth++;
						increment = -increment;
					}
				}
			} else {
				solDepth++;
			}

			//Once the solution path has moved on - we determine whether this is the final room, and if not we continue on
			if (solDepth < 4) {
				int temp2 = curRoom + (solDepth * 4);
				solutionPath.Add (temp2);
			}
			if (solDepth == 4) {
				goalRoom = curRoom + 12;
				print ("Goal Reached");
			}
		}
	}

	private bool CheckLeftWall () {
		if (curRoom == 1) {
			return true;
		} else {
			return false;
		}
	}

	private bool CheckRightWall () {
		if (curRoom == 4) {
			return true;	
		} else {
			return false;
		}
	}

	//
	private void GenerateObstacles (short dif) {
		
		//Easy mode obstacles
		if (dif == 1) {
			
		}
		
		//Normal mode obstacles
		if (dif == 2) {
			
		}
		
		//Hard mode obstacles
		if (dif == 3) {
			
		}
	}
	
    private void EndLevel()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.LoadLevel(0);
        }
    }

    private int[] layoutSelector (int rType) {
        string layout1;
        string layout2;
        string layout3;
        string layout4;
		string temp1;
		string temp2;
		int rand;



		switch (rType) {
			case 0:
				int[] layo0 = new int[80];
				int strLoc = Random.Range (35, 45);
				int randomiser;
				layo0[strLoc] = 0;

				for (int i = 0; i < 80; i++) {
					layo0[i] = 1;
				}
				for (int i = 0; i < emptyRandoms; i++) {
					randomiser = Random.Range (1, 5);
					int randomiser2 = Random.Range(0, 80); 
					if (randomiser == 1) {
						if (strLoc < 79) {
							if (layo0[strLoc + 1] == 1) {
								layo0[strLoc + 1] = 0;
								strLoc++;
							}
						} else if (strLoc > 0) {
							if (layo0[strLoc - 1] == 1) {
								layo0[strLoc - 1] = 0;
								strLoc--;
							}
						} else if (strLoc > 10) { 
							if (layo0[strLoc - 10] == 1) {
								layo0[strLoc - 10] = 0;
								strLoc -= 10;
							}
						} else if (strLoc < 70) { 
							if (layo0[strLoc + 10] == 1) {
								layo0[strLoc + 10] = 0;
								strLoc += 10;
							}	
						} else {
							while (strLoc != randomiser2) {
								randomiser2 = Random.Range (0,80);
								if (layo0[randomiser2] == 1) {
									layo0[randomiser2] = 0;
									strLoc = randomiser2;
								}
							}
						}
					} else if (randomiser == 2) {
						if (strLoc > 0) {
							if (layo0[strLoc - 1] == 1) {
								layo0[strLoc - 1] = 0;
								strLoc--;
							}
						} else if (strLoc < 79) {
							if (layo0[strLoc + 1] == 1) {
								layo0[strLoc + 1] = 0;
								strLoc++;
							}
						} else if (strLoc > 10) { 
							if (layo0[strLoc - 10] == 1) {
								layo0[strLoc - 10] = 0;
								strLoc -= 10;
							}
						} else if (strLoc < 70) { 
							if (layo0[strLoc + 10] == 1) {
								layo0[strLoc + 10] = 0;
								strLoc += 10;
							}
						} else {
							while (strLoc != randomiser2) {
								randomiser2 = Random.Range (0,80);
								if (layo0[randomiser2] == 1) {
									layo0[randomiser2] = 0;
									strLoc = randomiser2;
								}
							}
						}
					} else if (randomiser == 3) {
						if (strLoc < 70) {
							if (layo0[strLoc + 10] == 1) {
								layo0[strLoc + 10] = 0;
								strLoc += 10;
							}
						} else if (strLoc > 0) {
							if (layo0[strLoc - 1] == 1) {
								layo0[strLoc - 1] = 0;
								strLoc--;
							}
						} else if (strLoc > 10) { 
							if (layo0[strLoc - 10] == 1) {
								layo0[strLoc - 10] = 0;
								strLoc -= 10;
							}
						} else if (strLoc < 79) {
							if (layo0[strLoc + 1] == 1) {
								layo0[strLoc + 1] = 0;
								strLoc++;
							}
						} else {
							while (strLoc != randomiser2) {
								randomiser2 = Random.Range (0,80);
								if (layo0[randomiser2] == 1) {
									layo0[randomiser2] = 0;
									strLoc = randomiser2;
								}
							}
						}
					} else if (randomiser == 4) {
						if (strLoc > 10) {
							if (layo0[strLoc - 10] == 1) {
								layo0[strLoc - 10] = 0;
								strLoc -= 10;
							}
						} else if (strLoc < 79) { 
							if (layo0[strLoc + 1] == 1) {
								layo0[strLoc + 1] = 0;
								strLoc++;
							}
						} else if (strLoc > 0)	{
							if (layo0[strLoc - 1] == 1) {
								layo0[strLoc - 1] = 0;
								strLoc--;
							}	
						} else if (strLoc < 70) {
							if (layo0[strLoc + 10] == 1) {
								layo0[strLoc + 10] = 0;
								strLoc += 10;
							}
						} else {
							while (strLoc != randomiser2) {
								randomiser2 = Random.Range (0,80);
								if (layo0[randomiser2] == 1) {
									layo0[randomiser2] = 0;
									strLoc = randomiser2;
								}
							}
						}
					}
				}
				return layo0;
			case 1: 
				temp1 = 
					"1;1;1;2;2;1;1;1;1;1;" +
					"1;0;0;0;0;0;2;2;1;2;" +
					"0;0;0;0;2;0;0;2;2;2;" +
					"2;2;2;0;0;0;0;0;2;2;" +
					"0;0;0;0;0;0;0;0;0;0;" +
					"0;0;0;2;2;2;0;0;0;1;" +
					"0;0;0;0;0;0;0;1;1;1;" +
					"1;1;1;2;2;2;1;1;1;1";

                temp2 = 
                    "1;1;1;1;1;2;2;2;2;2;" +
                    "2;1;0;0;0;0;0;0;0;2;" +
                    "2;0;0;0;0;0;0;0;0;2;" +
                    "0;0;2;2;0;2;0;0;0;0;" +
                    "0;0;0;0;0;2;2;2;0;0;" +
                    "1;1;0;0;1;1;0;2;2;1;" +
                    "1;1;1;0;0;0;0;0;1;1;" +
                    "1;1;1;1;1;2;2;1;1;1";
                
                rand = Random.Range (1,3);
                if (rand == 1) {
                    layout1 = temp1;
                } else {
                    layout1 = temp2;
                }

				int[] layo1 = layout1.Split (';').Select(c => int.Parse (c.ToString ())).ToArray();
				return (int[])layo1;
			case 2: 
				temp1 = 
					"1;1;1;2;2;2;2;2;1;1;" +
					"0;0;0;0;0;1;0;0;0;0;" +
					"0;0;0;0;0;1;1;1;0;0;" +
					"0;0;0;0;0;0;0;0;0;0;" +
					"0;0;2;2;0;0;0;0;1;1;" +
					"1;1;0;0;0;1;1;0;0;2;" +
					"2;2;0;0;1;1;1;1;0;0;" +
					"0;0;0;0;0;0;0;0;0;0";

                temp2 = 
                    "1;1;1;1;1;1;1;1;1;1;" +
                    "1;1;2;2;2;2;2;2;2;1;" +
                    "2;0;0;0;0;0;0;0;0;0;" +
                    "2;0;0;0;0;0;0;2;2;2;" +
                    "0;0;0;2;2;2;0;0;0;0;" +
                    "0;0;0;0;0;0;0;0;0;0;" +
                    "0;0;0;0;0;0;0;2;1;0;" +
                    "0;1;1;1;0;0;2;2;1;1";

                rand = Random.Range(1,3);
                if (rand == 1) {
                    layout2 = temp1;
                } else {
                    layout2 = temp2;
                }
    

				int[] layo2 = layout2.Split (';').Select(c => int.Parse (c.ToString ())).ToArray();
				return (int[])layo2;
			case 3: 
				temp1 = 
					"1;1;0;0;0;0;0;0;1;1;" +
					"1;0;0;1;1;0;0;1;1;1;" +
					"0;0;0;0;0;0;0;0;0;0;" +
					"0;2;0;2;2;1;0;0;1;1;" +
					"0;0;1;1;1;1;1;0;0;0;" +
					"0;0;0;0;0;0;0;0;1;1;" +
					"0;1;1;1;0;0;0;1;1;1;" +
					"1;1;1;1;1;1;1;1;1;1";

                temp2 =
                    "2;2;0;0;0;0;0;0;0;0;" +
                    "0;0;0;1;1;1;0;0;0;0;" +
                    "2;0;0;0;0;2;2;0;0;0;" +
                    "1;1;0;0;0;0;0;0;1;1;" +
                    "0;0;0;1;1;1;1;0;0;0;" +
                    "0;0;0;0;2;2;0;0;0;1;" +
                    "0;0;0;0;0;0;0;0;1;1;" +
                    "1;1;1;1;2;2;1;1;1;1";

                rand = Random.Range(1,3);
                if (rand == 1) {
                    layout3 = temp1;
                } else {
                    layout3 = temp2;
                }

				int[] layo3 = layout3.Split (';').Select(c => int.Parse (c.ToString ())).ToArray();
				return (int[])layo3;
			case 4:
				temp1 = 
					"0;0;0;0;0;0;1;1;1;1;" +
					"0;1;1;1;1;0;0;0;0;0;" +
					"0;0;0;0;0;0;0;2;2;0;" +
					"0;0;0;0;0;1;1;1;1;0;" +
					"0;0;2;1;0;0;0;0;0;0;" +
					"1;0;0;0;0;0;0;0;0;1;" +
					"1;1;0;0;0;0;0;0;2;1;" +
					"1;1;1;0;0;0;2;2;2;1";

                temp2 =
                    "0;0;0;0;0;0;0;0;0;0;" +
                    "2;2;2;0;0;1;1;1;0;0;" +
                    "1;0;0;0;1;2;0;0;0;2;" +
                    "1;1;1;0;0;0;0;2;2;2;" +
                    "0;0;0;1;1;1;0;0;0;0;" +
                    "0;0;0;0;0;0;0;0;0;1;" + 
                    "1;1;1;0;0;0;2;1;1;1;" +
                    "0;0;0;0;0;0;0;0;0;0";

                rand = Random.Range(1,3);
                if (rand == 1) {
                    layout4 = temp1;
                } else {
                    layout4 = temp2;
                }

				int[] layo4 = layout4.Split (';').Select(c => int.Parse (c.ToString ())).ToArray();  
				return (int[])layo4;
			default: 
                print("Invalid room type found!");
				return null;
		}
	}

	private class LayoutRoom {
		public int index;
	 	public int type;

		public void SetIndex(int index) {
			this.index = index;
		}

		public int GetIndex() {
			return index;
		}

		public void SetType(int type) {
			this.type = type;
		}

		public int GetType() {
			return type;
		}

	}

}
