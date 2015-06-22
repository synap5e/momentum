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
	public AudioClip whoosh;

	private bool playAudio = true;
	private bool inJump = false;
	private float startTime;
	private bool playLand = false;
	private float whooshTime;
	private bool playWhoosh = false;

	private AudioSource beepsource;
	private AudioSource explosionsource;
	private AudioSource jumpingSource;
	private AudioSource landingSource;
	private AudioSource pantingSource;
	private AudioSource runningSource;
	private AudioSource whooshSource;

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

		whooshSource = player.AddComponent<AudioSource>();
		whooshSource.clip = whoosh;
		whooshSource.loop = true;
		audiosourceDic.Add (whooshSource,0.1f);

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


			//If in jump by pressing jump key
			if (Input.GetKeyDown (KeyCode.Space) &&!inJump) {
				jumpingSource.Play ();
				runningSource.Stop ();
				inJump = true;
				startTime = Time.time;
			}

			//If jumped more than 0.1 sec ago
			if(inJump && jumping && Time.time - startTime > 0.1f){
				playLand = true;
			}

			//If injump and landed
			if(inJump && !jumping && Time.time - startTime > 0.1f){	
				if(playLand)
					landingSource.Play();
				inJump = false;
				playLand = false;
			}

			//If standing on the ground - not moving and not inJump
			if (moving && !inJump && !jumping && !runningSource.isPlaying ){
				runningSource.Play ();
				pantingSource.Stop ();
			}

			//if jumping and moving stop running 
			if(moving && jumping)
				runningSource.Stop ();	

			//If standing on the ground - not moving and not inJump
			if(!moving && !inJump && !pantingSource.isPlaying){
				pantingSource.Play ();
				runningSource.Stop ();
			}

			//If near bomb
			if (bombAc.nearBomb ()) {
				if (Input.GetMouseButtonDown (0)) {
					beepsource.Play ();
					explosionsource.Play ();
				}
			}

			//if faster than 14units
			if(player.GetComponent<RigidbodyFPSController>().currentSpeed > 18f){	
				float volume = (Time.time - whooshTime);
				if(volume<1f){
					audiosourceDic[whooshSource] = volume;
					changeVolume();
				}
				if(!whooshSource.isPlaying){
					whooshTime = Time.time;
					whooshSource.Play ();
				}	
				playWhoosh = true;
			}
			else{
				/*(if(playWhoosh){
					playWhoosh = false;
					whooshTime = Time.time;
				}	
				float volume = 1f-(Time.time - whooshTime);
				if(volume>0.1f){
					audiosourceDic[whooshSource] = volume;
					changeVolume();
				}
				else
					whooshSource.Stop ();
				Debug.Log(volume); */

				whooshSource.Stop ();
			}
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
