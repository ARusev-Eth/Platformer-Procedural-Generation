using UnityEngine;
using System.Collections;

public class DestroyCheck : MonoBehaviour {
	
	// Checks all colliders which come in contact with the trigger
	private void OnTriggerEnter2D (Collider2D c)
	{
		if (c.tag == "Player"){
			GameObject.FindGameObjectWithTag("NPC").GetComponent<SM4_R7>().SetAggro(true);
			GameObject.FindGameObjectWithTag("NPC").GetComponent<SM4_R7>().SetChase(false);
			GameObject.FindGameObjectWithTag("NPC").GetComponent<SM4_R7>().SetIdle(false);
		}
	}

	// Useful for melee attackers mostly
	private void OnTriggerExit2D (Collider2D c)
	{
		if (c.tag == "Player"){
			GameObject.FindGameObjectWithTag("NPC").GetComponent<SM4_R7>().SetChase(true);
			GameObject.FindGameObjectWithTag("NPC").GetComponent<SM4_R7>().SetAggro(false);
		}
	}
}
