using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

// adapted from http://wiki.unity3d.com/index.php?title=RigidbodyFPSWalker
public class RigidbodyFPSController : MonoBehaviour {
	
	private float sensitivityX = 5F;
	private float sensitivityY = 5F;
	
	private float speed = 10.0f;
	private float jumpHeight = 4.0f;

	private float bunnyhopThreshold = 0.6f;
	
	private bool grounded = true;
	bool onGround {
		get { return grounded; }
	}
	
	private Rigidbody rigidBody;
	private Camera viewCamera;

	private float jump = 0;
	private float onGroundTime;
	private float incomingMagnitude;
	private bool compensatingBhop = false; 
	
	void Awake () {
		rigidBody = GetComponent<Rigidbody> ();
		viewCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
		
		rigidBody.freezeRotation = true;
		rigidBody.useGravity = false;

		// frictionless
		GetComponent<Collider> ().material.dynamicFriction = 0;
		GetComponent<Collider> ().material.frictionCombine = PhysicMaterialCombine.Multiply;
	}
	
	
	void Start() {
		
	}

	private float secondsTo0;


	void Update(){
		if (grounded) {
			onGroundTime += Time.deltaTime;
		}

		if (rigidBody.velocity.magnitude > 0.0001) {
			secondsTo0 += Time.deltaTime;
		}

		if (jump <= 0 && Input.GetButtonDown ("Jump")) {
			jump = bunnyhopThreshold / 2F;
		}
		jump -= Time.deltaTime;
	}

	float f = 0;

	void OnGUI(){
		Vector3 horvel = rigidBody.velocity;
		horvel.y = 0;
		GUI.Label (new Rect (0, 0, 100, 100), Mathf.Round(horvel.magnitude*100)/100F + "");
		if (rigidBody.velocity.magnitude < 0.0001) {

			GUI.Label (new Rect (0, 20, 100, 100), secondsTo0 + "s");
		}

		GUI.Label (new Rect (0, 40, 100, 100), Mathf.Round(f*10000)/10000F + "");


		GUI.Label (new Rect (0, 80, 100, 100), Mathf.Round(horvel.magnitude*10000)/10000F + " / " + Mathf.Round(incomingMagnitude*10000)/10000F + "");
	}


	void FixedUpdate () {
		// mouse X axis rotates the playerm but the Y axis simply tilts the camera
		float rotationX = Input.GetAxis("Mouse X") * sensitivityX;
		float cameraTilt = -Input.GetAxis("Mouse Y") * sensitivityY;
		
		transform.Rotate(new Vector3(0, rotationX, 0));
		applyTiltClamped (cameraTilt, 90, 270);
		 
		if (onGround) {
			if (jump > 0) {
				rigidBody.AddForce (Vector3.up * jumpHeight, ForceMode.VelocityChange);
				f = jump;
				jump = 0;

				if (onGroundTime < bunnyhopThreshold / 2F){
					// we have already hit the ground, but jumped within the bhop window


					if (Mathf.Abs (rigidBody.velocity.magnitude - incomingMagnitude) < 0.000001){
						// no velocity was lost, perfect bhop
						Debug.Log ("Perfect");
					} else {
						Debug.Log ("imperfect");

						compensatingBhop = true;
						// we have lost some of the velocity that we came into this bhop with, but will \
						// grant it back to the player

					}
				}
			} else {
				// Calculate how fast we should be moving
				Vector3 targetVelocity = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
				targetVelocity.Normalize ();
				targetVelocity = transform.TransformDirection (targetVelocity);
				targetVelocity *= speed;

				if (targetVelocity.magnitude > 0.001) {
					secondsTo0 = 0;
				}
				
				// Apply a force that attempts to reach our target velocity
				Vector3 velocity = rigidBody.velocity;
				Vector3 velocityChange = (targetVelocity - velocity);
				velocityChange.y = 0;
				velocityChange = Vector3.ClampMagnitude (velocityChange, 0.5f);

				rigidBody.AddForce (velocityChange, ForceMode.VelocityChange);
			}
		} else {
		}

		if (compensatingBhop) {
			if (rigidBody.velocity.magnitude < incomingMagnitude){
				float requiredMultiplier = incomingMagnitude / rigidBody.velocity.magnitude;
				requiredMultiplier = Mathf.Min (requiredMultiplier, 1.5F);
				rigidBody.velocity *= requiredMultiplier;
			} else {
				compensatingBhop = false;
			}
		}
		
		// We apply gravity manually for more tuning control
		rigidBody.AddForce(Physics.gravity * rigidBody.mass);
	}
	
	void OnCollisionExit(Collision collisionInfo) {
		//if (collisionInfo.collider.tag == "ground") {
		grounded = false;
		onGroundTime = 0;
		//	}
		//	Debug.Log ("OP_exit");
	}
	
	void OnCollisionEnter(Collision collisionInfo) {
		Vector3 incomingVelocity = rigidBody.velocity;
		incomingVelocity.y = 0;
		incomingMagnitude = incomingVelocity.magnitude;

		secondsTo0 = 0;
		//	if (collisionInfo.collider.tag == "ground") {
		
		//	}
		//Debug.Log ("OP_enter");
	}
	
	void OnCollisionStay (Collision collisionInfo) {
		grounded = true; 
		//	Debug.Log ("OP_s");
		//	grounded = true;    
	}
	
	void applyTiltClamped (float cameraTilt, float lowerLimit, float upperLimit)
	{
		float newTilt = viewCamera.transform.localEulerAngles.x + cameraTilt;
		newTilt = newTilt % 360;
		if (newTilt < 0){
			newTilt += 360;
		}
		if (newTilt <= 180) {
			// looking down
			newTilt = Mathf.Min(lowerLimit, newTilt);
		} else if (newTilt != 0){
			// looking up
			newTilt = Mathf.Max(upperLimit, newTilt);
		}
		viewCamera.transform.localEulerAngles = new Vector3(newTilt, 0, 0);
	}
	
	/*float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * -Physics.gravity.y);
	}*/
	
}