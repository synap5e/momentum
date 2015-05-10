using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

    public GameObject spawn;
    public bool requireLanded = true;

    public int CheckpointNumber;
    public int ReplaySnapshot;

    void Awake()
    {
        if (spawn == null)
        {
            // default to closest
            float minDist = float.PositiveInfinity;
            foreach (GameObject respawn in GameObject.FindGameObjectsWithTag("Respawn"))
            {
                if (spawn == null) spawn = respawn;

                float dist = (transform.position - respawn.transform.position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    spawn = respawn;
                }
            }
        }
        else
        {
            spawn.GetComponent<Renderer>().enabled = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && (!requireLanded || Vector3.Angle(collision.contacts[0].normal, Vector3.down) < 80)) // disallow hitting a vertical surface to count as hitting the checkpoint
        {
            collision.gameObject.GetComponent<RespawnController>().CurrentCheckpoint = this;
        }
    }
}
