using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using SimpleJSON;

public class Recorder : MonoBehaviour {

	public float recordingFrameDurationMin;
	public List<Snapshot> snapshotList {get; set;}
	private bool recording = false;

	public bool recordKeys = true;
	public GameObject player;
	private Rigidbody playerRigidbody;
	private RigidbodyFPSController rigidbodyFPSController;
	private float frameDuration;
	private String fileName = "Keylog.txt";

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
		public float playerRotation{ get; set;}
		public float cameraRotation{ get; set;}

		public Snapshot(Vector3 position,Quaternion rotation,bool inJump, float duration,float playerRotation,float cameraRotation){
			this.position = position;
			this.rotation = rotation;
			this.inJump = inJump;
			this.duration = duration;
			this.playerRotation = playerRotation;
			this.cameraRotation = cameraRotation;
		}
	}

	void Start () {
		snapshotList = new List<Snapshot> ();
		File.CreateText (fileName);
		playerRigidbody = player.GetComponent<Rigidbody>();
		rigidbodyFPSController = player.GetComponent<RigidbodyFPSController>();
		Save ();
	}


	void Update(){
		if (recording) 
			RecordSnapshot();

		if(recordKeys)
			RecordKeys ();
	}

	public void RecordSnapshot(){
		frameDuration += Time.deltaTime;
		if (frameDuration > recordingFrameDurationMin) {
			Vector3 position = playerRigidbody.position;
			Quaternion rotation = playerRigidbody.rotation;
			bool inJump = rigidbodyFPSController.onGround;
			float duration = frameDuration;
			float playerRotation = player.transform.localEulerAngles.y;
			Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
			float cameraRotation = camera.transform.localEulerAngles.x;
			
			//Add the snapshot to the list
			Snapshot s = new Snapshot (position, rotation, inJump, duration,playerRotation,cameraRotation);
			
			//Checks snapshotList is initialised for hot swapping
			if(snapshotList ==null)
				snapshotList = new List<Snapshot> ();
			snapshotList.Add (s);
			
			//reset frame duration
			frameDuration = 0;
		}
	}

	public void RecordKeys(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			File.AppendAllText(fileName, DateTime.Now.ToString() + " Space" +"\n");
		}
		else if (Input.GetKeyDown (KeyCode.LeftShift)) {
			File.AppendAllText(fileName, DateTime.Now.ToString() + " LeftShift" +"\n");
		}
		else if (Input.anyKeyDown) {
			File.AppendAllText(fileName, DateTime.Now.ToString() + " "+Input.inputString +"\n");
		}
	}

	public void StartRecording(){
		recording = true;
	}

	public void StopRecording(){
		recording = false;
	}

	public void ResetRecording(){
		snapshotList = new List<Snapshot> ();
		recording = true;
	}

	public void StartLoggingKeys(){
		recordKeys = true;
	}
	
	public void StopLoggingKeys(){
		recordKeys = false;
	}

	public void ResetLoggingKeys(){
		File.CreateText (fileName);
		recordKeys = true;
	}

	public void Save(){
		JSONNode N = new JSONClass (); // Start with JSONArray or JSONClass

		N ["player recording"] ["total duration"].AsInt = 11;
		N ["player recording"] ["number frames"].AsInt = 400;
		N ["player recording"] ["frames"] [0] ["duration"].AsFloat = 0.023f;
		N ["player recording"] ["frames"] [0] ["position"][0].AsInt = 4;
		N ["player recording"] ["frames"] [0] ["position"][1].AsInt = 4; 
		N ["player recording"] ["frames"] [0] ["position"][2].AsInt = 6;  
		N ["player recording"] ["frames"] [0] ["inJump"].AsInt = 0;
		N ["player recording"] ["frames"] [0] ["y rotation"].AsFloat = 45f;
		N ["player recording"] ["frames"] [0] ["camera tilt"].AsFloat = 11f;
		N ["player recording"] ["frames"] [0] ["key events"] [0] ["time"].AsFloat = 0.008f;
		N ["player recording"] ["frames"] [0] ["key events"] [0] ["type"] = "keyup";
		N ["player recording"] ["frames"] [0] ["key events"] [0] ["key"].AsInt = 32;
		N ["player recording"] ["frames"] [0] ["key events"] [1] ["time"].AsFloat = 0.008f;
		N ["player recording"] ["frames"] [0] ["key events"] [1] ["type"] = "keyup";
		N ["player recording"] ["frames"] [0] ["key events"] [1] ["key"].AsInt = 32;

		Debug.Log (N.ToJSON (4));
	}

}
