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

    void Awake()
    {
        GetComponent<Renderer>().material.color = Color.green;
    }

    internal void select()
    {
        GetComponent<Renderer>().material.color = Color.red;
        // TODO
    }

    internal void deselect()
    {
        // TODO
        GetComponent<Renderer>().material.color = Color.green;
    }


}
