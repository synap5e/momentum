using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour {


	public GameObject Player;
	public GameObject FirstPersonCamera;
	public GameObject ThirdPersonCamera;

	public GameObject GhostPlayerPrefab;

	private List<GameObject> ghostPlayers = new List<GameObject>();

	private bool inPlaybackMode = false;
	private bool inMenuMode = false;
	int test = 6;

	private Vector3 cameraOffset = new Vector3(0f, 5f, -5f);
	private float cameraDistance = 2f;

	// Use this for initialization
	void Start () {
		ThirdPersonCamera.SetActive(false);
        Player.GetComponent<Recorder>().StartRecording();

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
		/*if (Input.GetKeyDown(KeyCode.R))
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
		}*/

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	public void StartPlayback()
	{
        if (Player.GetComponent<Recorder>().IsRecording)
		{
            Player.GetComponent<Recorder>().StopRecording();
		}

		GameObject newGhost = Instantiate<GameObject>(GhostPlayerPrefab);
		Playback playback = newGhost.GetComponent<Playback>();
		playback.StopPlayback();
        playback.Snapshots = Player.GetComponent<Recorder>().snapshotList;
		ghostPlayers.Add(newGhost);
		Player.SetActive(false);

		FirstPersonCamera.SetActive(false);
		FirstPersonCamera.GetComponent<AudioListener>().enabled = false;

		ThirdPersonCamera.SetActive(true);
		ThirdPersonCamera.GetComponent<AudioListener>().enabled = true;
		ThirdPersonCamera.transform.position = playback.GetStartPosition() + (playback.GetStartRotation() * cameraOffset);
		Vector3 focusVector = GhostPlayerPrefab.GetComponent<SkinnedMeshRenderer>().bounds.max;
		focusVector.x = GhostPlayerPrefab.GetComponent<SkinnedMeshRenderer>().bounds.center.x;
		ThirdPersonCamera.transform.LookAt(playback.GetStartPosition() + focusVector);

		playback.StartPlayback();
		inPlaybackMode = true;
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

		Player.SetActive(true);

		FirstPersonCamera.SetActive(true);
		FirstPersonCamera.GetComponent<AudioListener>().enabled = true;
		ThirdPersonCamera.SetActive(false);
		ThirdPersonCamera.GetComponent<AudioListener>().enabled = false;

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
