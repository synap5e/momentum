using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtensionMethods;

public class Playback : MonoBehaviour {
	public float Speed;
	public float SpeedDampening;

	private int snapshotIndex;
	private bool playback = false;
	private bool wasPaused = false;

	private float frameDuration;

	private Rigidbody playerRigidbody;
	private Animator playerAnimator;

	private Recorder.Snapshot currentSnapshot;
	private Recorder.Snapshot previousSnapshot;
	private Recorder.Snapshot nextSnapshot;

	private float previousSpeed;
	private float nextSpeed;

	public List<Recorder.Snapshot> Snapshots { get; set; }

	// Use this for initialization
	void Start () {
		snapshotIndex = 0;
		playerRigidbody = GetComponent<Rigidbody>();
		playerAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
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
		if(wasPaused){
			wasPaused = false;
		} else {
			frameDuration += Time.deltaTime;
		}
		if (frameDuration >= currentSnapshot.duration)
		{
			frameDuration = 0;
			moveToNextSnapshot();
		}
		if(nextSnapshot == null){
			StopPlayback();
			StartPlayback();
		}
		float framePercentageComplete = frameDuration / currentSnapshot.duration;
		Vector3 previousPosition = previousSnapshot == null ? currentSnapshot.position : playerRigidbody.position;
		Vector3 nextPosition = Vector3.Lerp(currentSnapshot.position, nextSnapshot.position, framePercentageComplete);
		playerRigidbody.position = nextPosition;
		playerRigidbody.rotation = Quaternion.Slerp(currentSnapshot.rotation, nextSnapshot.rotation, framePercentageComplete);

		float instantaneousSpeed = (nextPosition - previousPosition).ToXZ().magnitude;

		if(currentSnapshot.inJump){
			instantaneousSpeed = 0f;
		}
		playerAnimator.SetFloat(Animator.StringToHash("Speed"), instantaneousSpeed * Speed,  previousSnapshot == null ? 0 : SpeedDampening, Time.deltaTime);

	}

	void moveToNextSnapshot()
	{
		currentSnapshot = Snapshots[snapshotIndex];
		previousSnapshot = null;
		nextSnapshot = null;
		previousSpeed = 0f;

		if(snapshotIndex > 0)
		{
			previousSnapshot = Snapshots[snapshotIndex - 1];
			previousSpeed = (currentSnapshot.position.ToXZ() - previousSnapshot.position.ToXZ()).magnitude / previousSnapshot.duration;
		}
		if (snapshotIndex + 1 < Snapshots.Count)
		{
			nextSnapshot = Snapshots[snapshotIndex + 1];
			nextSpeed = (nextSnapshot.position.ToXZ() - currentSnapshot.position.ToXZ()).magnitude / currentSnapshot.duration;
		}
		snapshotIndex++;
	}



	public void StartPlayback()
	{
		if (currentSnapshot == null)
		{
			moveToNextSnapshot();

		}
		playback = true;
	}

	public void StopPlayback()
	{
		playback = false;
		snapshotIndex = 0;
		currentSnapshot = null;
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
}
