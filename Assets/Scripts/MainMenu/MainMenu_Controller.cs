using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.IO;
using SimpleJSON;

public class MainMenu_Controller : MonoBehaviour {
	
	[Header("Title")]
	public GameObject fader;
	public GameObject momentumTitle;
	public float fadeSpeed = 5f;
	
	[Header("Menu")]
	public GameObject mainMenu;
	public GameObject modeMenu;	
	public GameObject levelSelectMenu;
	public GameObject creditsMenu;
	public GameObject controlMenu;
	public GameObject settingsMenu;	
	public GameObject audioMenu;
	public GameObject videoMenu;
	public GameObject skipMenu;
	
	//Music
	private AudioSource mainmenuSource;
	
	private bool fadedIn = false;
	static public int currentLevel = 0;
	static public int currentMode = 1; // 0 = Normal and 1 = SpeedRun
	public enum menuNames {MainMenu, ModeMenu,LevelSelectMenu,CreditsMenu,SettingsMenu,AudioMenu,ControlMenu,VideoMenu};
	
	// Use this for initialization
	void Awake () {
		fader.gameObject.SetActive (true);
		
		// set menus to false except skipMenu
		mainMenu.SetActive(false);
		modeMenu.SetActive (false);
		levelSelectMenu.SetActive (false);
		creditsMenu.SetActive (false);
		settingsMenu.SetActive (false);
		controlMenu.SetActive (false);
		audioMenu.SetActive(false);
		videoMenu.SetActive(false);
		skipMenu.SetActive (true);	
		
		mainmenuSource = GetComponent<AudioSource> ();
		
		//fader = GameObject.Find("ScreenFader");
		fader.GetComponent<UnityEngine.UI.RawImage> ().CrossFadeAlpha (0f,.5f, true);
		
		//Load Settings from file
		GetComponent<SettingsController>().Load ();
		updateSettings ();
	}
	
	// Update is called once per frame
	void Update () {
		Animator anim = GetComponent<Animator>();
		
		//Returns to previous menu
		if( Input.GetKeyDown(KeyCode.Escape))
		{		
			if(creditsMenu.activeSelf || controlMenu.activeSelf || levelSelectMenu.activeSelf){
				MainMenu();
			}
			else if(audioMenu.activeSelf || settingsMenu.activeSelf || videoMenu.activeSelf){
				SettingsRevert();
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
		
		//Skip title
		if(!fadedIn && Input.GetKeyDown(KeyCode.Space)){
			TitleOff();
			TextFadeIn();
			anim.SetTrigger ("Skip");
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
	
	public void Controls(){
		setMenuActive (menuNames.ControlMenu);
	}
	
	public void Settings(){
		setMenuActive (menuNames.SettingsMenu);
	}
	
	public void AudioMenu(){
		setMenuActive (menuNames.AudioMenu);
	}
	
	public void VideoMenu(){
		setMenuActive (menuNames.VideoMenu);
	}
	
	//Sets all menus to false except the currentMenu
	public void setMenuActive(menuNames currentMenu){
		
		//set all menus to false
		creditsMenu.SetActive (false);
		levelSelectMenu.SetActive (false);
		mainMenu.SetActive (false);
		modeMenu.SetActive (false);
		settingsMenu.SetActive (false);
		audioMenu.SetActive(false);
		videoMenu.SetActive(false);
		controlMenu.SetActive(false);
		
		switch(currentMenu)
		{
		case menuNames.CreditsMenu:
			creditsMenu.SetActive (true);
			break;
		case menuNames.ControlMenu:
			controlMenu.SetActive (true);
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
		case menuNames.AudioMenu:
			audioMenu.SetActive(true);
			break;
		case menuNames.SettingsMenu:
			settingsMenu.SetActive(true);
			break;
		case menuNames.VideoMenu:
			videoMenu.SetActive(true);
			break;
		default:
			Debug.Log("missing menu in MainMenu_Controller");
			break;
		}
	}
	
	//Sets what mode and starts the level
	public void setMode(int mode){
		currentMode = mode;
		if (currentLevel == 1)
			Application.LoadLevel ("Eliot Easy");
		else if (currentLevel == 2)
			Application.LoadLevel ("Regan_Medium");
		else if (currentLevel == 3)
			Application.LoadLevel ("Eliot Hard");
		else 
			Debug.Log ("Missing level in MainMenu_Controller");
	}
	
	public void QuitGameYes(){
		Application.Quit();
		Debug.Log ("Quit");
	}
	
	//Sets the current level
	public void setLevel(int levelNumber){
		currentLevel = levelNumber;
		ModeMenu ();
	}
	
	public void Tutorial(){
		currentMode = 0;
		Application.LoadLevel ("Eliot Tutorial");
	}
	
	public void SettingsApply(){
		GetComponent<SettingsController>().Save ();
		updateSettings ();
		MainMenu ();
	}
	
	public void SettingsRevert(){
		GetComponent<SettingsController>().Load ();
	}
	
	public void updateSettings(){
		mainmenuSource.volume = GetComponent<SettingsController>().masterVolume / 10f * GetComponent<SettingsController>().musicVolume / 10f;
	}
	
}


