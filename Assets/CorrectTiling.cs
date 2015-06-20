using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CorrectTiling : MonoBehaviour {
    public Material Material;

	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().material = new Material(Material);
	}
	
	// Update is called once per frame
	void Update () {
        Matrix4x4 m = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        GetComponent<Renderer>().material.SetMatrix("_Transform", m);
	}
}
