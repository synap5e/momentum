using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

public class DebugMovement : MonoBehaviour
{

    public float trailDuration = 10F;

    public Vector3 airstrafeAccellerationDirection
    {
        get;
        set;
    }

    public bool clampedStrafe
    {
        get;
        set;
    }

    void FixedUpdate()
    {
        if (!enabled) return;

        // facing direction
        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.blue);

        if (GetComponent<HeadingCorrectiveAirstrafeController>() != null)
        {
            // show arc we can turn within without losing velocity

            float angle = GetComponent<HeadingCorrectiveAirstrafeController>().maximumConservationStrafeAngle;
            Vector3 vel = GetComponent<Rigidbody>().velocity;
            vel.y = 0;
            vel.Normalize();

            Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, -angle, 0) * vel, Color.cyan);
            Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, angle, 0) * vel, Color.cyan);
        }

        Color trailColor = Color.black;

        if (! GetComponent<RigidbodyFPSController>().onGround)
        {
            trailColor = Color.red;
            if (clampedStrafe) trailColor = Color.cyan;
        }
        Debug.DrawLine(transform.position, transform.position + GetComponent<Rigidbody>().velocity * Time.deltaTime, trailColor, trailDuration);
        
       

        Vector3 rv = GetComponent<Rigidbody>().velocity;
        rv.y = 0;
        Debug.DrawLine(transform.position, transform.position + rv, Color.green);


        //Debug.DrawLine(transform.position, transform.position + airstrafeAccellerationDirection.normalized, Color.blue);
    }
}
