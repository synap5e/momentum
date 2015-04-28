using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public Recorder recorder;
	public Playback playback;

	public GameObject Player;
	public GameObject FirstPersonCamera;
	public GameObject ThirdPersonCamera;

	public GameObject GhostPlayer;

	private bool inPlaybackMode = false;

	// Use this for initialization
	void Start () {
		GhostPlayer.SetActive(false);
		ThirdPersonCamera.SetActive(false);
		recorder.StartRecording();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.R))
		{
			if(recorder.IsRecording){
				recorder.StopRecording();
			}
			else
			{
				recorder.StartRecording();
			}
		}

		if (Input.GetKeyDown(KeyCode.P))
		{
			if (!inPlaybackMode)
			{
				StartPlayback();
			}
			else
			{
				StopPlayback();

			}
		}
	}

	void StartPlayback()
	{
		if(recorder.IsRecording){
			recorder.StopRecording();
		}
		playback.StopPlayback();
		playback.Snapshots = recorder.snapshotList;

		GhostPlayer.SetActive(true);
		Player.SetActive(false);

		FirstPersonCamera.SetActive(false);
		ThirdPersonCamera.SetActive(true);

		playback.StartPlayback();
		inPlaybackMode = true;
	}

	void StopPlayback()
	{
		playback.StopPlayback();

		GhostPlayer.SetActive(false);
		Player.SetActive(true);

		FirstPersonCamera.SetActive(true);
		ThirdPersonCamera.SetActive(true);

		inPlaybackMode = false;
	}

	void StopRecording()
	{
		recorder.StopRecording();

	}
}
