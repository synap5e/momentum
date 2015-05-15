using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

    #if UNITY_EDITOR
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
    #endif

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

			player.GetComponent<Rigidbody> ().velocity = Vector3.Lerp (enterVelocity, transform.forward * exitVelocity, Mathf.Max(0.1f, along));


		}
    }

    internal void EnterRamp(GameObject player)
    {
        this.enterPoint = player.transform.position;
        this.enterVelocity = player.GetComponent<Rigidbody>().velocity;
        Debug.Log((player.transform.position - ExitPoint).magnitude);
    }
}
