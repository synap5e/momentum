using UnityEngine;
using System.Collections;

using UnityEditor;

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

    [CustomEditor(typeof(FunnelRamp))]
    public class FunnelRampInspector : Editor
    {

        private int id = 0;

        private void OnSceneGUI()
        {
            Handles.color = Color.yellow;

            FunnelRamp ramp = (FunnelRamp)target;

			DrawArrow(ramp.transform.position, ramp.ExitPoint);
            Handles.SphereCap(id++, ramp.ExitPoint, Quaternion.identity, 0.75f);

			Vector3 vel = ramp.transform.forward * ramp.exitVelocity;
			Vector3 prev = ramp.ExitPoint;
			for (float x=0; x<300; x++) 
			{
				Handles.DrawLine(prev, prev = prev + vel * 0.01f);
				vel.y -= 0.35f;
			}
            //DrawArrow(ramp.transform.position + Vector3.Cross(ramp.transform.position, ramp.transform.up).normalized * ramp.transform.localScale.x/3, ramp.ExitPoint);
        }

        private void DrawArrow(Vector3 start, Vector3 end)
        {
            Handles.ArrowCap(id++, start, Quaternion.LookRotation(end - start), (end - start).magnitude - 1);
        }
    }


    internal void Accelerate(GameObject player)
    {
        Vector3 enterLocal = transform.InverseTransformPoint(enterPoint);
        Vector3 playerLocal = transform.InverseTransformPoint(player.transform.position);
        Vector3 exitLocal = transform.InverseTransformPoint(ExitPoint);

        float along = (playerLocal.z - enterLocal.z) / (exitLocal.z - enterLocal.z);
        Debug.Log(along);
		Debug.Log (playerLocal.x + ", " + exitLocal.x);

		playerLocal.x = Mathf.Lerp(enterLocal.x, exitLocal.x, along);

        player.transform.position = transform.TransformPoint(playerLocal);
        //player.transform.position = transform.TransformPoint(exitLocal);

        Debug.Log(enterLocal + " ? " + enterPoint);

        player.GetComponent<Rigidbody>().velocity = Vector3.Lerp(enterVelocity, transform.forward * exitVelocity, along);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision);
        if (collision.gameObject.tag == "Player")
        {
           
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log((collision.collider.gameObject.transform.position - ExitPoint).magnitude);
        }
    }

    internal void EnterRamp(GameObject player)
    {
        this.enterPoint = player.transform.position;
        this.enterVelocity = player.GetComponent<Rigidbody>().velocity;
        Debug.Log((player.transform.position - ExitPoint).magnitude);
    }
}
