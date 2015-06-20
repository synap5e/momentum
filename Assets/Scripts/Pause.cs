using UnityEngine;
using System.Collections;
using System;
using System.IO;
using SimpleJSON;
using UnityStandardAssets.ImageEffects;

public class Pause : MonoBehaviour {
	
	[Header("Menu")]
	public GameObject pauseMenu;
	public GameObject quitMenu;
	public GameObject settingsMenu;
	public GameObject audioMenu;
	public GameObject videoMenu;
	public GameObject controlMenu;
	public GameObject endLevelMenu;
	public GameObject nextLevelMenu;
	
	public UnityEngine.UI.Text timeText;
	
	[Header("Menu BG")]
	public GameObject background;
	
	[Header("GameObjects")]
	public GameObject player;
	public GameObject gameController;
	public GameObject feet;
	
	private GameObject camera;
	
	private GameObject hands;
	
	private bool isPause;
	
	public enum menuNames {PauseMenu, QuitMenu,SettingsMenu,AudioMenu,ControlMenu,VideoMenu,EndLevelMenu};
	
	void Start () {
		//Sets the BG to black with opacity of 90%
		background.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.90f);
		
		//Set menus for false
		pauseMenu.SetActive(false);
		settingsMenu.SetActive (false);
		quitMenu.SetActive (false);
		controlMenu.SetActive (false);
		audioMenu.SetActive (false);
		videoMenu.SetActive (false);
		endLevelMenu.SetActive (false);
		background.SetActive (false);
		
		hands = GameObject.FindGameObjectWithTag ("Hands");
		camera = GameObject.FindGameObjectWithTag ("MainCamera");
		
		//Load Settings from file
		GetComponent<SettingsController>().Load ();
		updateSettings ();
	}
	
	void Update () {
		if( Input.GetKeyDown(KeyCode.Escape))
		{		
			if(quitMenu.activeSelf || controlMenu.activeSelf){
				PauseMenu();
			}
			else if(audioMenu.activeSelf || settingsMenu.activeSelf || videoMenu.activeSelf){
				SettingsRevert();
				PauseMenu();
			}
			else if(pauseMenu.activeSelf){
				ResumePlay();
			}
			else if(endLevelMenu.activeSelf){
				//Do nothing
			}
			else {
				PausePlay();
				isPause = !isPause;
			}			
			
		}
	}
	
	public void PausePlay(){
		Time.timeScale = 0;
		PauseMenu ();
		background.SetActive (true);
		player.GetComponent<RigidbodyFPSController> ().disableMouse = false;
		player.GetComponent<RigidbodyFPSController> ().enableInput = false;
		player.GetComponent<Recorder> ().StopRecording ();
		player.GetComponent<AudioController> ().PauseAudio ();
		Goal.paused = true;
		//		Screen.lockCursor = false; // Unity 5 Cursor is bugged
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;
	}
	
	public void ResumePlay(){
		Time.timeScale = 1;
		pauseMenu.SetActive(false);
		endLevelMenu.SetActive (false);
		background.SetActive (false);
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
	
	public void EndLevel(){
		PausePlay ();
		setMenuActive (menuNames.EndLevelMenu);
		
		if (MainMenu_Controller.currentLevel == 3)
			nextLevelMenu.SetActive(false);
		else
			nextLevelMenu.SetActive(true);
		
	}
	
	public void NextLevel(){
		ResumePlay();
		MainMenu_Controller.currentLevel++;
		if (MainMenu_Controller.currentLevel == 1)
			Application.LoadLevel ("Eliot Easy");
		else if (MainMenu_Controller.currentLevel == 2)
			Application.LoadLevel ("Regan_Medium");
		else if (MainMenu_Controller.currentLevel == 3)
			Application.LoadLevel ("Eliot Hard");
		else 
			Debug.Log ("Missing level in Goal");

	}
	
	public void SettingsMenu(){
		setMenuActive (menuNames.SettingsMenu);
	}
	
	public void AudioMenu(){
		setMenuActive (menuNames.AudioMenu);
	}
	
	public void VideoMenu(){
		setMenuActive (menuNames.VideoMenu);
	}
	
	public void ControlMenu(){
		setMenuActive (menuNames.ControlMenu);
	}
	
	public void PauseMenu(){
		setMenuActive (menuNames.PauseMenu);
	}
	
	public void MainMenu(){
		Time.timeScale = 1;
		Application.LoadLevel ("Main Menu");
		Goal.paused = false;
	}
	
	public void setEndLevelText(string s){
		timeText.text = s;
		Debug.Log (s);
	}
	
	public void SettingsApply(){
		GetComponent<SettingsController>().Save ();		
		updateSettings ();
		PauseMenu ();
	}
	
	public void SettingsRevert(){
		GetComponent<SettingsController>().Load ();
	}
	
	public void updateSettings(){
		GetComponent<AudioController> ().changeVolume ();
		
		GameObject.FindObjectOfType <Camera> ().fieldOfView = GetComponent<SettingsController>().fov;
		hands.SetActive (GetComponent<SettingsController>().viewModelOn);
		feet.SetActive (GetComponent<SettingsController>().viewModelOn);
		player.GetComponent<RigidbodyFPSController> ().changeSensitivity (GetComponent<SettingsController>().sensitivity);	
		
		camera.GetComponent<ScreenSpaceAmbientOcclusion>().enabled = GetComponent<SettingsController>().ambientOcclusion;
		camera.GetComponent<VignetteAndChromaticAberration> ().enabled = GetComponent<SettingsController>().vignetteAberration;
		camera.GetComponent<EdgeDetection> ().enabled = GetComponent<SettingsController>().edgeDetection;
		camera.GetComponent<CameraMotionBlur> ().enabled = GetComponent<SettingsController>().motionBlur;
		camera.GetComponent<Bloom> ().enabled = GetComponent<SettingsController>().bloom;
	}
	
	public void QuitMenu(){
		setMenuActive (menuNames.QuitMenu);
	}
	
	public void QuitGameYes(){
		Application.Quit();
		Debug.Log ("Quit");
	}
	
	//Sets all menus to false except the currentMenu
	public void setMenuActive(menuNames currentMenu){
		
		//set all menus to false
		pauseMenu.SetActive (false);
		quitMenu.SetActive (false);
		settingsMenu.SetActive (false);
		audioMenu.SetActive(false);
		videoMenu.SetActive (false);
		controlMenu.SetActive(false);
		endLevelMenu.SetActive(false);
		
		switch(currentMenu)
		{
		case menuNames.PauseMenu:
			pauseMenu.SetActive (true);
			break;
		case menuNames.ControlMenu:
			controlMenu.SetActive (true);
			break;
		case menuNames.QuitMenu:
			quitMenu.SetActive(true);
			break;
		case menuNames.AudioMenu:
			audioMenu.SetActive(true);
			break;
		case menuNames.VideoMenu:
			videoMenu.SetActive(true);
			break;
		case menuNames.SettingsMenu:
			settingsMenu.SetActive(true);
			break;
		case menuNames.EndLevelMenu:
			endLevelMenu.SetActive(true);
			break;
		default:
			Debug.Log("missing menu in Pause");
			break;
		}
	}
}