using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneLint
{

    private bool pause = false;

    public static void Lint()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.tag == "Ground" || go.tag == "Low Friction Ground" || go.tag == "High Friction Ground")
            {
                Require(go, go.layer == LayerMask.NameToLayer("Ground"), "An object tagged as ground must be in the ground layer");
            }

            if (go.tag == "Instant Kill")
            {
                Suggest(go, go.layer != LayerMask.NameToLayer("Ground"), "An instant kill object should not be ground, or it may allow frame-perfect bunnyhopping");
            }

            if (go.tag == "Delayed Kill")
            {
                Suggest(go, go.layer == LayerMask.NameToLayer("Ground"), "An object acting as delayed kill is usually used to allow BHing which requires being in the Ground <i>layer</i>");
            }

            if (go.GetComponent<Checkpoint>() != null)
            {
                Suggest(go, go.GetComponent<Checkpoint>().spawn != null, "Checkpoints should specify their spawn points explicitly");
            }

            if (go.GetComponent<BombController>() != null)
            {
                Require(go, go.layer == LayerMask.NameToLayer("Bombs"), "A bomb object must be in the Bombs layer");
                Suggest(go, go.GetComponent<BombController>().respawnTime > 0.5, "A bomb with a respawn time of less than 500ms may allow exploits");
            }
            if (go.layer == LayerMask.NameToLayer("Bombs"))
            {
                Require(go, go.GetComponent<BombController>() != null, "A bomb object should have a BombController");
            }
        }
    }

    private static void Suggest(GameObject g, bool p1, string p2)
    {
        if (!p1)
        {
            Debug.LogWarning("Suggestion: " + p2);
            Debug.Log(g);
      //      UnityEditor.Selection.activeGameObject = g;
        }
    }

    private static void Require(GameObject g, bool p1, string p2)
    {
        if (!p1)
        {
            Debug.LogError("Requirement: " + p2);
            Debug.Log(g);
     //       UnityEditor.Selection.activeGameObject = g;
            Application.Quit();
        }
    }

}
