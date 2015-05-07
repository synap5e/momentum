using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AirstrafeController))]
[RequireComponent(typeof(DebugMovement))]

// adapted from http://wiki.unity3d.com/index.php?title=RigidbodyFPSWalker
public class RigidbodyFPSController : MonoBehaviour
{

    [Header("Input")]
    [Range(0.01f, 20f)]
    public float mouseSensitivity = 5F;

    [Header("Basic Movement")]
    public float speed = 10.0f;
    public float jumpForce = 10.0f;

    [Header("Air Movement")]
    public float aircontrolForce = 0.1f;
    public float airMovementMaxVelocitySq = 100.0f;

    [Header("Bunnyhopping")]
    [Tooltip("Length of the window that we will accept a bunnyhop in.\n" +
             "This allows the player 1/2 that time before hitting the ground and 1/2 after. " +
             "If they jump within this time they will not lose any velocity to friction.")]
    public float bunnyhopWindow = 0.2f;
    public bool autoBunnyhop = false;


    [Header("Other (WIP)")]
    [Range(0, 1)]
    public float surfaceFriction = 0.25f;


    private Camera viewCamera;

    private int groundApplicationTicks = 5;

    // number of ticks the player has been on the ground
    private int onGroundTicks;

    // number of ticks the player has been off the ground
    private int offGroundTicks;

    internal bool usingGroundedPhysics
    {
        get
        {
            return onGroundTicks > groundApplicationTicks;
        }
    }
    private bool inAir = false;
    internal bool onGround
    {
        get
        {
            return !inAir;
        }
    }

    private float rotation, cameraTilt;

    private bool doJump = false;
    private float jumpQueuedTime;
    private bool prematureJump;
    private Vector3 incomingVel;


    void Awake()
    {
        viewCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().useGravity = false;

      //  Time.timeScale = 0.1f;

        // frictionless
        GetComponent<Collider>().material.dynamicFriction = 0;
        GetComponent<Collider>().material.frictionCombine = PhysicMaterialCombine.Multiply;
    }


    void Start()
    {
    }

    void Update()
    {

		Screen.lockCursor = true;

        // mouse X axis rotates the player but the Y axis simply tilts the camera
        rotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        cameraTilt = -Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(new Vector3(0, rotation, 0));
        ApplyTiltClamped(cameraTilt, 90, 270);

        // TODO: hack to reset while testing 
        if (transform.position.y < -100)
        {
            transform.position = Vector3.zero;
            if (GetComponent<BombActivator>() != null)
                GetComponent<BombActivator>().ReactivateBombs();
        }

        // TODO: hack to simulate explosive jump
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            GetComponent<Rigidbody>().AddForce(transform.TransformVector(new Vector3(0, 1200, 2000)));
        }

        if (!doJump && (Input.GetButtonDown("Jump") || Input.GetButton("Jump") && autoBunnyhop))
        {
            doJump = true;
            jumpQueuedTime = Time.time;
            prematureJump = inAir;
        }
    }

    void OnGUI()
    {
        Vector3 horvel = GetComponent<Rigidbody>().velocity;
        horvel.y = 0;

        GUI.Label(new Rect(0, 20, 500, 500), Mathf.Round(horvel.magnitude * 10000) / 10000F + " / " + Mathf.Round(incomingVel.magnitude * 10000) / 10000F + "");
    }

    void FixedUpdate()
    {
        CheckGrounded();

        if (!inAir)
        {
            if (!PerformJump())
            {
                // if we are not jumping then accellerate to our target velocity
                AccellerateToDesired();
            }
        }
        else
        {
            if (GetComponent<AirstrafeController>() != null)
                GetComponent<AirstrafeController>().PerformAirstrafe(rotation, cameraTilt);

            AirControl();
        }


        // We apply gravity manually for more tuning control
        GetComponent<Rigidbody>().AddForce(Physics.gravity * 3.0f * GetComponent<Rigidbody>().mass);

    }

    private void CheckGrounded()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);

        if (Physics.Raycast(ray, out hit) && hit.distance <= 1.1)
        {
            if (onGroundTicks == 0)
            {
                // first frame of being on the ground
                inAir = false;

                Vector3 incomingVelocity = GetComponent<Rigidbody>().velocity;
                incomingVelocity.y = 0;
                incomingVel = incomingVelocity;

                //Debug.Log(incomingVel);
            }
            offGroundTicks = 0;
            onGroundTicks++;
        }
        else
        {
            if (offGroundTicks == 0)
            {
                // first frame of actually being in the air
                inAir = true;
            }
            offGroundTicks++;
            onGroundTicks = 0;
        }
    }

    private void AirControl()
    {
        Vector3 velocityChange = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        velocityChange.Normalize();
        velocityChange = transform.TransformDirection(velocityChange);
        velocityChange *= aircontrolForce;

        if (GetComponent<Rigidbody>().velocity.sqrMagnitude < airMovementMaxVelocitySq ||
            (GetComponent<Rigidbody>().velocity + velocityChange).sqrMagnitude < GetComponent<Rigidbody>().velocity.sqrMagnitude) 
        {
            GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);
        }   
    }
    

    void AccellerateToDesired()
    {
        // Calculate how fast we want to be moving
        Vector3 targetVelocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        targetVelocity.Normalize();
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = GetComponent<Rigidbody>().velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.y = 0;

        // apply acceleration at friction rate
        velocityChange *= surfaceFriction;

        // GetComponent<Rigidbody>().velocity += velocityChange * surfaceFriction;

        GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);
    }

    bool PerformJump()
    {
        if (!doJump) return false;

        doJump = false;
        if (autoBunnyhop)
        {
            if (!Input.GetButton("Jump")) return false;
            GetComponent<ActionFeedback>().EarlyBHop(0);
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            return true;
        }
        else
        {
            float timeAgo = Time.time - jumpQueuedTime;

            Vector3 newvel = GetComponent<Rigidbody>().velocity;
            if (prematureJump)
            {
                // jump was premature (fired while still airbourne)
                if (timeAgo < bunnyhopWindow / 2.0f)
                {
                    // although premature, jump was within the bhop window, so we allow it as a bhop
                    GetComponent<ActionFeedback>().EarlyBHop((int)Mathf.Round(timeAgo / Time.fixedDeltaTime));
                    
                    newvel.y = jumpForce;
                    GetComponent<Rigidbody>().velocity = newvel;
                    return true;
                }
                else
                {
                    // outside of our bhop window, so no-go
                    GetComponent<ActionFeedback>().FailedJump();
                    return false;
                }
            }
            else
            {
                // jump was performed while grounded
                if (onGroundTicks * Time.fixedDeltaTime < bunnyhopWindow / 2.0f)
                {
                    // jump was performed after we had been on the ground for some time, but still within our window
                    GetComponent<ActionFeedback>().LateBHop(onGroundTicks);

                    // the player has lost some velocity due to friction (or even gained some by moving), but we are
                    // now reverting this contact with the ground and pretending it was a bunnyhop
                    newvel = incomingVel;
                    newvel.y = jumpForce;
                    GetComponent<Rigidbody>().velocity = newvel;
                    //GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
                    return true;
                }
                else
                {
                    // player was on the ground too long to be a bunnyhop, so it's just a garden-variety jump
                    GetComponent<ActionFeedback>().Jump();
                    newvel.y = jumpForce;
                    GetComponent<Rigidbody>().velocity = newvel;
                    return true;
                }

            } 
           /* Debug.Log("jump");
            Vector3 newvel = GetComponent<Rigidbody>().velocity;
            newvel.y = jumpForce;
            GetComponent<Rigidbody>().velocity = newvel;// .AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            return true;*/
        }
    }

    void ApplyTiltClamped(float cameraTilt, float lowerLimit, float upperLimit)
    {
        float newTilt = viewCamera.transform.localEulerAngles.x + cameraTilt;
        newTilt = newTilt % 360;
        if (newTilt < 0)
        {
            newTilt += 360;
        }
        if (newTilt <= 180)
        {
            // looking down
            newTilt = Mathf.Min(lowerLimit, newTilt);
        }
        else if (newTilt != 0)
        {
            // looking up
            newTilt = Mathf.Max(upperLimit, newTilt);
        }
        viewCamera.transform.localEulerAngles = new Vector3(newTilt, 0, 0);
    }

}
