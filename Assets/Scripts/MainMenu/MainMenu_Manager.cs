using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu_Manager : MonoBehaviour {


	public GameObject fader;
	public GameObject canvas;
	public GameObject menu;
	public GameObject momentumTitle;
	public float fadeSpeed = 5f;

	
	public GameObject mainMenu;
	public GameObject modeMenu;
		

	private bool inModeMenu = false;



	// Use this for initialization
	void Awake () {
		canvas.gameObject.active = true;
		fader.gameObject.active = true;

		mainMenu.SetActive(false); //set to true for testing.
		modeMenu.SetActive (false);


		fader = GameObject.Find("ScreenFader");
	fader.GetComponent<UnityEngine.UI.RawImage> ().CrossFadeAlpha (0f,.5f, true);

		}

	// Update is called once per frame






	 void TitleOff () {
		momentumTitle.gameObject.active = false;
	}

	void TitleOn () {
		momentumTitle.gameObject.active = true;
	}


	void FadeIn () {
		fader.GetComponent<UnityEngine.UI.RawImage> ().CrossFadeAlpha (1f,.5f, true);
	}

	void FadeOut () {
		fader.GetComponent<UnityEngine.UI.RawImage> ().CrossFadeAlpha (0f,.5f, true);

	}

	void TextFadeIn () {
		mainMenu.SetActive(true);
	}

	public void ChangeScene (int sceneToChangeTo) {
		Application.LoadLevel (sceneToChangeTo);	
	}


	public void ModeMenu(){
		mainMenu.SetActive (false);
		modeMenu.SetActive (true);
		inModeMenu = true;
	}
	public void ModeReturn(){
		mainMenu.SetActive (true);
		modeMenu.SetActive (false);
		inModeMenu = false;
	}

	public void QuitGameYes(){
		Application.Quit();
		Debug.Log ("Quit");
	}


}




