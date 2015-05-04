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
            if (bombObject.activeSelf && dist < closestDist)
            {
                closestBomb = bombObject;
                closestDist = dist;
            }
        }

        if (closestBomb == null && selectedBomb != null)
        {
            selectedBomb.GetComponent<BombController>().deselect();
            selectedBomb = null;
        }
        if (closestBomb != null && closestBomb != selectedBomb)
        {
            if (selectedBomb != null)
            {
                selectedBomb.GetComponent<BombController>().deselect();
            }
            selectedBomb = closestBomb;
            selectedBomb.GetComponent<BombController>().select();
           
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
                selectedBomb.SetActive(false);
                BombController bombController = selectedBomb.GetComponent<BombController>();

                Vector3 forceDirection = transform.position - selectedBomb.transform.position;

                float force;
                if (bombController.radius == 0)
                {
                    force = bombController.force;
                } else
                {
                    force = Mathf.Lerp(0, bombController.force, 1.0f - (forceDirection.magnitude / bombController.radius));
                }
                forceDirection = forceDirection.normalized * force;

                Vector3 oldvel = GetComponent<Rigidbody>().velocity;
                if (bombController.nullifyFall && forceDirection.y > 10)
                {
                    oldvel.y = 0;
                }
                oldvel += forceDirection;
                GetComponent<Rigidbody>().velocity = oldvel;

				ReactivateBombs();
            }
        }
    }

    internal void ReactivateBombs()
    {
        foreach (BombController b in bombs)
        {
            b.gameObject.SetActive(true);
        }
    }
}
