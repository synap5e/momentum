using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtensionMethods;

public class Playback : MonoBehaviour {
	public float Speed;
	public float SpeedDampening;

	private int snapshotIndex = 0;
	private bool playback = false;
	private bool wasPaused = false;

	private float frameDuration;

	private Animator playerAnimator;

	private Recorder.Snapshot currentSnapshot;
	private Recorder.Snapshot previousSnapshot;
	private Recorder.Snapshot nextSnapshot;

	private float previousSpeed;
	private float nextSpeed;

	public List<Recorder.Snapshot> Snapshots { get; set; }

	// Use this for initialization
	void Start () {


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
			frameDuration -= currentSnapshot.duration;
			moveToNextSnapshot();
		}
		if(nextSnapshot == null){
			EndPlayback();
			return;
		}

		float framePercentageComplete = frameDuration / currentSnapshot.duration;
		Vector3 previousPosition = previousSnapshot == null ? currentSnapshot.position : transform.position;
		Vector3 nextPosition = Vector3.Lerp(currentSnapshot.position, nextSnapshot.position, framePercentageComplete);

		gameObject.transform.position = nextPosition;
		gameObject.transform.rotation = Quaternion.Slerp(currentSnapshot.rotation, nextSnapshot.rotation, framePercentageComplete);

		float instantaneousSpeed = (nextPosition - previousPosition).ToXZ().magnitude;
		bool inAir = currentSnapshot.inJump;

		if (inAir)
		{
			instantaneousSpeed = 0f;
		}

		// TODO: add more logic to handle free-fall vs jump

		GetComponent<Animator>().SetBool(Animator.StringToHash("InJump"), inAir);
		GetComponent<Animator>().SetFloat(Animator.StringToHash("Speed"), instantaneousSpeed * Speed, 0, Time.deltaTime);
		Debug.Log(GetComponent<Animator>().GetFloat(Animator.StringToHash("Speed")));
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
}
