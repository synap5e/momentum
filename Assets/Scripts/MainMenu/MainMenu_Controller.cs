using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu_Controller : MonoBehaviour {
	
	
	public GameObject fader;
	public GameObject canvas;
	public GameObject momentumTitle;
	public float fadeSpeed = 5f;
	
	public GameObject mainMenu;
	public GameObject modeMenu;	
	public GameObject levelSelectMenu;
	
	//	private bool inModeMenu = false;
	private bool fadedIn = false;
	private int currentLevel = 0;
	static public int currentMode = 0; // 0 = Normal and 1 = SpeedRun
	
	
	// Use this for initialization
	void Awake () {
		canvas.gameObject.active = true;
		fader.gameObject.active = true;
		
		mainMenu.SetActive(false); //set to true for testing.
		modeMenu.SetActive (false);
		levelSelectMenu.SetActive (false);
		
		fader = GameObject.Find("ScreenFader");
		fader.GetComponent<UnityEngine.UI.RawImage> ().CrossFadeAlpha (0f,.5f, true);
		DontDestroyOnLoad (this);
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
		fader.GetComponent<UnityEngine.UI.RawImage> ().CrossFadeAlpha (1f,.5f, true);
	}
	
	void FadeOut () {
		fader.GetComponent<UnityEngine.UI.RawImage> ().CrossFadeAlpha (0f,.5f, true);
	}
	
	void TextFadeIn () {
		if (!fadedIn) {
			mainMenu.SetActive (true);
			fadedIn = true;
		}
	}
	
	public void ModeMenu(){
		modeMenu.SetActive (true);
		mainMenu.SetActive (false);
		levelSelectMenu.SetActive (false);
	}
	
	public void MainMenu(){
		mainMenu.SetActive (true);
		modeMenu.SetActive (false);
		levelSelectMenu.SetActive (false);
	}
	
	public void LevelSelect(){
		levelSelectMenu.SetActive (true);
		mainMenu.SetActive (false);
		modeMenu.SetActive (false);
	}
	
	public void setMode(int mode){
		currentMode = mode;
		if (currentLevel ==0) Application.LoadLevel ("Eliot Tutorial");
		else if (currentLevel ==1) Application.LoadLevel ("Eliot 2");
		else if (currentLevel ==2) Application.LoadLevel ("Eliot 3");
		else Application.LoadLevel ("Eliot-hard");
	}
	
	public void QuitGameYes(){
		Application.Quit();
		Debug.Log ("Quit");
	}
	
	public void setLevel(int levelNumber){
		currentLevel = levelNumber;
		ModeMenu ();
	}
}



