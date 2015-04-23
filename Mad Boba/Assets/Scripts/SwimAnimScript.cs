using UnityEngine;
using System.Collections;

public class SwimAnimScript : MonoBehaviour, ArgoAnimationController {

	Animator anim;
	public bool canSprint = false;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		//Debug.Log (anim);
	}
	
	// Update is called once per frame
	void Update () {
		anim.SetBool ("sprinting", (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift)) && canSprint);
	}

	public void SignalStopMoving ()
	{
		anim.SetFloat ("speed", 0);
	}

	public void SignalStartMoving ()
	{
		anim.SetFloat ("speed", 1);
	}

	public float walkSpeed {
		set {

		}
	}

}
