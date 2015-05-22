using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

	public GameObject image;
	// Use this for initialization
	void Start () {
		image.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.90f);
	}
	
	// Update is called once per frame
	void Update () {
		image.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.90f);
	}
}
