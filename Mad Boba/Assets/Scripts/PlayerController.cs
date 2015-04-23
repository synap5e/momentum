using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float walkSpeed = 2.0f;
	public float rotateSpeed = 2.0f;
	public float destinationThreshold = 1.0f;
	public float sprintMultiplier = 1.5f;

	public bool useForces = false;

	static Plane worldPlane = new Plane(Vector3.up, Vector3.zero);

	private Vector3 destination;
	private bool traveling = false;
	private bool keyboardOverride = false;

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
		Vector3 keyboardMove = new Vector3 (Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		if (keyboardMove.magnitude > 0) {
			destination = transform.position + keyboardMove * 100.0f;
			traveling = true;
			keyboardOverride = true;
			GetComponent<ArgoAnimationController> ().SignalStartMoving ();
		} else if (keyboardOverride) {
			traveling = false;
			keyboardOverride = false;
		}

		if (Input.GetButton("Fire2")) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float rayDistance;
			if(worldPlane.Raycast (ray, out rayDistance)) {
				Vector3 hitPoint = ray.GetPoint(rayDistance);
				hitPoint.y = 0;
				destination = hitPoint;
				traveling = true;
				keyboardOverride = false;
				GetComponent<ArgoAnimationController>().SignalStartMoving();
			}
		}

		Vector3 toTarget = destination - transform.position;
		toTarget.y = 0;
		if (toTarget.magnitude < destinationThreshold && traveling) {
			traveling = false;
			GetComponent<ArgoAnimationController>().SignalStopMoving();
		}

		float speed = 1;
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			speed = sprintMultiplier;
		}

		if (traveling) {
			Quaternion lookRotation = Quaternion.LookRotation (toTarget, Vector3.up);
			float rotate = Quaternion.Angle (transform.rotation, lookRotation);
			float donePercentage = Mathf.Min (1F, Time.deltaTime / (rotate / (rotateSpeed * 100)));
			transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, donePercentage);

			if (useForces){
				GetComponent<Rigidbody>().AddForce(transform.forward * speed * Time.deltaTime * walkSpeed, ForceMode.Impulse);
			} else {
				transform.position += transform.forward * speed * Time.deltaTime * walkSpeed;
			}
		}


	}

	void FixedUpdate()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			GetComponent<Rigidbody>().AddForce(Vector3.up * 1.0f, ForceMode.Impulse);
		}
	}
}
