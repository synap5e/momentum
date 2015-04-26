using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]
[RequireComponent (typeof (DebugMovement))]

// adapted from http://wiki.unity3d.com/index.php?title=RigidbodyFPSWalker
public class RigidbodyFPSController : MonoBehaviour {

	[Header("Input")]
	[Range(0.01f, 20f)]
	public float mouseSensitivity = 5F;

	[Header("Basic Movement")]
	public float speed = 10.0f;
	public float jumpForce = 10.0f;

	[Header("Bunnyhopping")]
	[Tooltip("Length of the window that we will accept a bunnyhop in.\n" +
	         "This allows the player 1/2 that time before hitting the ground and 1/2 after. " + 
	         "If they jump within this time they will not lose any velocity to friction.")]
	public float bunnyhopWindow = 0.2f;
	public bool autoBunnyhop = false;

	[Header("Airstrafing")]
	public float maxStrafeSpeed = 5.0f;
	public float airAccelerate = 0.2f;

	
	private bool grounded = true;
	bool onGround {
		get { return grounded; }
	}
	
	private Rigidbody rigidBody;
	private Camera viewCamera;

	private float jump = 0;
	private float onGroundTime;
	private Vector3 incomingVel;
	
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

	void Update(){
		// TODO: hack to reset while testing 
		if (transform.position.y < -20) {
			transform.position= Vector3.zero;
		}

		// TODO: hack to simulate explosive jump
		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			rigidBody.AddForce(transform.TransformVector(new Vector3(0, 600, 2000)));
		}

		if (grounded) {
			onGroundTime += Time.deltaTime;
		}

		if ((autoBunnyhop && Input.GetButton ("Jump")) || (jump <= 0 && Input.GetButtonDown ("Jump"))) {
			jump = bunnyhopWindow / 2F;
		}
		jump -= Time.deltaTime;

		//Debug.DrawLine(transform.position, transform.position + rigidBody.velocity * Time.deltaTime, Color.green, 10);
	}

	void OnGUI(){
		Vector3 horvel = rigidBody.velocity;
		horvel.y = 0;
		GUI.Label (new Rect (0, 0, 100, 100), Mathf.Round(horvel.magnitude*100)/100F + "");


		GUI.Label (new Rect (0, 80, 100, 100), Mathf.Round(horvel.magnitude*10000)/10000F + " / " + Mathf.Round(incomingVel.magnitude*10000)/10000F + "");
	}


	void FixedUpdate () {
		Debug.DrawLine(transform.position, transform.position + transform.forward, Color.blue);

		Vector3 rv = rigidBody.velocity;
		rv.y = 0;
		Debug.DrawLine(transform.position, transform.position + rv, Color.green);


		// mouse X axis rotates the playerm but the Y axis simply tilts the camera
		float rotationX = Input.GetAxis("Mouse X") * mouseSensitivity;
		float cameraTilt = -Input.GetAxis("Mouse Y") * mouseSensitivity;
		
		transform.Rotate(new Vector3(0, rotationX, 0));
		applyTiltClamped (cameraTilt, 90, 270);
		 
		if (onGround) {
			if (jump > 0) {
				rigidBody.AddForce (Vector3.up * jumpForce, ForceMode.VelocityChange);
				jump = 0;

				if (onGroundTime < bunnyhopWindow / 2F){
					// we have already hit the ground, but jumped within the bhop window (and still had velocity left)

					if (Mathf.Abs (rigidBody.velocity.sqrMagnitude - incomingVel.sqrMagnitude) < 0.000001){
						// no velocity was lost, perfect bhop
						Debug.Log ("Perfect");
					} else {
						Debug.Log ("imperfect");
						// player has lost some (or all) of the velocity that they came into this bhop with, but we will
						// grant it back

						rigidBody.velocity = new Vector3(incomingVel.x, rigidBody.velocity.y, incomingVel.z);

					}
				}
			} else {
				// Calculate how fast we should be moving
				Vector3 targetVelocity = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
				targetVelocity.Normalize ();
				targetVelocity = transform.TransformDirection (targetVelocity);
				targetVelocity *= speed;

				// Apply a force that attempts to reach our target velocity
				Vector3 velocity = rigidBody.velocity;
				Vector3 velocityChange = (targetVelocity - velocity);
				velocityChange.y = 0;

				// friction coefficient
				velocityChange *= 0.25f;

				rigidBody.AddForce (velocityChange, ForceMode.VelocityChange);
			}
		} else {
			// airstrafe

			if (rotationX != 0){

				Vector3 accellDir = transform.TransformVector(new Vector3(
					(rotationX > 0 ? 1 : -1) * airAccelerate, 0, 0
				));

				Debug.DrawLine(transform.position, transform.position + accellDir, Color.red);

				float projectedSpeed = Vector3.Dot(rigidBody.velocity, accellDir);

				float allowedAccell = Mathf.Max (0, maxStrafeSpeed - projectedSpeed);

				rigidBody.velocity = rigidBody.velocity + accellDir * allowedAccell;

			}

		}

		// We apply gravity manually for more tuning control
		rigidBody.AddForce(Physics.gravity * 3.0f * rigidBody.mass);
	}
	
	void OnCollisionExit(Collision collisionInfo) {
		// TODO: only on ground object
		grounded = false;
		onGroundTime = 0;
	}
	
	void OnCollisionEnter(Collision collisionInfo) {
		// TODO: only for ground objects with a correct normal of the face we hit

		Vector3 incomingVelocity = rigidBody.velocity;
		incomingVelocity.y = 0;
		incomingVel = incomingVelocity;

		grounded = true; 
	}
	
	void OnCollisionStay (Collision collisionInfo) {
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
	
}