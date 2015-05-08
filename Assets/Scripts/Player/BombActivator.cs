using UnityEngine;
using System.Collections;

public class BombActivator : MonoBehaviour
{

    public float activationRadius = 10.0f;

    private GameObject selectedBomb = null;
    private bool fire = false;
    private BombController[] bombs;

    void Awake()
    {
        bombs = GameObject.FindObjectsOfType(typeof(BombController)) as BombController[];
    }

    void Update()
    {
        float closestDist = float.PositiveInfinity;
        GameObject closestBomb = null;
        foreach (Collider b in Physics.OverlapSphere(transform.position, activationRadius, 1 << LayerMask.NameToLayer("Bombs")))
        {
            GameObject bombObject = b.gameObject;
            float dist = (transform.position - bombObject.transform.position).sqrMagnitude;
            if (bombObject.GetComponent<BombController>().detonatable && dist < closestDist)
            {
                closestBomb = bombObject;
                closestDist = dist;
            }
        }

        if (closestBomb == null && selectedBomb != null)
        {
            selectedBomb.GetComponent<BombController>().Deselect();
            selectedBomb = null;
        }
        if (closestBomb != null && closestBomb != selectedBomb)
        {
            if (selectedBomb != null)
            {
                selectedBomb.GetComponent<BombController>().Deselect();
            }
            selectedBomb = closestBomb;
            selectedBomb.GetComponent<BombController>().Select();
           
        }

        if (Input.GetMouseButtonDown(0))
        {
            fire = true;
        }
    }

    void FixedUpdate()
    {
        if (fire)
        {
            fire = false;
            if (selectedBomb != null)
            {
                BombController bombController = selectedBomb.GetComponent<BombController>();

                Vector3 forceDirection = (transform.position + Vector3.up) - selectedBomb.transform.position;

                float force = bombController.SolveForce(forceDirection.magnitude);
                if (force > 0)
                {
                    bombController.Detonate();

                    forceDirection = forceDirection.normalized * force;

                    Vector3 oldvel = GetComponent<Rigidbody>().velocity;
                    if (bombController.nullifyFall && forceDirection.y > 10)
                    {
                        oldvel.y = 0;
                    }
                    oldvel += forceDirection;
                    GetComponent<Rigidbody>().velocity = oldvel;
                }

            }
        }
    }

    internal void ReactivateBombs()
    {
        foreach (BombController b in bombs)
        {
            b.Respawn();
        }
    }
}
