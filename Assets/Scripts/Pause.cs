using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

	public GameObject pauseMenu;
	public GameObject quitMenu;
	public GameObject settingsMenu;
	public GameObject pauseMenuBG;
	public GameObject quitMenuBG;
	public GameObject settingsMenuBG;
	public GameObject player;
	public GameObject gameController;
	public bool isPause;


	void Start () {
		pauseMenuBG.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.90f);
		quitMenuBG.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.90f);
		isPause = false;
		pauseMenu.SetActive(false);
		quitMenu.SetActive (false);
	}

	void Update () {
		if( Input.GetKeyDown(KeyCode.Escape))
		{
			isPause = !isPause;
			if(isPause){
				PausePlay();
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
		isPause = false;
	}

	public void RestartLevel(){
		player.GetComponent<Recorder> ().ResetRecording ();
	}

	public void SettingsMenu(){
		pauseMenu.SetActive (false);
		settingsMenu.SetActive (true);
	}
	public void SettingsReturn(){
		pauseMenu.SetActive (true);
		settingsMenu.SetActive (false);
	}

	public void QuitMenu(){
		pauseMenu.SetActive (false);
		quitMenu.SetActive (true);
	}

	public void QuitGameYes(){
		Application.Quit();
		Debug.Log ("Quit");
	}

	public void QuitGameNo(){
		pauseMenu.SetActive (true);
		quitMenu.SetActive (false);
	}

}
