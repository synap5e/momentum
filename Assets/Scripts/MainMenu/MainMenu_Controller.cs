using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu_Controller : MonoBehaviour {
	
	
	public GameObject fader;
//	public GameObject canvas;
	public GameObject momentumTitle;
	public float fadeSpeed = 5f;
	
	public GameObject mainMenu;
	public GameObject modeMenu;	
	public GameObject levelSelectMenu;
	public GameObject creditsMenu;
	public GameObject settingsMenu;
	public GameObject skipMenu;
	
	//	private bool inModeMenu = false;
	private bool fadedIn = false;
	private int currentLevel = 0;
	static public int currentMode = 0; // 0 = Normal and 1 = SpeedRun
	public enum menuNames {MainMenu, ModeMenu,LevelSelectMenu,CreditsMenu,SettingsMenu};


	// Use this for initialization
	void Awake () {
//		canvas.gameObject.active = true;
		fader.gameObject.SetActive (true);
		
		mainMenu.SetActive(false); //set to true for testing.
		modeMenu.SetActive (false);
		levelSelectMenu.SetActive (false);
		creditsMenu.SetActive (false);
		settingsMenu.SetActive (false);
		skipMenu.SetActive (true);
		
		fader = GameObject.Find("ScreenFader");
		fader.GetComponent<UnityEngine.UI.RawImage> ().CrossFadeAlpha (0f,.5f, true);
//		DontDestroyOnLoad (this);
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.Escape))
		{			

			if(settingsMenu.activeSelf){
				MainMenu();
			}
			else if(creditsMenu.activeSelf){
				MainMenu();
			}
			else if(levelSelectMenu.activeSelf){
				MainMenu();
			}
			else if(modeMenu.activeSelf){
				LevelSelect();
			}
			else if(mainMenu.activeSelf){
				//mainmenu so do nothing
			}
			
			else{
				Debug.Log("Missing menu in escape listener in MainMenu_Controller");
			}
		}
		if(!fadedIn && Input.GetKeyDown(KeyCode.Space)){
			TitleOff();
			TextFadeIn();
		}
	}
	
	void TitleOff () {
		momentumTitle.gameObject.SetActive(false);
		skipMenu.SetActive(false);
	}
	
	void TitleOn () {
		momentumTitle.gameObject.SetActive(true);
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
		setMenuActive (menuNames.ModeMenu);
	}
	
	public void MainMenu(){
		setMenuActive (menuNames.MainMenu);
	}
	
	public void LevelSelect(){
		setMenuActive (menuNames.LevelSelectMenu);
	}

	public void Credits(){
		setMenuActive (menuNames.CreditsMenu);
	}

	public void Settings(){
		setMenuActive (menuNames.SettingsMenu);
	}

	public void setMenuActive(menuNames currentMenu){

		//set all menus to false
		creditsMenu.SetActive (false);
		levelSelectMenu.SetActive (false);
		mainMenu.SetActive (false);
		modeMenu.SetActive (false);
		settingsMenu.SetActive (false);

		switch(currentMenu)
		{
		case menuNames.CreditsMenu:
			creditsMenu.SetActive (true);
			break;
		case menuNames.LevelSelectMenu:
			levelSelectMenu.SetActive(true);
			break;
		case menuNames.MainMenu:
			mainMenu.SetActive(true);
			break;
		case menuNames.ModeMenu:
			modeMenu.SetActive(true);
			break;
		case menuNames.SettingsMenu:
			settingsMenu.SetActive(true);
			break;
		default:
			Debug.Log("missing menu in MainMenu_Controller");
			break;
		}

	}
	
	public void setMode(int mode){
		currentMode = mode;
		if (currentLevel == 1)
			Application.LoadLevel ("Eliot Easy");
		else if (currentLevel == 2)
			Application.LoadLevel ("Eliot Medium");
		else if (currentLevel == 3)
			Application.LoadLevel ("Eliot Hard");
		else 
			Debug.Log ("Missing level in MainMenu_Controller");
	}
	
	public void QuitGameYes(){
		Application.Quit();
		Debug.Log ("Quit");
	}
	
	public void setLevel(int levelNumber){
		currentLevel = levelNumber;
		if (levelNumber == 0) {
			setMode (0); // Normal
			Application.LoadLevel ("Eliot Tutorial");
		}
		else
			ModeMenu ();
	}

}



