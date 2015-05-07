using UnityEngine;
using System.Collections;

public class AdHocTimer : MonoBehaviour {
    private float stime;
    private float etime;
    private Vector3 spos;
    private Quaternion srot;

	// Use this for initialization
	void Start () {
        this.stime = Time.time;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        this.spos = player.transform.position;
        this.srot = player.transform.rotation;
	}

    void OnGUI()
    {
        GUI.Label(new Rect(10, 50, 100, 20), (etime == 0 ? Time.time - stime : etime-stime) + "s");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            this.stime = Time.time;
            this.etime = 0;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            player.transform.position = spos;
            player.transform.rotation = srot;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.etime = Time.time;
        }
    }
	
	
}
