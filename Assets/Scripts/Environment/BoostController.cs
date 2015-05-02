using UnityEngine;
using System.Collections;

public class BoostController : MonoBehaviour {

    
    public float force = 50;

    [Header("Internals")]
    public int coolDownMillis = 500;
    public float activationRadius = 6;

    private GameObject player;
    private float lastActivated;
	
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Start()
    {
        lastActivated = Time.time;
    }

	void Update () {
        if (Input.GetMouseButtonDown(0) && (player.transform.position - transform.position).sqrMagnitude < activationRadius && (lastActivated + coolDownMillis / 1000.0f) < Time.time)
        {
            player.GetComponent<Rigidbody>().AddForce(transform.up * force, ForceMode.VelocityChange);
            lastActivated = Time.time;
        }
	}

}
