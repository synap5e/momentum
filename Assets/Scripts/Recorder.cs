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
	public bool recordKeys = true;
	public UnityEngine.UI.Text timeText;
	
	private bool recording = false;
	private float frameDuration;
	private float time;
	private float startTime;
	
	public bool IsRecording
	{
		get
		{
			return recording;
		}
	}
	
	void Start(){
		startTime = Time.time;
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
	
	void Update(){
		if (recording)
			RecordSnapshot();
		
		time = Time.time - startTime;
		timeText.text = getTimeString ();
	}
	
	public string getTimeString(){
		string minutes = Mathf.Floor(time / 60).ToString("00");
		string seconds = (time % 60).ToString("00");
		return minutes + ":" + seconds;
	}
	
	public void RecordSnapshot(){
		
		frameDuration += Time.deltaTime;
		if (frameDuration > recordingFrameDurationMin) {
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			bool inJump = !GetComponent<RigidbodyFPSController>().onGround;
			float duration = frameDuration;
			float playerRotation = transform.localEulerAngles.y;
			Camera camera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
			float cameraRotation = camera.transform.localEulerAngles.x;
			
			//Add the snapshot to the list
			Snapshot s = new Snapshot (position, rotation, inJump, duration, playerRotation, cameraRotation, keylist);
			
			snapshotList.Add (s);
			
			//reset frame duration
			frameDuration = 0;
			
			//reset keylist
			keylist = new List<KeyRecording> ();
		} 
		//Still within frame so record keys
		if(recordKeys){
			if (Input.GetKeyDown (KeyCode.Space)) {
				keylist.Add (new KeyRecording(Time.deltaTime,"keyDown","Space"));
			}
			if(Input.GetMouseButtonDown(0)){
				keylist.Add (new KeyRecording(Time.deltaTime,"keyDown","MouseButton"));
			}
			if (Input.anyKeyDown) {
				keylist.Add (new KeyRecording(Time.deltaTime,"keyDown",Input.inputString));
			}
			if (Input.GetKeyUp (KeyCode.Space)) {
				keylist.Add (new KeyRecording(Time.deltaTime,"keyUp","Space"));
			}
			if(Input.GetMouseButtonUp(0)){
				keylist.Add (new KeyRecording(Time.deltaTime,"keyUp","MouseButton"));
			}
			if (Input.GetKeyUp(KeyCode.W)) {
				keylist.Add (new KeyRecording(Time.deltaTime,"keyUp","w"));
			}
			if (Input.GetKeyUp(KeyCode.A)) {
				keylist.Add (new KeyRecording(Time.deltaTime,"keyUp","a"));
			}
			if (Input.GetKeyUp(KeyCode.S)) {
				keylist.Add (new KeyRecording(Time.deltaTime,"keyUp","s"));
			}
			if (Input.GetKeyUp(KeyCode.D)) {
				keylist.Add (new KeyRecording(Time.deltaTime,"keyUp","d"));
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
		startTime = Time.time;
		time = 0f;
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
	
	
	public string SaveToString()
	{
		JSONNode N = new JSONClass(); // Start with JSONArray or JSONClass
		float totalDuration = 0;
		foreach (Snapshot s in snapshotList)
		{
			totalDuration += s.duration;
		}
		N["player recording"]["total duration"].AsFloat = totalDuration;
		N["player recording"]["number frames"].AsInt = snapshotList.Count;
		for (int i = 0; i < snapshotList.Count; i++)
		{
			Snapshot snap = snapshotList[i];
			N["player recording"]["frames"][i]["duration"].AsFloat = snap.duration;
			N["player recording"]["frames"][i]["position"][0].AsFloat = snap.position.x;
			N["player recording"]["frames"][i]["position"][1].AsFloat = snap.position.y;
			N["player recording"]["frames"][i]["position"][2].AsFloat = snap.position.z;
			N["player recording"]["frames"][i]["rotation"][0].AsFloat = snap.rotation.x;
			N["player recording"]["frames"][i]["rotation"][1].AsFloat = snap.rotation.y;
			N["player recording"]["frames"][i]["rotation"][2].AsFloat = snap.rotation.z;
			N["player recording"]["frames"][i]["rotation"][3].AsFloat = snap.rotation.w;
			N["player recording"]["frames"][i]["inJump"].AsFloat = snap.inJump ? 1 : 0;
			N["player recording"]["frames"][i]["y rotation"].AsFloat = snap.playerRotation;
			N["player recording"]["frames"][i]["camera tilt"].AsFloat = snap.cameraRotation;
			N["player recording"]["frames"][i]["numberKeyRec"].AsInt = snap.keyRecList.Count;
			for (int k = 0; k < snap.keyRecList.Count; k++)
			{
				KeyRecording keyRecording = snap.keyRecList[k];
				N["player recording"]["frames"][i]["key events"][k]["time"].AsFloat = keyRecording.time;
				N["player recording"]["frames"][i]["key events"][k]["type"] = keyRecording.type;
				N["player recording"]["frames"][i]["key events"][k]["key"] = keyRecording.key;
			}
		}
		return N.ToJSON(4);
	}
	
	//Creates a new Json file of the Recorder
	public void Save(String filename){
		File.WriteAllText(filename, SaveToString());
	}
	
	//Loads Recorder from a  Json file 
	public void Load(String filename){
		string text = File.ReadAllText(filename);
		LoadString(text);
	}
	
	internal void LoadString(string text)
	{
		snapshotList = LoadSnapshots(text);
	}
	
	public static List<Snapshot> LoadSnapshots(String text)
	{
		JSONNode N = JSON.Parse(text);
		int numberFrames = N["player recording"]["number frames"].AsInt;
		List<Snapshot> snapshotList = new List<Snapshot>();
		for (int i = 0; i < numberFrames; i++)
		{
			float duration = N["player recording"]["frames"][i]["duration"].AsFloat;
			float playerX = N["player recording"]["frames"][i]["position"][0].AsFloat;
			float playerY = N["player recording"]["frames"][i]["position"][1].AsFloat;
			float playerZ = N["player recording"]["frames"][i]["position"][2].AsFloat;
			Vector3 position = new Vector3(playerX, playerY, playerZ);
			float rotationX = N["player recording"]["frames"][i]["rotation"][0].AsFloat;
			float rotationY = N["player recording"]["frames"][i]["rotation"][1].AsFloat;
			float rotationZ = N["player recording"]["frames"][i]["rotation"][2].AsFloat;
			float rotationW = N["player recording"]["frames"][i]["rotation"][3].AsFloat;
			Quaternion rotation = new Quaternion(rotationX, rotationY, rotationZ, rotationW);
			bool inJump = false;
			if (N["player recording"]["frames"][i]["inJump"].AsFloat == 1) inJump = true;
			float playerRotation = N["player recording"]["frames"][i]["y rotation"].AsFloat;
			float cameraRotation = N["player recording"]["frames"][i]["camera tilt"].AsFloat;
			int numberKeyRec = N["player recording"]["frames"][i]["numberKeyRec"].AsInt;
			List<KeyRecording> keylist = new List<KeyRecording>();
			for (int k = 0; k < numberKeyRec; k++)
			{
				float time = N["player recording"]["frames"][i]["key events"][0]["time"].AsFloat;
				String type = N["player recording"]["frames"][i]["key events"][0]["type"];
				String key = N["player recording"]["frames"][i]["key events"][0]["key"];
				keylist.Add(new KeyRecording(time, type, key));
			}
			Snapshot snap = new Snapshot(position, rotation, inJump, duration, playerRotation, cameraRotation, keylist);
			snapshotList.Add(snap);
		}
		return snapshotList;
	}
}