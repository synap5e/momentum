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

	public AudioClip music1;
	public AudioClip music2;
	public AudioClip music3;
	public AudioClip music4;
	public AudioClip music5;
	public AudioClip music6;
	public AudioClip music7;
	public AudioClip music8;

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

	private AudioSource musicSource;
	private int playlistIndex =0;

	private Dictionary <AudioSource,float> audiosourceDic;
	private List<AudioClip> musicPlaylist;

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

		musicPlaylist.Add (music1);
		musicPlaylist.Add (music2);
		musicPlaylist.Add (music3);
		musicPlaylist.Add (music4);
		musicPlaylist.Add (music5);
		musicPlaylist.Add (music6);
		musicPlaylist.Add (music7);
		musicPlaylist.Add (music8);
		Shuffle (musicPlaylist);
		musicSource = player.AddComponent<AudioSource>();
		musicSource.clip = musicPlaylist [0];
		Debug.Log ("playing " + musicPlaylist [0].ToString ());
		musicSource.loop = false;

		GetComponent<SettingsController>().Load ();
		changeVolume ();
	}

	public AudioController(){		
		audiosourceDic = new Dictionary <AudioSource,float> ();
		musicPlaylist = new List<AudioClip> ();
	}

	void Shuffle<T>(List<T> list) {
		System.Random random = new System.Random();
		int n = list.Count;
		while (n > 1) {
			int k = random.Next(n);
			n--;
			T temp = list[k];
			list[k] = list[n];
			list[n] = temp;
		}
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

			if(!musicSource.isPlaying){
				playlistIndex++;
				if(playlistIndex == musicPlaylist.Count){
					playlistIndex =0;
					Shuffle (musicPlaylist);
				}
				musicSource.clip = musicPlaylist[playlistIndex];
				musicSource.Play ();
			}
		}
	}

	public void PlayAudio(){
		playAudio = true;
		musicSource.UnPause ();
	}

	public void PauseAudio(){
		playAudio = false;
		beepsource.Pause();
		explosionsource.Pause();
		jumpingSource.Pause();
		landingSource.Pause();
		pantingSource.Pause();
		runningSource.Pause();
		musicSource.Pause ();
	}

	public void changeVolume(){
		float masterVolume = GetComponent<SettingsController>().masterVolume/10f;
		float soundEffectsVolume = GetComponent<SettingsController>().soundEffectsVolume/10f;
		float musicVolume = GetComponent<SettingsController>().musicVolume/10f;
		foreach (AudioSource a in audiosourceDic.Keys) 
			a.volume = masterVolume * soundEffectsVolume * audiosourceDic[a];
		musicSource.volume = masterVolume * musicVolume;
	}


}
