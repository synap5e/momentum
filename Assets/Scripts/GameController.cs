using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour {
	public GameObject Player;
    public GameObject FirstPersonCameraPrefab;
	public GameObject ThirdPersonCamera;

	public GameObject GhostPlayerPrefab;

	public TextAsset ReplayFile;

	private List<GameObject> ghostPlayers = new List<GameObject>();
	private GameObject replayGhost;

	private bool inPlaybackMode = false;
	private bool inMenuMode = false;

	private Vector3 cameraOffset = new Vector3(0f, 4f, -4f);

	public float SnapshotMinDistance = 5;
	private List<Recorder.Snapshot> replaySnapshots;

    public bool FollowReplay = false;

    private GameObject FirstPersonCamera;

    void Awake()
    {
        FirstPersonCamera = Instantiate(FirstPersonCameraPrefab);
        FirstPersonCamera.transform.SetParent(Player.transform);
	}

    private void InitialiseCheckpointsIntoReplay()
    {
        // SUPER EXPENSIVE!! THIS SHOULD BE BAKED DURING BUILDING, NOT RAN EVERY PLAY :(
        if (replaySnapshots == null)
        {
            return;
        }

        int index = 0;
        List<Checkpoint> checkpoints = GameObject.FindObjectsOfType<Checkpoint>().ToList();
        float minSqrDistance = float.PositiveInfinity;
        for (int i = 0; i != replaySnapshots.Count; i++ )
        {
            Recorder.Snapshot snapshot = replaySnapshots[i];
            float currentMinSqrDistance = float.PositiveInfinity;
            Checkpoint nearestCheckpoint = null;
            foreach (Checkpoint checkpoint in checkpoints)
            {
                float sqrDistance = (checkpoint.spawn.transform.position - snapshot.position).sqrMagnitude;
                if (sqrDistance < currentMinSqrDistance)
                {
                    nearestCheckpoint = checkpoint;
                    currentMinSqrDistance = sqrDistance;
                }
            }
            if (currentMinSqrDistance > minSqrDistance)
            {
                nearestCheckpoint.Index = index;
                nearestCheckpoint.SnapshotIndex = i;
                index++;
                checkpoints.Remove(nearestCheckpoint);
                minSqrDistance = float.PositiveInfinity;
            }
            else
            {
                minSqrDistance = currentMinSqrDistance;
            }
            if (!checkpoints.Any())
            {
                break;
            }
        }
    }

	// Use this for initialization
	void Start () {
		ThirdPersonCamera.SetActive(false);
        if (ReplayFile != null)
        {
            replaySnapshots = Recorder.LoadSnapshots(ReplayFile.text);
            InitialiseCheckpointsIntoReplay();
        }

        Player.GetComponent<Recorder>().StartRecording();
        CreateReplayGhost();

	}
	
	// Update is called once per frame
	void Update () {
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

		if (Input.GetKeyDown(KeyCode.I))
		{
			Player.GetComponent<Recorder>().Save("tutorial");
		}

//		if (Input.GetKeyDown(KeyCode.Escape))
//		{
//			Application.Quit();
//		}

        if (FollowReplay && replayGhost != null)
        {
            EnableFirstPersonCamera(false);
            Playback playback = replayGhost.GetComponent<Playback>();
            ThirdPersonCamera.GetComponent<Rigidbody>().position = replayGhost.transform.position + (replayGhost.transform.rotation * cameraOffset);
        }
	}

	private void CreateReplayGhost()
	{
		if (replaySnapshots == null)
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
        playback.SetPlaybackPosition(Player.GetComponent<RespawnController>().CurrentCheckpoint.SnapshotIndex);
		playback.StartPlayback();
	}

	public void StartPlayback()
	{
        if (Player.GetComponent<Recorder>().IsRecording)
		{
            Player.GetComponent<Recorder>().StopRecording();
		}

		//Setup new ghost
		GameObject newGhost = Instantiate<GameObject>(GhostPlayerPrefab);
		Playback playback = newGhost.GetComponent<Playback>();
        playback.Snapshots = Player.GetComponent<Recorder>().snapshotList;
		ghostPlayers.Add(newGhost);

		//Set up camera
		ThirdPersonCamera.transform.position = playback.GetStartPosition() + (playback.GetStartRotation() * cameraOffset);
		Vector3 focusVector = newGhost.GetComponentInChildren<SkinnedMeshRenderer>().bounds.max;
		focusVector.x = newGhost.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center.x;
		ThirdPersonCamera.transform.LookAt(playback.GetStartPosition() + focusVector);

		// Set active camera
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
