using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioController : MonoBehaviour {

	public GameObject player;
	
	public AudioClip beep;
	public AudioClip explosion;
	public AudioClip jumping;
	public AudioClip landing;
	public AudioClip panting;
	public AudioClip running;

	private bool playAudio = true;
	private bool inJump = false;
	private float startTime;

	private AudioSource beepsource;
	private AudioSource explosionsource;
	private AudioSource jumpingSource;
	private AudioSource landingSource;
	private AudioSource pantingSource;
	private AudioSource runningSource;

	private Dictionary <AudioSource,float> audiosourceDic;

	// Use this for initialization
	void Start () {

		jumpingSource = player.AddComponent<AudioSource>();
		jumpingSource.clip = jumping;
		audiosourceDic.Add (jumpingSource,0.5f);
		
		landingSource = player.AddComponent<AudioSource>();
		landingSource.clip = landing;
		audiosourceDic.Add (landingSource,0.5f);
		
		pantingSource = player.AddComponent<AudioSource>();
		pantingSource.clip = panting;
		pantingSource.loop = true;
		audiosourceDic.Add (pantingSource,1f);
		
		runningSource = player.AddComponent<AudioSource>();
		runningSource.clip = running;
		runningSource.loop = false;
		audiosourceDic.Add (runningSource,0.5f);

		beepsource = player.AddComponent<AudioSource>();
		beepsource.clip = beep;
		audiosourceDic.Add (beepsource,0.1f);
		
		explosionsource = player.AddComponent<AudioSource>();
		explosionsource.clip = explosion;
		audiosourceDic.Add (explosionsource,1f);

		GetComponent<SettingsController>().Load ();
		changeVolume ();
	}

	public AudioController(){		
		audiosourceDic = new Dictionary <AudioSource,float> ();
	}
	
	void FixedUpdate () {

		if (playAudio) {
			bool jumping = !GetComponent<RigidbodyFPSController> ().onGround;
			bool moving = Input.GetAxis ("Horizontal") != 0f || Input.GetAxis ("Vertical") != 0f;
			BombActivator bombAc = player.GetComponent<BombActivator> ();
			if (Input.GetKeyDown (KeyCode.Space)) {
				jumpingSource.Play ();
				runningSource.Stop ();
				inJump = true;
				startTime = Time.time;
			}
			else if(Time.time - startTime > 1f && inJump && !jumping){
				landingSource.Play();
				inJump = false;
			}
			else if (moving && !inJump && !jumping && !runningSource.isPlaying ){
				pantingSource.Stop ();
				runningSource.Play ();
			}
			else if (moving && !inJump && jumping){
				runningSource.Stop ();
			}
			else if(!moving && !pantingSource.isPlaying){
				runningSource.Stop ();
				pantingSource.Play ();
			}
			else if (bombAc.nearBomb ()) {
				if (Input.GetMouseButtonDown (0)) {
					beepsource.Play ();
					explosionsource.Play ();
				}
			}
//			else if(player.GetComponent<RigidbodyFPSController>().speed > 20f && !whooshSource.isPlaying){
//				whooshSource.Play ();
//			}

		}
	}

	public void PlayAudio(){
		playAudio = true;
	}

	public void PauseAudio(){
		playAudio = false;
		beepsource.Pause();
		explosionsource.Pause();
		jumpingSource.Pause();
		landingSource.Pause();
		pantingSource.Pause();
		runningSource.Pause();
	}

	public void changeVolume(){
		foreach (AudioSource a in audiosourceDic.Keys) {
			float masterVolume = GetComponent<SettingsController>().masterVolume/10f;
			float soundEffectsVolume = GetComponent<SettingsController>().soundEffectsVolume/10f;
			a.volume = masterVolume * soundEffectsVolume * audiosourceDic[a];
		}
	}


}
