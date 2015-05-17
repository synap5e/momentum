using UnityEngine;
using System.Collections;

public class FirstPersonBombController : MonoBehaviour {

	public GameObject player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		BombActivator bombAc = player.GetComponent<BombActivator>();
		Animator anim = GetComponent<Animator>();
		if (bombAc.nearBomb ()) {
			anim.SetBool ("Near Bomb", true);
			if (Input.GetMouseButtonDown (0))
				anim.SetTrigger ("Click");
		}
		else anim.SetBool ("Near Bomb", false);

	
	
	}
}
