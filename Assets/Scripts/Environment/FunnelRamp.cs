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
            return transform.position + transform.forward * transform.localScale.z / 2;
        }
    }

    [CustomEditor(typeof(FunnelRamp))]
    public class FunnelRampInspector : Editor
    {

        private int id = 0;

        private void OnSceneGUI()
        {
            Handles.color = Color.yellow;

            Debug.Log(target);
            FunnelRamp ramp = (FunnelRamp)target;


            DrawArrow(ramp.transform.position, ramp.ExitPoint);
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

        float along = playerLocal.z - enterLocal.z;
       // Debug.Log(along);

        playerLocal.x = Mathf.Lerp(enterLocal.x, ExitPoint.x, along);

        player.transform.position = transform.TransformPoint(playerLocal);
        player.GetComponent<Rigidbody>().velocity = Vector3.Lerp(enterVelocity, transform.forward * exitVelocity, along);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.enterPoint = collision.contacts[0].point;
            this.enterVelocity = collision.gameObject.GetComponent<Rigidbody>().velocity;
            Debug.Log((collision.collider.gameObject.transform.position - ExitPoint).magnitude);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log((collision.collider.gameObject.transform.position - ExitPoint).magnitude);
        }
    }
}
