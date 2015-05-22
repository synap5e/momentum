using UnityEngine;
using System.Collections;

public class FirstPersonCameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeFov(float fov){
		GetComponent<Camera> ().fieldOfView = fov;
	}
}
