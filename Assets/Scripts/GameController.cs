using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour {


	public GameObject Player;
	public GameObject FirstPersonCamera;
	public GameObject ThirdPersonCamera;

	public GameObject GhostPlayerPrefab;

	public TextAsset ReplayFile;

	private List<GameObject> ghostPlayers = new List<GameObject>();
	private GameObject replayGhost;

	private bool inPlaybackMode = false;
	private bool inMenuMode = false;
	int test = 6;

	private Vector3 cameraOffset = new Vector3(0f, 5f, -5f);
	private float cameraDistance = 2f;

	// Use this for initialization
	void Start () {
		ThirdPersonCamera.SetActive(false);
        Player.GetComponent<Recorder>().StartRecording();
	}
	
	// Update is called once per frame
	void Update () {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		if (Input.GetKeyDown(KeyCode.R))
		{
            if (Player.GetComponent<Recorder>().IsRecording)
            {
                Player.GetComponent<Recorder>().StopRecording();
			}
			else
			{
                Player.GetComponent<Recorder>().StartRecording();
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

		if (Input.GetKeyDown(KeyCode.H))
		{
			CreateReplayGhost();
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	private void CreateReplayGhost()
	{
		if (ReplayFile == null)
		{
			return;
		}
		if (replayGhost != null)
		{
			Destroy(replayGhost);
		}
		replayGhost = Instantiate<GameObject>(GhostPlayerPrefab);
		Playback playback = replayGhost.GetComponent<Playback>();
		playback.Snapshots = Recorder.LoadSnapshots(ReplayFile.text);
		playback.StartPlayback();
	}

	public void StartPlayback()
	{
        if (Player.GetComponent<Recorder>().IsRecording)
		{
            Player.GetComponent<Recorder>().StopRecording();
		}

		GameObject newGhost = Instantiate<GameObject>(GhostPlayerPrefab);
		Playback playback = newGhost.GetComponent<Playback>();
        playback.Snapshots = Player.GetComponent<Recorder>().snapshotList;
		ghostPlayers.Add(newGhost);

		ThirdPersonCamera.transform.position = playback.GetStartPosition() + (playback.GetStartRotation() * cameraOffset);
		Vector3 focusVector = newGhost.GetComponentInChildren<SkinnedMeshRenderer>().bounds.max;
		focusVector.x = newGhost.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center.x;
		ThirdPersonCamera.transform.LookAt(playback.GetStartPosition() + focusVector);

		EnableFirstPersonCamera(false);

		playback.StartPlayback();
		inPlaybackMode = true;
	}

	public void EnableFirstPersonCamera(bool isFirstPerson)
	{
		Player.SetActive(isFirstPerson);
		FirstPersonCamera.SetActive(isFirstPerson);
		FirstPersonCamera.GetComponent<AudioListener>().enabled = isFirstPerson;

		ThirdPersonCamera.SetActive(!isFirstPerson);
		ThirdPersonCamera.GetComponent<AudioListener>().enabled = !isFirstPerson;
	}

    public void StopPlayback()
	{
		if (ghostPlayers.Any())
		{
			GameObject lastGhost = ghostPlayers.Last();

			Playback playback = lastGhost.GetComponent<Playback>();
			playback.StopPlayback();

			Destroy(lastGhost);
			ghostPlayers.RemoveAt(ghostPlayers.Count - 1);
		}

		EnableFirstPersonCamera(true);

		inPlaybackMode = false;

        Player.GetComponent<Recorder>().StartRecording();
	}

	void StopRecording()
	{
        Player.GetComponent<Recorder>().StopRecording();

	}

	public void RemoveGhost(GameObject ghost)
	{
		ghostPlayers.Remove(ghost);
		Destroy(ghost);
		if (!ghostPlayers.Any())
		{
			StopPlayback();
		}
	}
}
