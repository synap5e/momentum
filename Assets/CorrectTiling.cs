using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CorrectTiling : MonoBehaviour {
    public Material Material;
    public bool ApplyRotation = false;
    public bool ApplyPosition = false;

	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().material = new Material(Material);
	}
	
	// Update is called once per frame
	void Update () {
        if (ApplyRotation)
        {
            transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
        }
        if (ApplyPosition)
        {
            transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime, Space.World);
        }
        Matrix4x4 m = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        GetComponent<Renderer>().material.SetMatrix("_Transform", m);
	}
}
