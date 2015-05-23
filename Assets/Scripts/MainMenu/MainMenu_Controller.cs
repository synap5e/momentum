using UnityEngine;
using System.Collections;

public class MainMenu_Controller : MonoBehaviour {



	public GameObject momentumTitle;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	 void TitleOff () {
		momentumTitle.gameObject.active = false;
	}


	 void TitleOn () {
		momentumTitle.gameObject.active = true;
	}



}


