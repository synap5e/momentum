using UnityEngine;
using System.Collections;

public class FirstPersonAnimationController : MonoBehaviour {
	
	private GameObject player;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Animator anim = GetComponent<Animator>();
		if (Input.GetAxis ("Horizontal") != 0f || Input.GetAxis ("Vertical") != 0f) {
			anim.SetFloat ("Velocity", 1f);
		} else {
			anim.SetFloat ("Velocity", 0);
		}

		if (!player.GetComponent<RigidbodyFPSController> ().onGround) {
			anim.SetBool ("Jumping", true);
		} else {
			anim.SetBool ("Jumping", false);
		}		 
	}
}
