using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

	public GameObject player;
	
	public AudioClip beep;
	public AudioClip explosion;
	public AudioClip jumping;
	public AudioClip landing;
	public AudioClip panting;
	public AudioClip runnning;

	private bool playAudio = true;
	private bool inJump = false;
	private float startTime;

	private AudioSource beepsource;
	private AudioSource explosionsource;
	private AudioSource jumpingSource;
	private AudioSource landingSource;
	private AudioSource pantingSource;
	private AudioSource runningSource;

	// Use this for initialization
	void Start () {
		jumpingSource = player.AddComponent<AudioSource>();
		jumpingSource.clip = jumping;
		jumpingSource.volume = 0.5f;
		
		landingSource = player.AddComponent<AudioSource>();
		landingSource.clip = landing;
		landingSource.volume = 0.5f;
		
		pantingSource = player.AddComponent<AudioSource>();
		pantingSource.clip = panting;
		pantingSource.volume = 1f;
		pantingSource.loop = true;
		
		runningSource = player.AddComponent<AudioSource>();
		runningSource.clip = runnning;
		runningSource.volume = 0.5f;
		runningSource.loop = false;

		beepsource = player.AddComponent<AudioSource>();
		beepsource.clip = beep;
		beepsource.volume = 0.1f;
		
		explosionsource = player.AddComponent<AudioSource>();
		explosionsource.clip = explosion;
		explosionsource.volume = 0.5f;
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
			else if (moving && !inJump && !runningSource.isPlaying){
				pantingSource.Stop ();
				runningSource.Play ();
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
}
