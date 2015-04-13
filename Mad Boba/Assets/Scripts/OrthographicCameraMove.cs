using UnityEngine;
using System.Collections;

public class OrthographicCameraMove : MonoBehaviour {
	
	public int edgeDistance = 100;
	public float cameraMoveSpeed = 5.0f;
	public float focusSpeed = 10.0f;

	public GameObject focusableCharacter;

	private Vector3 desiredOffset;

	private Vector3 startPosition;
	private Vector3 desiredPosition;
	private float focust = 1;

	// Use this for initialization
	void Start () {
		desiredOffset = transform.position - focusableCharacter.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 move = Vector3.zero;

		if (Input.mousePosition.x < edgeDistance) {
			// left edge
			move.x += Mathf.Min(edgeDistance - Input.mousePosition.x, edgeDistance);
		} else if (Input.mousePosition.x > Screen.width - edgeDistance) {
			// right edge
			move.x -= Mathf.Min(edgeDistance - (Screen.width - Input.mousePosition.x), edgeDistance);
		}

		if (Input.mousePosition.y < edgeDistance) {
			// bottom edge
			move.y += Mathf.Min(edgeDistance - Input.mousePosition.y, edgeDistance);
		} else if (Input.mousePosition.y > Screen.height - edgeDistance) {
			// top edge
			move.y -= Mathf.Min(edgeDistance - (Screen.height - Input.mousePosition.y), edgeDistance);
		}

		if (Input.GetKey (KeyCode.Space)) {
			startPosition = transform.position;
			desiredPosition = focusableCharacter.transform.position + desiredOffset;
			focust = 0;
		}

		if (focust < 1) {
			focust += (focusSpeed / 100.0f);
			transform.position = Vector3.Lerp (startPosition, desiredPosition, focust);
		} else {
			transform.position -= move * (cameraMoveSpeed / 100.0f) * Time.deltaTime;
		}
	}
}
