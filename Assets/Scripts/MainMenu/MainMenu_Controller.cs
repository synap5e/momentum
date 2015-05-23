using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu_Controller : MonoBehaviour {


	public GameObject Fader;
	public GameObject momentumTitle;
	public float fadeSpeed = 5f;




	// Use this for initialization
	void Awake () {
		Fader.gameObject.active = true;
		Fader = GameObject.Find("ScreenFader");
	Fader.GetComponent<UnityEngine.UI.RawImage> ().CrossFadeAlpha (0f,.5f, true);
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


	void FadeIn () {
		Fader.GetComponent<UnityEngine.UI.RawImage> ().CrossFadeAlpha (1f,.5f, true);
	}

	void FadeOut () {
		Fader.GetComponent<UnityEngine.UI.RawImage> ().CrossFadeAlpha (0f,.5f, true);
	}




}


