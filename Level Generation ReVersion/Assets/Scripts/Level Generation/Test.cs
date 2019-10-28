using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	private int temp;
	public int temp2;

	// Update is called once per frame
	void Update () {
		if (temp2 < 16){ 
			temp2++;
			print (temp2 % 4);
		}
	}
}
