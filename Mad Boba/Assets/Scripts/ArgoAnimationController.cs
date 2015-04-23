using UnityEngine;
using System.Collections;

public interface ArgoAnimationController 
{

	float walkSpeed {
		set;
	}

	void SignalStopMoving ();

	void SignalStartMoving ();
}
