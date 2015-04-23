using UnityEngine;
using System.Collections;

public class PickupableCoin : MonoBehaviour {

	public GameObject pickerUpper;
	public CastMagic coinThrower;
	public float pickupDistance = 2.0f;

	void Update () {
		if (Vector3.Magnitude(transform.position - pickerUpper.transform.position) < pickupDistance){
			coinThrower.coins ++;
			Destroy(gameObject);
		}
	}
}
