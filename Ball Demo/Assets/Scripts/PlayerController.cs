using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	float speed = 6.0f;
	float jumpSpeed = 300f;
	float gravity = 10.0f;
	private Vector3 moveDirection;
	private bool isGrounded = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Rigidbody rigidBody = GetComponent<Rigidbody>();
		moveDirection = Vector3.zero;

		if (isGrounded) {
			moveDirection = new Vector3 (-Input.GetAxis ("Vertical"), 0, Input.GetAxis ("Horizontal"));
			moveDirection = Vector3.Normalize(moveDirection);
			moveDirection *= speed;
			
			if (Input.GetKey( KeyCode.Space)) {
				moveDirection.y = jumpSpeed;
				isGrounded = false;
			}
		} else {
			//moveDirection.y -= gravity * Time.deltaTime;
		}
		rigidBody.AddForce(moveDirection * Time.deltaTime, ForceMode.Impulse);
	}

	void OnCollisionEnter(Collision other) {
		//Debug.Log (other.gameObject.tag);
		if(other.gameObject.tag == "Ground"){
			isGrounded = true;
		}
	}

	void OnCollisionExit(Collision other) {
		if(other.gameObject.tag == "Ground"){
			isGrounded = false;
		}
	}
}
