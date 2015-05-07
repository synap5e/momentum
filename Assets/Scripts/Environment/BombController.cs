using UnityEngine;
using System.Collections;

public class BombController : MonoBehaviour
{
    public float force = 20;
    
    [Tooltip("Whether to nullify the y velocity of a player if they are falling and this explosive force is opposed to their fall. " +
             "Usefull for making pogos consistent and preventing having to use large forces (that may act horizintally too) to break a fall")]
    public bool nullifyFall = true;

    [Tooltip("Falloff radius, knockback is interpoltated from force to 0 as the distance to the explosive increases to radius. " + 
             "If greater than the activation radius (in the player's BombActivator) then the force at radius will be non-0, but 0 beyond that distance. " +
             "If 0 there is no falloff, which can increase the consistency jumps using the explosion.")]
    public float radius = 0;

    public float respawnTime = 0.7f;

    private bool detonated = false;
    private float detonatedTime;

    void Awake()
    {
        GetComponent<Renderer>().material.color = Color.green;
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
        GetComponent<Renderer>().material.color = Color.red;
        // TODO
    }

    internal void Deselect()
    {
        // TODO
        GetComponent<Renderer>().material.color = Color.green;
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
