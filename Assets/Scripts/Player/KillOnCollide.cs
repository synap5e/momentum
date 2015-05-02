using UnityEngine;
using System.Collections;

public class KillOnCollide : MonoBehaviour
{
	public GameObject spawner;
	
	void Start()
	{
		spawner.SetActive(false);
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			// TODO: fancy animations etc, inform feedback, show on debug etc.
			collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
			collision.gameObject.transform.position = spawner.transform.position;

			collision.gameObject.GetComponent<BombActivator>().ReactivateBombs();
		}
	}
	
}