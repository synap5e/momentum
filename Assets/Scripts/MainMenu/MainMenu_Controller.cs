using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.IO;
using SimpleJSON;

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
	public GameObject audioMenu;
	public GameObject skipMenu;

	public UnityEngine.UI.Text sensitivityText;

	public GameObject fovSlider;
	public GameObject sensSlider;
	public GameObject viewmodelToggle;

	public GameObject masterSlider;
	public GameObject musicSlider;
	public GameObject soundEffectSlider;
	
	private string filename = "settings.json";

	private float fov = 60;
	private float sensitivity = 2;
	private bool viewModelOn = true;

	private float masterVolume = 10f;
	private float musicVolume = 10f;
	private float soundEffectsVolume = 10f;

	private AudioSource mainmenuSource;
	
	//	private bool inModeMenu = false;
	private bool fadedIn = false;
	private int currentLevel = 0;
	static public int currentMode = 0; // 0 = Normal and 1 = SpeedRun
	public enum menuNames {MainMenu, ModeMenu,LevelSelectMenu,CreditsMenu,SettingsMenu,AudioMenu};


	// Use this for initialization
	void Awake () {
		fader.gameObject.SetActive (true);
		
		mainMenu.SetActive(false);
		modeMenu.SetActive (false);
		levelSelectMenu.SetActive (false);
		creditsMenu.SetActive (false);
		settingsMenu.SetActive (false);
		audioMenu.SetActive(false);
		skipMenu.SetActive (true);
		
		mainmenuSource = GetComponent<AudioSource> ();
		
		fader = GameObject.Find("ScreenFader");
		fader.GetComponent<UnityEngine.UI.RawImage> ().CrossFadeAlpha (0f,.5f, true);
		Load (filename);
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.Escape))
		{			
			if(settingsMenu.activeSelf){
				MainMenu();
				SettingsRevert();
			}
			else if(creditsMenu.activeSelf){
				MainMenu();
			}
			else if(levelSelectMenu.activeSelf){
				MainMenu();
			}
			else if(audioMenu.activeSelf){
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

	public void AudioMenu(){
		setMenuActive (menuNames.AudioMenu);
	}

	public void setMenuActive(menuNames currentMenu){

		//set all menus to false
		creditsMenu.SetActive (false);
		levelSelectMenu.SetActive (false);
		mainMenu.SetActive (false);
		modeMenu.SetActive (false);
		settingsMenu.SetActive (false);
		audioMenu.SetActive(false);

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
		case menuNames.AudioMenu:
			audioMenu.SetActive(true);
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

	public void SettingsApply(){
		Save (filename);
		MainMenu ();
	}
	
	public void SettingsRevert(){
		Load (filename);
	}

	public void Save(String filename){
		File.WriteAllText(filename, SaveToString());
	}
	
	public void Load(String filename){
		if (!File.Exists (filename)) {
			Save (filename);
		}
		string text = File.ReadAllText(filename);
		LoadString(text);
		fovSlider.GetComponent<UnityEngine.UI.Slider> ().value = fov;
		sensSlider.GetComponent<UnityEngine.UI.Slider> ().value = sensitivity;
		viewmodelToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn = viewModelOn;
		masterSlider.GetComponent<UnityEngine.UI.Slider> ().value = masterVolume;
		musicSlider.GetComponent<UnityEngine.UI.Slider> ().value = musicVolume;
		soundEffectSlider.GetComponent<UnityEngine.UI.Slider> ().value = soundEffectsVolume;
		changeVolume ();

	}
	
	public string SaveToString()
	{
		JSONNode N = new JSONClass(); // Start with JSONArray or JSONClass
		N["settings"]["fov"].AsFloat = fov;
		N["settings"]["sensitivity"].AsFloat = sensitivity;
		N["settings"]["viewmodel"].AsFloat = viewModelOn ? 1 : 0;

		N["audio"]["master"].AsFloat = masterVolume;
		N["audio"]["music"].AsFloat = musicVolume;
		N["audio"]["sound effects"].AsFloat = soundEffectsVolume;
		return N.ToJSON(4);
	}
	
	public void LoadString(String text)
	{
		JSONNode N = JSON.Parse(text);
		fov = N["settings"]["fov"].AsFloat;
		sensitivity = N["settings"]["sensitivity"].AsFloat;
		viewModelOn = false;
		if (N["settings"]["viewmodel"].AsFloat == 1) viewModelOn = true;
		
		masterVolume = N["audio"]["master"].AsFloat;
		musicVolume = N["audio"]["music"].AsFloat;
		soundEffectsVolume = N["audio"]["sound effects"].AsFloat;
	}

	public void changeFOV(float newFov){
		fov = newFov;
	}

	public void changeSensitivity(float newSens){
		sensitivity = newSens;		
		sensitivityText.text =newSens.ToString("#.#");
	}

	public void toggleViewModel(bool isOn){
		viewModelOn = isOn;
	}

	public void changeMaster(float newMaster){
		masterVolume = newMaster;
	}

	public void changeMusic(float newMusic){
		musicVolume = newMusic;
	}

	public void changeSoundEffects(float newSound){
		soundEffectsVolume = newSound;
	}

	public void changeVolume(){
		mainmenuSource.volume = masterVolume / 10f * musicVolume / 10f;
	}
}



