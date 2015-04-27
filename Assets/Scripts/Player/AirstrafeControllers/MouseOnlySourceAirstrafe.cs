using UnityEngine;
using System.Collections;

public class MouseOnlySourceAirstrafe : MonoBehaviour, AirstrafeController {

    public float maxStrafeSpeed = 4;
    public float airAccelerate = 4;

    public bool PerformAirstrafe(float horizontalAxis, float verticalAxis)
    {
        Vector3 localVelocity = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);
        float x = localVelocity.x;

        float dxMag = airAccelerate;
        int dxSign = 1;
        if (horizontalAxis < 0)
            dxSign = -1;

        // clamp the acclelleration so that we cannot accellerate over
        // maxStrafeSpeed using airstrafings
        float nx = x + dxSign * dxMag;
        if (Mathf.Abs(nx) > maxStrafeSpeed)
        {
            dxMag = Mathf.Max(maxStrafeSpeed - Mathf.Abs(x), 0);
        }


        if (horizontalAxis != 0)
        {
            localVelocity.x += dxSign * dxMag;

            GetComponent<DebugMovement>().clampedStrafe = dxMag == 0;
            GetComponent<Rigidbody>().velocity = transform.TransformDirection(localVelocity);

            return true;

        }

        return false;
    }
}
