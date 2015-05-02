using UnityEngine;
using System.Collections;

public class HeadingCorrectiveAirstrafeController : MonoBehaviour, AirstrafeController{

    public float correctionScale = 5.0f;
    [Range(0, 180)]
    public float overstrafeAngle = 80.0f;

    public float baseOverstrafeDecay = 0.1f;
    public float multiplicativeOverstrafeDecay = 0.01f;
    public float squaredOverstrafeDecay = 0.0f;
    public float maxDecay = 40;

    public bool PerformAirstrafe(float horizontalAxis, float verticalAxis)
    {
        
        Rigidbody rigidbody = GetComponent<Rigidbody>();

        Vector3 velocityForward = rigidbody.velocity;
        velocityForward.y = 0;
        if (velocityForward.magnitude < 1)
            return false;

        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        float correctionAngle = Vector3.Angle(forward, velocityForward.normalized);
        float correctionFactor = Vector3.Dot(Vector3.up, Vector3.Cross(velocityForward.normalized, forward));

        velocityForward = Quaternion.Euler(0, correctionFactor * correctionScale, 0) * velocityForward;
        if (correctionAngle > 5)
        {
            GetComponent<ActionFeedback>().Strafe();
        }
        if (correctionAngle > overstrafeAngle)
        {
            GetComponent<ActionFeedback>().ImperfectStrafe();
            GetComponent<DebugMovement>().clampedStrafe = true;

            float overstrafe = (correctionAngle - overstrafeAngle)/360.0f;
            float decay = baseOverstrafeDecay + overstrafe * multiplicativeOverstrafeDecay + Mathf.Pow(overstrafe, 2) * squaredOverstrafeDecay;
            decay = Mathf.Min(maxDecay, decay);
            decay *= Time.deltaTime;
            velocityForward *= (1-decay);
        }
        else
        {
            GetComponent<DebugMovement>().clampedStrafe = false;
        }

        rigidbody.velocity = new Vector3(velocityForward.x, rigidbody.velocity.y, velocityForward.z);
        return true;
    }
}
