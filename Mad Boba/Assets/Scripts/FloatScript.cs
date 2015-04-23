using UnityEngine;
using System.Collections;

public class FloatScript : MonoBehaviour {

	void FixedUpdate () {
		GetComponent<Rigidbody>().AddForce(new Vector3(0, -transform.position.y/1.0f, 0), ForceMode.Impulse);
	}


}
