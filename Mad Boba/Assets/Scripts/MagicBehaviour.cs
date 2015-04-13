using UnityEngine;
using System.Collections;

public class MagicBehaviour : MonoBehaviour {

	public float speed = 10.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += transform.forward * speed * Time.deltaTime;
	}
}
