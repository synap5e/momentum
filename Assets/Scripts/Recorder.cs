using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Recorder : MonoBehaviour {

	public float recordingFrameDurationMin;
	public List<Snapshot> snapshotList {get; set;}
	private bool recording = false;


	public GameObject player;
	private Rigidbody playerRigidbody;
	private RigidbodyFPSController rigidbodyFPSController;

	private float frameDuration;

	public bool IsRecording
	{
		get
		{
			return recording;
		}
	}

	public class Snapshot{
		public Vector3 position{ get; set;}
		public Quaternion rotation{get; set;}
		public bool inJump{get; set;}
		public float duration{ get; set;}

		public Snapshot(Vector3 position,Quaternion rotation,bool inJump, float duration){
			this.position = position;
			this.rotation = rotation;
			this.inJump = inJump;
			this.duration = duration;
		}
	}

	public Recorder()
	{
		snapshotList = new List<Snapshot>();
	}

	void Start () {
		playerRigidbody = player.GetComponent<Rigidbody>();
		rigidbodyFPSController = player.GetComponent<RigidbodyFPSController>();
	}


	void Update(){
		if (recording) {
			frameDuration += Time.deltaTime;
			if (frameDuration > recordingFrameDurationMin) {
				Vector3 position = playerRigidbody.position;
				Quaternion rotation = playerRigidbody.rotation;
				bool inJump = !rigidbodyFPSController.onGround;
				float duration = frameDuration;

				//Add the snapshot to the list
				Snapshot s = new Snapshot (position, rotation, inJump, duration);
				snapshotList.Add (s);

				//reset frame duration
				frameDuration = 0;
			}
		}		
	}

	public void StartRecording()
	{
		recording = true;
	}

	public void StopRecording(){
		recording = false;
	}

}
