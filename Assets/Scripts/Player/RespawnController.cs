using UnityEngine;
using System.Collections;

public class RespawnController : MonoBehaviour {

    public GameObject spawnPosition
    {
        get;
        set;
    }

    private Vector3 initialPos;
    private Quaternion initialRot;

    void Start()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;

        foreach (GameObject respawn in GameObject.FindGameObjectsWithTag("Respawn"))
        {
            respawn.GetComponent<Renderer>().enabled = false;
        }
    }

    void Update()
    {
        // check out of bounds - TODO: look into using world AABB or distance from closest object
        if (transform.position.y < -100)
        {
            Respawn();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Instant Kill")
        {
            Respawn();
        }
       
    }

    void OnCollisionStay(Collision collision)
    {
        //Debug.Log(GetComponent<RigidbodyFPSController>().usingGroundedPhysics + " && " + (collision.gameObject.tag == "Delayed Kill"));
        if (GetComponent<RigidbodyFPSController>().usingGroundedPhysics && collision.gameObject.tag == "Delayed Kill")
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        // TODO: fancy animations etc, inform feedback, show on debug etc.
        if (spawnPosition)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = spawnPosition.transform.position;
            transform.forward = spawnPosition.transform.forward;
        }
        else
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = initialPos;
            transform.rotation = initialRot;

        }

        if (GetComponent<BombActivator>() != null)
            GetComponent<BombActivator>().ReactivateBombs();
    }
	
}
