using UnityEngine;
using System.Collections;

public class LevelOneObjective : MonoBehaviour {

	public GUIStyle style;
	public GameObject player;
	public float winDistance = 10;

	void Update () {
		if (Vector3.Magnitude (player.transform.position - transform.position) < winDistance) {
			Application.LoadLevel("land");
		}
	}

	void OnGUI() {
		/*GUI.Label(new Rect(100,10,Screen.width,Screen.height),"Swim to the green buoy!\nRight click on a location to swim there," +
		          "\nhold down SHIFT to swim faster.\n\nAlternatly use the arrow keys to move.\n\nMoving the mouse to the edge of the screen will scroll,\n" +
		          "and SPACE will return the camera to the player.\n\nSome floating objects can be pushed.", style);*/
	}
}
