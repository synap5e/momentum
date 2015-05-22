using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

	public GameObject pauseMenu;
	public GameObject quitMenu;
	public GameObject image;
	public GameObject player;


	public bool isPause;
	// Use this for initialization
	void Start () {
		image.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.90f);
		isPause = false;
		pauseMenu.SetActive(false);
		quitMenu.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		image.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.90f);
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
		pauseMenu.SetActive(true);
		player.GetComponent<RigidbodyFPSController> ().disableMouse = false;
		player.GetComponent<RigidbodyFPSController> ().enableInput = false;
		Screen.lockCursor = false; // Unity 5 Cursor is bugged
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;
	}

	public void ResumePlay(){
		Time.timeScale = 1;
		pauseMenu.SetActive(false);
		player.GetComponent<RigidbodyFPSController> ().disableMouse = true;
		player.GetComponent<RigidbodyFPSController> ().enableInput = true;
		isPause = false;
	}

	public void QuitGame(){
		Application.Quit();
	}


}
