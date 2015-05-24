using UnityEngine;
using System.Collections;

public class FirstPersonBombController : MonoBehaviour {

	private GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		BombActivator bombAc = player.GetComponent<BombActivator>();
		Animator anim = GetComponent<Animator>();
		if (bombAc.nearBomb ()) {
			anim.SetBool ("Near Bomb", true);
			if (Input.GetMouseButtonDown (0)){
				anim.SetTrigger ("Click");
			}
		}
		else anim.SetBool ("Near Bomb", false);
	}
}
