using UnityEngine;
using System.Collections;

public class HeadingCorrectiveAirstrafeController : MonoBehaviour, AirstrafeController{

    public float correctionScale = 1.0f;
    [Range(0, 90)]
    public float maximumConservationStrafeAngle = 80.0f;

    public bool PerformAirstrafe(float horizontalAxis, float verticalAxis)
    {
        
        Rigidbody rigidBody = GetComponent<Rigidbody>();

        Vector3 velocityForward = rigidBody.velocity;
        velocityForward.y = 0;
        if (velocityForward.magnitude == 0)
            return false;

        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        float correctionAngle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(forward, velocityForward.normalized));

        // correction vector from the direction we are facing to the direction we want to be facing
        // magnitude proportional to the difference
        Vector3 correction = (forward - velocityForward.normalized) * correctionScale;

        // new velocity (excluding y component)
        Vector3 newVelocityForward;

        if (correctionAngle < maximumConservationStrafeAngle)
        {
            // conserve velocity
            newVelocityForward = velocityForward + correction;
            newVelocityForward *= (velocityForward.magnitude / newVelocityForward.magnitude);
            if (correction.sqrMagnitude > 0.1)
            {
                GetComponent<ActionFeedback>().Strafe();
            }
            GetComponent<DebugMovement>().clampedStrafe = false;
        }
        else
        {
            GetComponent<ActionFeedback>().ImperfectStrafe();
            GetComponent<DebugMovement>().clampedStrafe = true;

            newVelocityForward = velocityForward * 0.9f + correction;
           // newVelocityForward -= correction * 0.9f;
        }

        // use original y component
        newVelocityForward.y = rigidBody.velocity.y;

        rigidBody.velocity = newVelocityForward;

        return true; // TODO
    }
}
