using UnityEngine;
using System.Collections;

public class OrthographicCameraZoom : MonoBehaviour {

	[System.Serializable]
	public class ZoomScale {
		public float mouseZoomScale = 1.0f;
		public float keyboardZoomScale = 1.0f;
	}
	public ZoomScale zoomScale;

	public float zoomSpeed = 1.0f;

	[System.Serializable]
	public class ZoomLimit {
		public float minZoom = 1.0f;
		public float maxZoom = 10.0f;
	}
	public ZoomLimit zoomLimit;

	private float startOrthographicSize;
	private float endOrthographicSize;
	
	private float elapsed = 0.0f;

	private Camera cam;

	void Start () {
		cam = Camera.main;

		startOrthographicSize = cam.orthographicSize;
		endOrthographicSize = cam.orthographicSize;
	}
	
	void Update () {
		float zoomDelta = 0;
		zoomDelta -= Input.GetAxisRaw ("Mouse ScrollWheel") * zoomScale.mouseZoomScale;
		zoomDelta -= (Input.GetKeyDown (KeyCode.Plus)  || Input.GetKeyDown (KeyCode.Equals) || Input.GetKeyDown (KeyCode.KeypadPlus)) ?  zoomScale.keyboardZoomScale : 0;
		zoomDelta -= (Input.GetKeyDown (KeyCode.Minus) || Input.GetKeyDown (KeyCode.KeypadMinus)) ? -zoomScale.keyboardZoomScale : 0;

		if (zoomDelta != 0) {
			startOrthographicSize = cam.orthographicSize;
			endOrthographicSize = Mathf.Clamp(endOrthographicSize + zoomDelta, zoomLimit.minZoom, zoomLimit.maxZoom);
			elapsed = 0;
		}

		if (cam.orthographicSize != endOrthographicSize) {
			cam.orthographicSize = Mathf.Lerp (startOrthographicSize, endOrthographicSize, elapsed);
			elapsed += Time.deltaTime / (1.0f/zoomSpeed);
		}

	}
}
