using UnityEngine;
using System.Collections;

public class RespawnController : MonoBehaviour {

    private Checkpoint _currentCheckpoint;

    private Vector3 initialPos;
    private Vector3 initialRot;

    public float respawnTravelVelocity = 2;
    public float minRespawnTime = 0.3f;

    public float deathOverlayCrossfadeTime = 0.5f;

    private float respawnTravelDuration = 0;
    private Vector3 deathPosition;

    private Checkpoint defaultCheckpoint;
    private bool respawning;
    private Quaternion deathLook;
    private GameObject deathOverlay;
   

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

        deathOverlay = GameObject.Find("Death Overlay");
        deathOverlay.GetComponent<UnityEngine.UI.RawImage>().CrossFadeAlpha(0f, 0f, true);
        //deathOverlay.SetActive(false);
    }

    void Update()
    {
        // check out of bounds - TODO: look into using world AABB or distance from closest object
        if (transform.position.y < -100)
        {
            Respawn();
        }

 //       Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent< Camera>().transform.localEulerAngles.x);
        if (respawning)
        {
            respawnTravelDuration += Time.deltaTime;
            if (Input.GetMouseButton(1))
            {
                // 5x faster animation is M2 held
                respawnTravelDuration += Time.deltaTime * 4;
            }
            float respawnDistance = (CurrentCheckpoint.spawn.transform.position - deathPosition).magnitude;
            float respawnTime = Mathf.Max(respawnDistance / respawnTravelVelocity, minRespawnTime);
            if (respawnTravelDuration < respawnTime)
            {
                float t = respawnTravelDuration / respawnTime;
                transform.position = Vector3.Lerp(deathPosition, CurrentCheckpoint.spawn.transform.position, t);
                Quaternion spawnLook = Quaternion.Euler(new Vector3(0, CurrentCheckpoint.spawn.transform.localEulerAngles.y, 0));
                Quaternion look = Quaternion.Slerp(deathLook, spawnLook, t * 2 - 1);

                transform.localEulerAngles = new Vector3(0, look.eulerAngles.y, 0);
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().transform.localEulerAngles = new Vector3(look.eulerAngles.x, 0, 0);
            } 
            else
            {
                respawning = false;
//                deathOverlay.SetActive(false);
                deathOverlay.GetComponent<UnityEngine.UI.RawImage>().CrossFadeAlpha(0f, deathOverlayCrossfadeTime, false);


                GetComponent<Collider>().enabled = true;
                GetComponent<RigidbodyFPSController>().enableInput = true;
                transform.position = CurrentCheckpoint.spawn.transform.position;
                transform.forward = CurrentCheckpoint.spawn.transform.forward;
            }
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
  //      

        respawnTravelDuration = 0;
        deathPosition = transform.position;
        deathLook = Quaternion.Euler(new Vector3(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().transform.localEulerAngles.x, transform.rotation.eulerAngles.y, 0));
        respawning = true;
        GetComponent<RigidbodyFPSController>().enableInput = false;
        GetComponent<Collider>().enabled = false;

//        deathOverlay.SetActive(true);
        deathOverlay.GetComponent<UnityEngine.UI.RawImage>().CrossFadeAlpha(1f, deathOverlayCrossfadeTime, false);

        if (GetComponent<BombActivator>() != null)
            GetComponent<BombActivator>().ReactivateBombs();
    }

}
