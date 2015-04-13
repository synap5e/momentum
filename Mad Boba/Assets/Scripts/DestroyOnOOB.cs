using UnityEngine;
using System.Collections;

public class DestroyOnOOB : MonoBehaviour {

	public Bounds bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(100, 10, 100));
	public int watchdog = 0;

	void Update () {
		if (!bounds.Contains (transform.position)) {
			Destroy(gameObject);
		}
	}

	private IEnumerator Start()
	{
		if (watchdog > 0) {
			yield return new WaitForSeconds (watchdog);
			Destroy (gameObject);
		}
	}

}
