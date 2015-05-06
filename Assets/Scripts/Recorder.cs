using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using SimpleJSON;

public class Recorder : MonoBehaviour {

	public float recordingFrameDurationMin;
	public List<Snapshot> snapshotList {get; set;}
	public List<KeyRecording> keylist {get; set;}
	private bool recording = false;
	public bool recordKeys = true;
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

	public Recorder(){
		keylist = new List<KeyRecording>();
		snapshotList = new List<Snapshot> ();
	}

	public class Snapshot{
		public Vector3 position{ get; set;}
		public Quaternion rotation{get; set;}
		public bool inJump{get; set;}
		public float duration{ get; set;}
		public float playerRotation{ get; set;}
		public float cameraRotation{ get; set;}
		public List<KeyRecording> keyRecList { get; set;}

		public Snapshot(Vector3 position,Quaternion rotation,bool inJump, float duration,float playerRotation,float cameraRotation,List<KeyRecording> keyRecList){
			this.position = position;
			this.rotation = rotation;
			this.inJump = inJump;
			this.duration = duration;
			this.playerRotation = playerRotation;
			this.cameraRotation = cameraRotation;
			this.keyRecList = keyRecList;
		}
	}

	public class KeyRecording{
		public float time{get; set;}
		public String type{get; set;}
		public String key{get; set;}
		public KeyRecording(float time, String type, String key){
			this.time = time;
			this.type = type;
			this.key = key;
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
		if (recording) 
			RecordSnapshot();
	}

	public void RecordSnapshot(){
		frameDuration += Time.deltaTime;
		if (frameDuration > recordingFrameDurationMin) {
			Vector3 position = playerRigidbody.position;
			Quaternion rotation = playerRigidbody.rotation;
			bool inJump = rigidbodyFPSController.onGround;
			float duration = frameDuration;
			float playerRotation = player.transform.localEulerAngles.y;
			Camera camera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
			float cameraRotation = camera.transform.localEulerAngles.x;
			
			//Add the snapshot to the list
			Snapshot s = new Snapshot (position, rotation, inJump, duration, playerRotation, cameraRotation,keylist);

			snapshotList.Add (s);
			
			//reset frame duration
			frameDuration = 0;

			//reset keylist
			keylist = new List<KeyRecording> ();
		} 
		else {//Still within frame so record keys
			if(recordKeys){
				if (Input.GetKeyDown (KeyCode.Space)) {
					keylist.Add (new KeyRecording(Time.deltaTime,"keyDown","Space"));
				}
				else if(Input.GetMouseButtonDown(0)){
					keylist.Add (new KeyRecording(Time.deltaTime,"keyDown","MouseButton"));
				}
				else if (Input.anyKeyDown) {
					keylist.Add (new KeyRecording(Time.deltaTime,"keyDown",Input.inputString));
				}
				if (Input.GetKeyUp (KeyCode.Space)) {
					keylist.Add (new KeyRecording(Time.deltaTime,"keyUp","Space"));
				}
				else if(Input.GetMouseButtonUp(0)){
					keylist.Add (new KeyRecording(Time.deltaTime,"keyUp","MouseButton"));
				}
				else if (Input.GetKeyUp(KeyCode.W)) {
					keylist.Add (new KeyRecording(Time.deltaTime,"keyUp","w"));
				}
				else if (Input.GetKeyUp(KeyCode.A)) {
					keylist.Add (new KeyRecording(Time.deltaTime,"keyUp","a"));
				}
				else if (Input.GetKeyUp(KeyCode.S)) {
					keylist.Add (new KeyRecording(Time.deltaTime,"keyUp","s"));
				}
				else if (Input.GetKeyUp(KeyCode.D)) {
					keylist.Add (new KeyRecording(Time.deltaTime,"keyUp","d"));
				}
			}
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
		keylist = new List<KeyRecording>();
		recordKeys = true;
	}


	//Creates a new Json file of the Recorder
	public void Save(String filename){
		JSONNode N = new JSONClass (); // Start with JSONArray or JSONClass
		float totalDuration = 0;
		foreach(Snapshot s in snapshotList){
			totalDuration +=s.duration;
		}
		N ["player recording"] ["total duration"].AsFloat = totalDuration;
		N ["player recording"] ["number frames"].AsInt = snapshotList.Count;
		for(int i =0; i<snapshotList.Count; i++){
			Snapshot snap = snapshotList[i];
			N ["player recording"] ["frames"] [i] ["duration"].AsFloat = snap.duration;
			N ["player recording"] ["frames"] [i] ["position"][0].AsFloat = snap.position.x;			
			N ["player recording"] ["frames"] [i] ["position"][1].AsFloat = snap.position.y;
			N ["player recording"] ["frames"] [i] ["position"][2].AsFloat = snap.position.z;  
			N ["player recording"] ["frames"] [i] ["rotation"][0].AsFloat = snap.rotation.x;			
			N ["player recording"] ["frames"] [i] ["rotation"][1].AsFloat = snap.rotation.y;
			N ["player recording"] ["frames"] [i] ["rotation"][2].AsFloat = snap.rotation.z;  
			N ["player recording"] ["frames"] [i] ["rotation"][3].AsFloat = snap.rotation.w;
			N ["player recording"] ["frames"] [i] ["inJump"].AsFloat = snap.inJump ? 1:0;
			N ["player recording"] ["frames"] [i] ["y rotation"].AsFloat = snap.playerRotation;
			N ["player recording"] ["frames"] [i] ["camera tilt"].AsFloat = snap.cameraRotation;
			N ["player recording"] ["frames"] [i] ["numberKeyRec"].AsInt = snap.keyRecList.Count;
			for(int k =0; k<snap.keyRecList.Count; k++){
				KeyRecording keyRecording = snap.keyRecList[k];
				N ["player recording"] ["frames"] [i] ["key events"] [k] ["time"].AsFloat = keyRecording.time;
				N ["player recording"] ["frames"] [i] ["key events"] [k] ["type"] = keyRecording.type;
				N ["player recording"] ["frames"] [i] ["key events"] [k] ["key"] = keyRecording.key;
			}
		}
		Debug.Log (N.ToJSON (4));
		File.WriteAllText(filename,N.ToJSON (4));
	}

	//Loads Recorder from a  Json file 
	public void Load(String filename){
		string text = File.ReadAllText(filename);
		JSONNode N = JSON.Parse(text);
		int numberFrames = N ["player recording"] ["number frames"].AsInt;
		snapshotList = new List<Snapshot> ();
		for(int i =0; i<numberFrames; i++){
			float duration = N ["player recording"] ["frames"] [i] ["duration"].AsFloat;
			float playerX = N ["player recording"] ["frames"] [i] ["position"][0].AsFloat;		
			float playerY = N ["player recording"] ["frames"] [i] ["position"][1].AsFloat;
			float playerZ = N ["player recording"] ["frames"] [i] ["position"][2].AsFloat; 
			Vector3 position = new Vector3(playerX,playerY,playerZ);
			float rotationX = N ["player recording"] ["frames"] [i] ["rotation"][0].AsFloat;			
			float rotationY = N ["player recording"] ["frames"] [i] ["rotation"][1].AsFloat;
			float rotationZ = N ["player recording"] ["frames"] [i] ["rotation"][2].AsFloat;
			float rotationW = N ["player recording"] ["frames"] [i] ["rotation"][3].AsFloat;
			Quaternion rotation = new Quaternion(rotationX,rotationY,rotationZ,rotationW);
			bool inJump = false;
			if(N ["player recording"] ["frames"] [i] ["inJump"].AsFloat ==1) inJump = true;
			float playerRotation = N ["player recording"] ["frames"] [i] ["y rotation"].AsFloat;
			float cameraRotation = N ["player recording"] ["frames"] [i] ["camera tilt"].AsFloat;
			int numberKeyRec = N ["player recording"] ["frames"] [i] ["numberKeyRec"].AsInt;
			keylist = new List<KeyRecording> ();
			for(int k =0; k<numberKeyRec; k++){
				float time = N ["player recording"] ["frames"] [i] ["key events"] [0] ["time"].AsFloat;
				String type = N ["player recording"] ["frames"] [i] ["key events"] [0] ["type"];
				String key = N ["player recording"] ["frames"] [i] ["key events"] [0] ["key"];
				keylist.Add(new KeyRecording(time,type,key));
			}
			Snapshot snap = new Snapshot(position, rotation, inJump, duration,playerRotation,cameraRotation,keylist);
			snapshotList.Add(snap);
		}
	}
}
