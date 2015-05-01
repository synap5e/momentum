using UnityEngine;
using System.Collections;

public class ActionFeedback : MonoBehaviour {
    internal void EarlyBHop(int t)
    {
        Debug.Log("early by " + t);
    }
    internal void LateBHop(int t)
    {
        Debug.Log("late by " + t);
    }

    internal void FailedBHop()
    {
   //     Debug.Log("FailedBHop");
    }

    internal void Jump()
    {
    //    Debug.Log("Jump");
    }

    internal void FailedJump()
    {
   //     Debug.Log("FailedJump");
    }

    internal void ImperfectStrafe()
    {
   //     Debug.Log("Imperfect strafe");
    }

    internal void Strafe()
    {
    //    Debug.Log("Strafe");
    }


}
