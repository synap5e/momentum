using UnityEngine;
using System.Collections;
using ExtensionMethods;

public class ThirdPersonCameraController : MonoBehaviour {

	public float mouseSensitivity = 5F;
	public float speed = 10.0f;
	public float surfaceFriction = 0.25f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		float rotation = Input.GetAxis("Mouse X") * mouseSensitivity;
		float cameraTilt = -Input.GetAxis("Mouse Y") * mouseSensitivity;

		transform.localEulerAngles = new Vector3((transform.localEulerAngles.x + cameraTilt).Clamp(90, 270), transform.localEulerAngles.y + rotation, transform.localEulerAngles.z);

		// Calculate how fast we want to be moving
		Vector3 targetVelocity = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Elevation"), Input.GetAxisRaw("Vertical"));
		targetVelocity.Normalize();
		targetVelocity = transform.TransformDirection(targetVelocity);
		targetVelocity *= speed;

		// Apply a force that attempts to reach our target velocity
		Vector3 velocity = GetComponent<Rigidbody>().velocity;
		Vector3 velocityChange = (targetVelocity - velocity);

		// apply acceleration at friction rate
		velocityChange *= surfaceFriction;

		GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);
	}
}
