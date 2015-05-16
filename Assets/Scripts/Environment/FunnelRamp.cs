using UnityEngine;
using System.Collections;

//using UnityEditor;

public class FunnelRamp : MonoBehaviour
{

    public float exitVelocity = 50;

    private Vector3 enterPoint;
    private Vector3 enterVelocity;

    private Vector3 ExitPoint
    {
        get
        {
            return transform.position + transform.forward * transform.localScale.z / 2 + transform.up * transform.localScale.y/2;
        }
    }

  


    internal void Accelerate(GameObject player)
    {
        Vector3 enterLocal = transform.InverseTransformPoint(enterPoint);
        Vector3 playerLocal = transform.InverseTransformPoint(player.transform.position);
        Vector3 exitLocal = transform.InverseTransformPoint(ExitPoint);


		Debug.Log (Mathf.Abs (playerLocal.x) + " < " + transform.localScale.x);
		if (Mathf.Abs (playerLocal.x) < 0.4) 
{

			float along = (playerLocal.z - enterLocal.z) / (exitLocal.z - enterLocal.z);
			Debug.Log (along);
			//Debug.Log (playerLocal.x + ", " + Vector3.Cross (transform.up, transform.forward) * transform.localScale.x);

			playerLocal.x = Mathf.Lerp (enterLocal.x, exitLocal.x, along);

			player.transform.position = transform.TransformPoint (playerLocal);
			//player.transform.position = transform.TransformPoint(exitLocal);

			//Debug.Log (enterLocal + " ? " + enterPoint);

			player.GetComponent<Rigidbody> ().velocity = Vector3.Lerp (enterVelocity, transform.forward * exitVelocity, along);


		}
    }

    internal void EnterRamp(GameObject player)
    {
        this.enterPoint = player.transform.position;
        this.enterVelocity = player.GetComponent<Rigidbody>().velocity;
        Debug.Log((player.transform.position - ExitPoint).magnitude);
    }
}
