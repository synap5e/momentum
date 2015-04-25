using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

// adapted from http://wiki.unity3d.com/index.php?title=RigidbodyFPSWalker
public class RigidbodyFPSController : MonoBehaviour {
	
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	
	public float speed = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;

	private bool grounded = true;
	public bool onGround {
		get { return grounded; }
	}
	
	private Rigidbody rigidBody;
	private Camera viewCamera;

	
	void Awake () {
		rigidBody = GetComponent<Rigidbody> ();
		viewCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();

		rigidBody.freezeRotation = true;
		rigidBody.useGravity = false;
	}

		
	void Start() {

	}
	
	void FixedUpdate () {
		// mouse X axis rotates the playerm but the Y axis simply tilts the camera
		float rotationX = Input.GetAxis("Mouse X") * sensitivityX;
		float cameraTilt = -Input.GetAxis("Mouse Y") * sensitivityY;

		transform.Rotate(new Vector3(0, rotationX, 0));
		applyTiltClamped (cameraTilt, 90, 270);

		if (grounded) {
			// Calculate how fast we should be moving
			Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			targetVelocity.Normalize();
			targetVelocity = transform.TransformDirection(targetVelocity);
			targetVelocity *= speed;
			
			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = rigidBody.velocity;
			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
			velocityChange.y = 0;
			rigidBody.AddForce(velocityChange, ForceMode.VelocityChange);
			
			// Jump
			if (canJump && Input.GetButton("Jump")) {
				rigidBody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
			}
		}
		
		// We apply gravity manually for more tuning control
		rigidBody.AddForce(Physics.gravity * rigidBody.mass);
		
		grounded = false;
	}
	
	void OnCollisionStay () {
		grounded = true;    
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
	
	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * -Physics.gravity.y);
	}
	
}
