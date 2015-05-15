#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Net;
using System.Collections.Generic;
using System;
using System.IO;

public class ShowRun : EditorWindow
{

    private string LevelHash()
    {
        int accum = 0;
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.layer == LayerMask.NameToLayer("Ground"))
                accum += (int)Mathf.Round((go.transform.position.x + go.transform.position.y + go.transform.position.z + go.transform.rotation.w + go.transform.rotation.x + go.transform.rotation.y + go.transform.rotation.z) * 100);
        }
        return accum.ToString();
    }

    [MenuItem("Run Analysis/Download Run")]
    static void DownloadRun()
    {
        EditorWindow window = EditorWindow.GetWindow<ShowRun>();
        ((ShowRun)window).disp = true;
        window.Show();
    }
   
    [MenuItem("Run Analysis/Open Run")]
    static void OpenRun()
    {
        var path = EditorUtility.OpenFilePanel(
                    "Select run to load",
                    "",
                    "json");

        EditorWindow window = EditorWindow.GetWindow<ShowRun>();
        ((ShowRun)window).toDraw = (Recorder.LoadSnapshots(File.ReadAllText(path)));
    }

    [MenuItem("Run Analysis/Clear Runs")]
    static void ClearRuns()
    {
        EditorWindow window = EditorWindow.GetWindow<ShowRun>();
        ((ShowRun)window).clear = true;
        
    }

    public void Callback(object obj)
    {
        string url = "http://uint8.me:443/run/" + obj;
        using (WebClient client = new WebClient())
        {
            string data = client.DownloadString(url);
            this.toDraw=(Recorder.LoadSnapshots(data)); ;
        }
    }

    public void OnSceneGUI (SceneView scnView)
    {

     //   Handles.SetCamera(scnView.camera);
     //   Handles.DrawCamera(new Rect(0, 0, 500, 500), Camera.current);
      //  Handles.PositionHandle(Vector3.zero, Quaternion.identity);
      //   Handles.BeginGUI();
        //foreach (List<Recorder.Snapshot> r in toDraw)
        // {

      //  Gizmos.DrawCube(Vector3.zero, Vector3.one);

        if (toDraw != null)
        {
            for (int i = 1; i < toDraw.Count; i++)
            {
           //     Handles.DrawLine(toDraw[i - 1].position, toDraw[i].position);
               Debug.DrawLine(toDraw[i - 1].position, toDraw[i].position, Color.red, 0);
            }
            toDraw = null;
        }
              //   H

             //for (int i = 1; i < r.snapshotList.Count; i++)
           //  {
           ///      Debug.DrawLine(r.snapshotList[i - 1].position, r.snapshotList[i].position, Color.red);
           //  }
       //  }
     //    Handles.EndGUI();
    }

    private bool disp = false;
    private List<Recorder.Snapshot> toDraw;
    private bool clear;

    void OnFocus()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }
    
    void OnGUI()
    {
        if (disp)
        {
            disp = false;
            GenericMenu menu = new GenericMenu();

            string url = "http://uint8.me:443/runs/" + LevelHash();
            using (WebClient client = new WebClient())
            {
                string s = client.DownloadString(url);
                string[] runs = s.Split(new string[] { ", " }, StringSplitOptions.None);

                foreach(string run in runs)
                {
                    string[] parts = run.Split('|');

                    menu.AddItem(new GUIContent("Player " + parts[1] + "/" + parts[2]), false, Callback, parts[0]);
                }

                menu.ShowAsContext();
            }
        }

        if (clear)
        {
      //      toDraw.Clear();
            SceneView.RepaintAll();
        }
    }



    /* // Validated menu item.
       // Add a menu item named "Log Selected Transform Name" to MyMenu in the menu bar.
       // We use a second function to validate the menu item
       // so it will only be enabled if we have a transform selected.
       //[MenuItem("MyMenu/Log Selected Transform Name")]
       static void LogSelectedTransformName()
       {
           Debug.Log("Selected Transform is on " + Selection.activeTransform.gameObject.name + ".");
       }

       // Validate the menu item defined by the function above.
       // The menu item will be disabled if this function returns false.
       //  [MenuItem("MyMenu/Log Selected Transform Name", true)]
       static bool ValidateLogSelectedTransformName()
       {
           // Return false if no transform is selected.
           return Selection.activeTransform != null;
       }


       // Add a menu item named "Do Something with a Shortcut Key" to MyMenu in the menu bar
       // and give it a shortcut (ctrl-g on Windows, cmd-g on OS X).
       //  [MenuItem("MyMenu/Do Something with a Shortcut Key %g")]
       static void DoSomethingWithAShortcutKey()
       {
           Debug.Log("Doing something with a Shortcut Key...");
       }

       // Add a menu item called "Double Mass" to a Rigidbody's context menu.
       /* [MenuItem("CONTEXT/Rigidbody/Double Mass")]
        static void DoubleMass(MenuCommand command)
        {
            Rigidbody body = (Rigidbody)command.context;
            body.mass = body.mass * 2;
            Debug.Log("Doubled Rigidbody's Mass to " + body.mass + " from Context Menu.");
        }

        // Add a menu item to create custom GameObjects.
        // Priority 1 ensures it is grouped with the other menu items of the same kind
        // and propagated to the hierarchy dropdown and hierarch context menus. 
        [MenuItem("GameObject/MyCategory/Custom Game Object", false, 10)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("Custom Game Object");
            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }*/
}

#endif