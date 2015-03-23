using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	private bool isActive;

	// Use this for initialization
	void Start () {
		isActive = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isActive)
		{
			return;
		}
		GameObject player = GameObject.FindWithTag("Player");
		Vector3 playerPosition = player.transform.position;
		Vector3 directionToPlayer = Vector3.Normalize(playerPosition - transform.position);
		directionToPlayer.y = 0;

		Rigidbody rigidBody = GetComponent<Rigidbody>();
		rigidBody.AddForce(directionToPlayer * Time.deltaTime, ForceMode.Impulse);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "GuardArea")
		{
			isActive = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "GuardArea")
		{
			isActive = false;
		}
	}
}
