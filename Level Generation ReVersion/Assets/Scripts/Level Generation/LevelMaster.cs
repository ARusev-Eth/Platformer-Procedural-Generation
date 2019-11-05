using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/*
 * 	This script is writen by Antonio Rusev,
 *  
 *  Process flow is as follows:
 *	1. Generate solution path.
 *	2. Generate each room along the solution path.
 *	3. Generate "filler" rooms (not on solution path). - Experimental
 *	4. Generate obstacles and enemies. - Stretch Goals
*/

public class LevelMaster : MonoBehaviour {

	//Assets/Blocks/Prefabs used for generation
	public GameObject solBlock;				//Solid blocks
	public GameObject player;				
	public GameObject exit;					
	public GameObject walls;				//The wall's root
    public GameObject backgroundWalls;      
    public GameObject litBackgroundWalls;   
	public GameObject level;				//The level's root
    public Camera mainCamera;               
    public Camera subCamera;                

	//General variables
	private List<int> roomLayout;			//Contains numbers representing the type of each room
	private List<int> roomLayoutRtrn;		//The return layout list
	private List<int> solutionPath;			//Contains the solution path - i.e. the room guaranteed to lead to the exit point
	private int[] solPath;					//An array used to convert the solutionPath ArrayList to an array
	private short numRooms;					//How many rooms have been generated so far
    private Camera curActiveCam;            //Currently active camera
    public int emptyRandoms;                //Experimental generation of non-main path rooms

    //Room generation varaibles
    private Vector3 position;				//The starting position of each room's generation
	private int[] tileLayout;				//Layout of the tiles in a room
	private int tileCounter; 				//Used for "navigation"
	private int tileDepth; 					//"navigation"
	private int entryPt;					
	private int exitPt;						

	//Path generation varaibles
	private	int iniDirectionRNG;			//Initial direction 
	private int solDepth;					//Depth counter for solution path
	private int curRoom;					//Current location of solution generator
	private int donRoom;					//Generation room counter
	private int goalRoom;					
	private int startRoom;					
	private bool firstRun = true;			

	//Position flag temps
	private bool posSet = false;
	private bool ePosSet = false;

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

        public int GetRType() {
            return type;
        }
    }

    //Triggers when the script is loaded 
    void Start () {
        emptyRandoms = StaticDataHolder.randomBlocks;
		solutionPath = new List<int> ();
		position = new Vector3 (); 
		GeneratePath ();
		GenerateLevel (roomLayout);
        mainCamera.enabled = false;
        curActiveCam = subCamera;
        subCamera.orthographicSize = 76 * Screen.height / Screen.width * 0.5f;
    }
    
    //Updates once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            CameraSwap();
        }
        EndLevel();
	}

    //Generates the level in it's entirity 
    private void GenerateLevel(List<int> sPath) {
        GenerateBorder();
        for (int i = 0; i < sPath.Count; i++) {
            GenerateRoom(sPath[i]);
        }
    }

    private void CameraSwap() {
        if (mainCamera.isActiveAndEnabled) {
            curActiveCam = subCamera;
            mainCamera.enabled = false;
            subCamera.enabled = true;  
        } else if (subCamera.isActiveAndEnabled) {
            curActiveCam = mainCamera;
            mainCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, mainCamera.transform.position.z);
            mainCamera.enabled = true;
            subCamera.enabled = false;
        }
    }

    private void EndLevel() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.LoadLevel(0);
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
            Application.LoadLevel(1);
        }

        if (player.GetComponent<Collider2D>().IsTouching(exit.GetComponent<Collider2D>())) {
            Application.LoadLevel(0);
        }
    }

    //Generates rooms - taking on	e parameter, representing the type of the room. 
    private void GenerateRoom (int rType) {

		//Determines the room's position
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

        int tempX = 0;
        for (int i = 0; i < temp.Length; i++) {
			GameObject tempTile;
			switch (temp[i]) {
				//Background
				case 0: 
                    if(tileCounter < 10) {
                        if(tempX < 10) {
                            tempTile = Instantiate(backgroundWalls, new Vector3(position.x + tileCounter, position.y - tileDepth, 0), Quaternion.identity) as GameObject;
                            tempTile.transform.parent = level.transform;
                            tempX++;
                        } else {
                            tempTile = Instantiate(litBackgroundWalls, new Vector3(position.x + tileCounter, position.y - tileDepth, 0), Quaternion.identity) as GameObject;
                            tempTile.transform.parent = level.transform;
                            tempX = 0;
                        }                     
                    } else if (tileCounter >= 10) {
                        tileCounter = 0;
                        tileDepth++;
                        if(tempX < 10) {
                            tempTile = Instantiate(backgroundWalls, new Vector3(position.x + tileCounter, position.y - tileDepth, 0), Quaternion.identity) as GameObject;
                            tempTile.transform.parent = level.transform;
                            tempX++;
                        } else {
                            tempTile = Instantiate(litBackgroundWalls, new Vector3(position.x + tileCounter, position.y - tileDepth, 0), Quaternion.identity) as GameObject;
                            tempTile.transform.parent = level.transform;
                            tempX = 0;
                        }
                    }
					break; 
				//A solid block - simple as that
				case 1:
					if (tileCounter < 10) {
						tempTile = Instantiate(solBlock, new Vector3(position.x + tileCounter, position.y - tileDepth, 0),Quaternion.identity) as GameObject;
						tempTile.transform.parent = level.transform;
					} else if (tileCounter >= 10) {
						tileCounter = 0; 
						tileDepth++;
						tempTile = Instantiate(solBlock, new Vector3(position.x + tileCounter, position.y - tileDepth, 0),Quaternion.identity) as GameObject;
						tempTile.transform.parent = level.transform;
					}
					break;
				//May be solid block or empty space 
				case 2:
					int temp2 = Random.Range (0,2);
                    if (temp2 == 0) {
                        if (tileCounter < 10) {
                            if (tempX < 10) {
                                tempTile = Instantiate(backgroundWalls, new Vector3(position.x + tileCounter, position.y - tileDepth, 0), Quaternion.identity) as GameObject;
                                tempTile.transform.parent = level.transform;
                                tempX++;
                            } else {
                                tempTile = Instantiate(litBackgroundWalls, new Vector3(position.x + tileCounter, position.y - tileDepth, 0), Quaternion.identity) as GameObject;
                                tempTile.transform.parent = level.transform;
                                tempX = 0;
                            }
                        } else if (tileCounter >= 10) {
                            tileCounter = 0;
                            tileDepth++;
                            if (tempX < 10) {
                                tempTile = Instantiate(backgroundWalls, new Vector3(position.x + tileCounter, position.y - tileDepth, 0), Quaternion.identity) as GameObject;
                                tempTile.transform.parent = level.transform;
                                tempX++;
                            } else {
                                tempTile = Instantiate(litBackgroundWalls, new Vector3(position.x + tileCounter, position.y - tileDepth, 0), Quaternion.identity) as GameObject;
                                tempTile.transform.parent = level.transform;
                                tempX = 0;
                            }
                        }
                    } else if (temp2 == 1) {
                        if (tileCounter < 10) {
                            tempTile = Instantiate(solBlock, new Vector3(position.x + tileCounter, position.y - tileDepth, 0), Quaternion.identity) as GameObject;
                            tempTile.transform.parent = level.transform;
                        } else if (tileCounter >= 10) {
                            tileCounter = 0;
                            tileDepth++;
                            tempTile = Instantiate(solBlock, new Vector3(position.x + tileCounter, position.y - tileDepth, 0), Quaternion.identity) as GameObject;
                            tempTile.transform.parent = level.transform;
                        }
                    }
					break;
				case 3:
					if (tileCounter < 10) {
					    player.transform.position = new Vector3(position.x + tileCounter, position.y - tileDepth, -1);
                        tempTile = Instantiate(backgroundWalls, new Vector3(position.x + tileCounter, position.y - tileDepth, 0), Quaternion.identity) as GameObject;
                        tempTile.transform.parent = level.transform;
                    } else if (tileCounter >= 10) {
						tileCounter = 0; 
						tileDepth++;
                        tempTile = Instantiate(backgroundWalls, new Vector3(position.x + tileCounter, position.y - tileDepth, 0), Quaternion.identity) as GameObject;
                        tempTile.transform.parent = level.transform;
                        player.transform.position = new Vector3(position.x + tileCounter, position.y - tileDepth, -1);
                    }
				    break;
			    case 4:
				    if (tileCounter < 10) {
					    exit.transform.position = new Vector3(position.x + tileCounter, position.y - tileDepth, -1);
                        tempTile = Instantiate(backgroundWalls, new Vector3(position.x + tileCounter, position.y - tileDepth, 0), Quaternion.identity) as GameObject;
                        tempTile.transform.parent = level.transform;
                    } else if (tileCounter >= 10) {
					    tileCounter = 0; 
					    tileDepth++;
					    exit.transform.position = new Vector3(position.x + tileCounter, position.y - tileDepth, -1);
                        tempTile = Instantiate(backgroundWalls, new Vector3(position.x + tileCounter, position.y - tileDepth, 0), Quaternion.identity) as GameObject;
                        tempTile.transform.parent = level.transform;
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

		for (int q = 0; q < 3; q++) {

			for (int i = 0; i < 45 - optCounter1; i++) {
				GameObject tempTile = Instantiate(solBlock, new Vector3(tempPos.x + counter1, tempPos.y, 0), Quaternion.identity) as GameObject;
                tempTile.transform.parent = walls.transform;
				counter1++;
			}

			for (int n = 0; n < 38 - optCounter2; n++) {
				GameObject tempTile = Instantiate(solBlock, new Vector3(tempPos.x, tempPos.y - counter2, 0), Quaternion.identity) as GameObject;
                tempTile.transform.parent = walls.transform;
				counter2++;
			}

			for (int m = 0; m < 43; m++) {
				GameObject tempTile = Instantiate(solBlock, new Vector3(tempPos2.x + counter3, tempPos2.y, 0), Quaternion.identity) as GameObject;
				tempTile.transform.parent = walls.transform;
				counter3++;
			}

			for (int b = 0; b < 32; b++) {
				GameObject tempTile = Instantiate(solBlock, new Vector3(tempPos3.x, tempPos3.y - counter4, 0), Quaternion.identity) as GameObject;
                tempTile.transform.parent = walls.transform;
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

		firstRun = false;
		int counter = 0;
		
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
					roomLayout[i - 1] = indexList[counter].GetRType();
					counter++;
				}
			}
		}
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


    private int[] layoutSelector (int rType) {
        string layout1;
        string layout2;
        string layout3;
        string layout4;
		string temp1;
		string temp2;
		int rand;

		switch (rType) {
            /*This is a different procedural generation method for type 0 rooms (i.e. Not on the main solution path).
            it is very much still a work in progress.*/
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

            //1 represents a solid grid space, 2 represents a randomised grid space and 0 represents an empty grid space
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
}
