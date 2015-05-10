using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BombController : MonoBehaviour
{

    public float force = 20;
    [Range(0, 10)]
    public float zeroFalloffDistance = 5;
    [Range(0, 10)]
    public float falloffDistance = 2;

    public float SolveForce(float distance)
    {
        // https://www.desmos.com/calculator/r4uuqtr9gw
        return Mathf.Max(0, force * (Mathf.Abs(distance - zeroFalloffDistance)) / (-distance + zeroFalloffDistance), -(force/Mathf.Pow(falloffDistance, 2)) * Mathf.Pow(distance - zeroFalloffDistance, 2) + force);
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(BombController))]
    public class BombControllerInspector : Editor
    {

         public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Texture2D tex = new Texture2D(301, 101);
            float xscale = 0.034f;
            float yscale = 2;
            for (int x = 0; x < 300; x++)
            {
                int y = (int)(((BombController)this.target).SolveForce(x * xscale) * yscale);
                tex.SetPixel(x, y, Color.black);
            }
            tex.Apply();
            GUILayout.Box(tex);
            
        }
    }
    #endif

    [Tooltip("Whether to nullify the y velocity of a player if they are falling and this explosive force is opposed to their fall. " +
             "Usefull for making pogos consistent and preventing having to use large forces (that may act horizintally too) to break a fall")]
    public bool nullifyFall = true;

    public float respawnTime = 0.7f;

    private bool detonated = false;
    private float detonatedTime;

    void Awake()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }

    void Update()
    {
        if (detonated && Time.time > detonatedTime + respawnTime)
        {
            Respawn();
        }
    }

    internal void Select()
    {
        GetComponent<Renderer>().material.color = Color.green;
        // TODO
    }

    internal void Deselect()
    {
        // TODO
        GetComponent<Renderer>().material.color = Color.white;
    }

    internal void Detonate()
    {
        // TODO: animations and stuffs
        detonated = true;
        detonatedTime = Time.time;

        GetComponent<Renderer>().enabled = false;
    }

    internal void Respawn()
    {
        detonated = false;
        GetComponent<Renderer>().enabled = true;
    }

    public bool detonatable
    {
        get
        {
            return !detonated;
        }
    }
}
