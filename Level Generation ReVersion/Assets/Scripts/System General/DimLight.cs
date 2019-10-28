using UnityEngine;
using System.Collections;

public class DimLight : MonoBehaviour {
	private Light temp;
	private bool bright;
	public float smooth;
	
	void Start () {
		temp = GetComponent<Light>();
	}

	// Update is called once per frame
	void Update () {
		if (bright && temp.intensity >= 0.15) {
			temp.intensity -= Time.deltaTime * smooth;
		} else if (temp.intensity <= 0.15) {
			bright = false;
		}

		if (!bright && temp.intensity <= 0.25) {
			temp.intensity += Time.deltaTime * smooth;
		} else if (temp.intensity >= 0.25) {
			bright = true;	
		}

	}
}
