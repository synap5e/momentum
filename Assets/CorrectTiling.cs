using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CorrectTiling : MonoBehaviour {
    public Material material;

	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().material = new Material(material);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
        if (transform.localScale.x == 1)
        {
            transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime, Space.World);
        }
        Matrix4x4 m = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Debug.Log(m);
        GetComponent<Renderer>().material.SetMatrix("_Transform", m);
	}
}
