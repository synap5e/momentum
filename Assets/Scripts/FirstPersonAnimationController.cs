using UnityEngine;
using System.Collections;

public class FirstPersonAnimationController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Animator anim = GetComponent<Animator>();
		if (Input.GetAxis ("Horizontal") != 0f || Input.GetAxis ("Vertical") != 0f) {
			anim.SetBool ("Moving", true);
		} else {
			anim.SetBool ("Moving", false);
		}

		 
	}
}
