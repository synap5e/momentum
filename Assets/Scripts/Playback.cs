using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtensionMethods;

public class Playback : MonoBehaviour {
	public float Speed;
	public float SpeedDampening;
    public bool Free = false;
    public bool Loop = false;

	private int snapshotIndex = 0;
	private bool playback = false;
	private bool wasPaused = false;

	private float frameDuration;

	private Recorder.Snapshot currentSnapshot;
	private Recorder.Snapshot previousSnapshot;
	private Recorder.Snapshot nextSnapshot;

	private Vector3 previousVelocity;
    private Vector3 nextVelocity;

	public List<Recorder.Snapshot> Snapshots { get; set; }

    private Checkpoint currentCheckpoint;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
    void Update()
    {
        if (Snapshots == null)
        {
            GameObject.FindObjectOfType<GameController>().RemoveGhost(gameObject);
            return;
        }
        if (!playback)
        {
            return;
        }
        if (Snapshots == null || snapshotIndex > Snapshots.Count)
        {
            StopPlayback();
            return;
        }

        bool pauseOnFrame = false;
        if (!Free)
        {
            Checkpoint playerCheckpoint = GameObject.FindObjectOfType<RespawnController>().CurrentCheckpoint;
            if (currentCheckpoint != null
                && currentCheckpoint.Index > playerCheckpoint.Index
                && !currentSnapshot.inJump
                && Vector3.Dot(currentCheckpoint.spawn.transform.position - gameObject.transform.position, gameObject.transform.forward) < -1)
            {
                pauseOnFrame = true;
            }
        }

        if (wasPaused)
        {
            wasPaused = false;
            pauseOnFrame = true;

        }
        if (!pauseOnFrame)
        {
            frameDuration += Time.deltaTime;
        }

        if (frameDuration >= currentSnapshot.duration)
        {
            frameDuration -= currentSnapshot.duration;
            moveToNextSnapshot();
        }

        if (nextSnapshot == null)
        {
            if (Loop)
            {
                SetPlaybackPosition(0);
            } else {
                EndPlayback();
                return;
            }
    
        }

        float framePercentageComplete = frameDuration / currentSnapshot.duration;
        Vector3 previousPosition = previousSnapshot == null ? currentSnapshot.position : transform.position;
        Vector3 nextPosition = Vector3.Lerp(currentSnapshot.position, nextSnapshot.position, framePercentageComplete);

        gameObject.transform.position = nextPosition;
        float instantaneousSpeed = (nextPosition - previousPosition).ToXZ().magnitude;
        if (instantaneousSpeed > 0.1)
        {
            Vector3 forward = Vector3.Slerp(previousVelocity, nextVelocity, framePercentageComplete);
            forward.y = 0;
            gameObject.transform.forward = forward.normalized;
        } else {
            gameObject.transform.rotation = Quaternion.Slerp(currentSnapshot.rotation, nextSnapshot.rotation, framePercentageComplete);
        }

        bool inAir = currentSnapshot.inJump;

        // TODO: add more logic to handle free-fall vs jump
        GetComponent<Animator>().SetBool(Animator.StringToHash("InJump"), inAir);
        GetComponent<Animator>().SetFloat(Animator.StringToHash("Speed"), instantaneousSpeed * Speed, 0, Time.deltaTime);
    }


	void moveToNextSnapshot()
	{
		currentSnapshot = Snapshots[snapshotIndex];
		previousSnapshot = null;
		nextSnapshot = null;
		previousVelocity = Vector3.zero;

		if(snapshotIndex > 0)
		{
			previousSnapshot = Snapshots[snapshotIndex - 1];
			previousVelocity = (currentSnapshot.position - previousSnapshot.position);
		}
		if (snapshotIndex + 1 < Snapshots.Count)
		{
			nextSnapshot = Snapshots[snapshotIndex + 1];
			nextVelocity = (nextSnapshot.position - currentSnapshot.position);
		}
		snapshotIndex++;
	}

    public void SetPlaybackPosition(int snapshotIndex)
    {
        if (snapshotIndex < 0 || snapshotIndex >= Snapshots.Count)
        {
            return;
        }
        this.snapshotIndex = snapshotIndex;
        moveToNextSnapshot();
    }


	public void StartPlayback()
	{
		if (currentSnapshot == null)
		{
			moveToNextSnapshot();

		}
		gameObject.SetActive(true);
		playback = true;
	}

	public void StopPlayback()
	{
		playback = false;
		snapshotIndex = 0;
		currentSnapshot = null;
	}

	public void EndPlayback()
	{
		StopPlayback();
		gameObject.SetActive(false);
	}

	public void PausePlayback()
	{
		playback = false;
		wasPaused = true;
	}

	public Vector3 GetStartPosition()
	{
		return Snapshots.First().position;
	}

	public Quaternion GetStartRotation()
	{
		return Snapshots.First().rotation;
	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground" && other.gameObject.GetComponent<Checkpoint>() != null)
        {
            currentCheckpoint = other.gameObject.GetComponent<Checkpoint>();
        }
    }
}
