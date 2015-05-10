using UnityEngine;
using System.Collections;

public class RespawnController : MonoBehaviour {

    private Checkpoint _currentCheckpoint;

    private Vector3 initialPos;
    private Vector3 initialRot;

    private Checkpoint defaultCheckpoint;

    public Checkpoint CurrentCheckpoint
    {
        set
        {
             _currentCheckpoint = value;
        }
        get
        {
            return _currentCheckpoint != null ? _currentCheckpoint : defaultCheckpoint;
        }
    }


    void Start()
    {
        GameObject defaultSpawn = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        defaultSpawn.transform.position = transform.position;
        defaultSpawn.transform.forward = transform.forward;

        defaultCheckpoint = gameObject.AddComponent<Checkpoint>();
        defaultCheckpoint.spawn = defaultSpawn;

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
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = CurrentCheckpoint.spawn.transform.position;
        transform.forward = CurrentCheckpoint.spawn.transform.forward;

        if (GetComponent<BombActivator>() != null)
            GetComponent<BombActivator>().ReactivateBombs();
    }

}
