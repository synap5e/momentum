using UnityEngine;
using System.Collections;
using System;
using System.IO;
using SimpleJSON;

public class Pause : MonoBehaviour {
	
	public GameObject pauseMenu;
	public GameObject quitMenu;
	public GameObject settingsMenu;
	
	public GameObject pauseMenuBG;
	public GameObject quitMenuBG;
	public GameObject settingsMenuBG;
	
	public GameObject player;
	public GameObject gameController;

	public GameObject feet;
	private GameObject hands;

	private Camera camera;
	
	public bool isPause;
	private bool inQuitMenu = false;
	private bool inSettingsMenu = false;
	private bool viewmodelOn = true;

	public GameObject fovSlider;
	public GameObject sensSlider;
	public GameObject viewmodelToggle;
		
	public UnityEngine.UI.Text sensitivityText;

	private string filename = "settings.json";
	
	
	void Start () {
		pauseMenuBG.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.90f);
		quitMenuBG.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.90f);
		settingsMenuBG.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.90f);
		isPause = false;
		settingsMenu.SetActive (false);
		pauseMenu.SetActive(false);
		quitMenu.SetActive (false);
		hands = GameObject.FindGameObjectWithTag ("Hands");
		camera = GameObject.FindObjectOfType <Camera> ().GetComponent<Camera> ();
		Load (filename);
	}
	
	void Update () {
		if( Input.GetKeyDown(KeyCode.Escape))
		{			
			if(!isPause){
				PausePlay();
				isPause = !isPause;
			}
			else if(inQuitMenu){
				QuitGameNo();
			}
			else if(inSettingsMenu){
				SettingsRevert();
				SettingsReturn();
			}
			
			else{
				ResumePlay();
			}
		}
	}
	
	public void PausePlay(){
		Time.timeScale = 0;
		pauseMenu.SetActive (true);
		player.GetComponent<RigidbodyFPSController> ().disableMouse = false;
		player.GetComponent<RigidbodyFPSController> ().enableInput = false;
		player.GetComponent<Recorder> ().StopRecording ();
		player.GetComponent<AudioController> ().PauseAudio ();
		Goal.paused = true;
		Screen.lockCursor = false; // Unity 5 Cursor is bugged
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;
	}
	
	public void ResumePlay(){
		Time.timeScale = 1;
		pauseMenu.SetActive(false);
		player.GetComponent<RigidbodyFPSController> ().disableMouse = true;
		player.GetComponent<RigidbodyFPSController> ().enableInput = true;
		player.GetComponent<Recorder> ().StartRecording ();
		player.GetComponent<AudioController> ().PlayAudio ();
		Goal.paused = false;
		isPause = false;
	}
	
	public void RestartLevel(){
		player.GetComponent<Recorder> ().ResetRecording ();
	}
	
	public void SettingsMenu(){
		pauseMenu.SetActive (false);
		settingsMenu.SetActive (true);
		inSettingsMenu = true;
	}
	public void SettingsReturn(){
		pauseMenu.SetActive (true);
		settingsMenu.SetActive (false);
		inSettingsMenu = false;
	}

	public void SettingsApply(){
		Save (filename);
		SettingsReturn ();
	}

	public void SettingsRevert(){
		Load (filename);
	}
	
	public void QuitMenu(){
		pauseMenu.SetActive (false);
		quitMenu.SetActive (true);
		inQuitMenu = true;
	}
	
	public void QuitGameYes(){
		Application.Quit();
		Debug.Log ("Quit");
	}
	
	public void QuitGameNo(){
		pauseMenu.SetActive (true);
		quitMenu.SetActive (false);
		inQuitMenu = false;
	}

	public void MainMenu(){
		Time.timeScale = 1;
		Application.LoadLevel ("Main Menu");
		Goal.paused = false;
	}

	public void ChangeFov(float fov){
		camera.fieldOfView = fov;
	}

	public void showViewModel(bool show){
		viewmodelOn = show;
		hands.SetActive (show);
		feet.SetActive (show);
	}

	public void ChangeSensitivity(float sens){
		player.GetComponent<RigidbodyFPSController> ().changeSensitivity (sens);		
		sensitivityText.text =sens.ToString("#.#");
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
		fovSlider.GetComponent<UnityEngine.UI.Slider> ().value = camera.fieldOfView;
		sensSlider.GetComponent<UnityEngine.UI.Slider> ().value = player.GetComponent<RigidbodyFPSController> ().mouseSensitivity;
		viewmodelToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn = viewmodelOn;
	}

	public string SaveToString()
	{
		JSONNode N = new JSONClass(); // Start with JSONArray or JSONClass
		N["settings"]["fov"].AsFloat = camera.fieldOfView;
		N["settings"]["sensitivity"].AsFloat = player.GetComponent<RigidbodyFPSController> ().mouseSensitivity;
		N["settings"]["viewmodel"].AsFloat = viewmodelOn ? 1 : 0;
		return N.ToJSON(4);
	}

	public void LoadString(String text)
	{
		JSONNode N = JSON.Parse(text);
		ChangeFov (N ["settings"] ["fov"].AsFloat); 
		ChangeSensitivity(N["settings"]["sensitivity"].AsFloat);
		viewmodelOn = false;
		if (N["settings"]["viewmodel"].AsFloat == 1) viewmodelOn = true;
		showViewModel(viewmodelOn);
	}
	
}
