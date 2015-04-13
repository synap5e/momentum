using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float walkSpeed = 2.0f;
	public float rotateSpeed = 2.0f;
	public float destinationThreshold = 1.0f;

	static Plane worldPlane = new Plane(Vector3.up, Vector3.zero);

	private Vector3 destination;
	private bool traveling = false;

	// Use this for initialization
	void Start () {
		GetComponent<ArgoAnimationController> ().walkSpeed = walkSpeed;
	}
	
	static float Signum(float n) {
		// **sigh**
		return n < 0 ? -1 : (n > 0 ? 1 : 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Fire2")) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float rayDistance;
			if(worldPlane.Raycast (ray, out rayDistance)) {
				Vector3 hitPoint = ray.GetPoint(rayDistance);
				hitPoint.y = 0;
				destination = hitPoint;
				traveling = true;
				GetComponent<ArgoAnimationController>().SignalStartMoving();
			}
		}

		Vector3 toTarget = destination - transform.position;
		toTarget.y = 0;
		if (toTarget.magnitude < destinationThreshold && traveling) {
			traveling = false;
			GetComponent<ArgoAnimationController>().SignalStopMoving();
		}

		if (traveling) {
			Quaternion lookRotation = Quaternion.LookRotation (toTarget, Vector3.up);
			float rotate = Quaternion.Angle (transform.rotation, lookRotation);
			float donePercentage = Mathf.Min (1F, Time.deltaTime / (rotate / (rotateSpeed * 100)));
			transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, donePercentage);

			transform.position += transform.forward * Time.deltaTime * walkSpeed;
		}
	}

	void FixedUpdate()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			GetComponent<Rigidbody>().AddForce(Vector3.up * 1.0f, ForceMode.Impulse);
		}
	}
}
