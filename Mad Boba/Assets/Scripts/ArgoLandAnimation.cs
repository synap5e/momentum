using UnityEngine;
using System.Collections;

public class ArgoLandAnimation : MonoBehaviour, ArgoAnimationController {
	
	public int framesPerIdleAnimation = 100;
	public int framesPerIdleAnimationPlusMinus = 50;
	
	private int idleCountdown;
	
	void Start () {
		SignalStopMoving ();
	}
	
	public float walkSpeed {
		set { GetComponent<Animation> () ["argo_anim_walk"].speed = value / 1.3f; }
	}
	
	public void SignalStopMoving ()
	{
		GetComponent<Animation> ().CrossFade ("argo_anim_idle_1");
		idleCountdown = Mathf.Min (1, framesPerIdleAnimation - Random.Range (-framesPerIdleAnimationPlusMinus, framesPerIdleAnimationPlusMinus)); 
	}
	
	public void SignalStartMoving ()
	{
		GetComponent<Animation> ().CrossFade ("argo_anim_walk");
		idleCountdown = -1;
	}
	
	// Update is called once per frame
	void Update () {
		idleCountdown--;
		if (idleCountdown == 0){
			GetComponent<Animation> ().CrossFade ("argo_anim_idle_1");
			idleCountdown = framesPerIdleAnimation - Random.Range (-framesPerIdleAnimationPlusMinus, framesPerIdleAnimationPlusMinus);
		}
	}
}
