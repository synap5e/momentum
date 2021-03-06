using UnityEngine;
using System.Collections;
using System;

public class Goal : MonoBehaviour
{

    public bool submitRuns = false;
    public string postRunURL = "http://uint8.me:8196/submit_run";

    public enum Mode { Normal, Speedrun, Practice };
    public Mode playMode;

    public float speedrunCountdownTime = 0.5f;

    public GUIStyle announceText;
    public GUIStyle timeText;

    public GameObject ghostPrefab;

    static public bool paused = false;

    private float countdownRemaining;
    private float time;

    private bool complete = false;

    public bool newPlayerPerSession = false;

    private Vector3 spos;
    private Quaternion srot;

    private int playerIndex = 0;

    private GameObject player;

    private GameObject gui;



    RigidbodyFPSController playerController
    {
        get
        {
            return GameObject.FindGameObjectWithTag("Player").GetComponent<RigidbodyFPSController>();
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        this.spos = playerController.transform.position;
        this.srot = playerController.transform.rotation;
        if (MainMenu_Controller.currentMode == 0)
        {
            Debug.Log("Playing Normal Mode");
            playMode = Mode.Normal;
        }
        else
        {
            Debug.Log("Playing Speedrun Mode");
            playMode = Mode.Speedrun;
        }

        gui = GameObject.FindGameObjectWithTag("GUI");

        if (playMode == Mode.Speedrun)
        {
            playerController.enableInput = false;
            RestartSpeedrun();
            gui.SetActive(true);
        }
        else
        {
            playerController.GetComponent<Recorder>().StartRecording();
            gui.SetActive(false);
        }
        playerController.GetComponent<Recorder>().StartLoggingKeys();

        if (newPlayerPerSession)
        {
            playerIndex = UnityEngine.Random.Range(0, 1000000);
        }
    }

    //    void OnGUI()
    //    {
    //        // copy the "label" style from the current skin
    //        Rect s = new Rect(0, 0, Screen.width, Screen.height);
    //
    //        if (playMode == Mode.Speedrun)
    //        {
    //            if (complete)
    //            {
    //                GUI.Label(s, time.ToString("#.###") + "s", announceText);
    //            }
    //            else if (playerController.enableInput)
    //            {
    //                GUI.Label(s, time.ToString("#.##") + "s", timeText);
    //            }
    //            else
    //            {
    //                GUI.Label(s, countdownRemaining.ToString("#.##") + "s", announceText);
    //            }
    //        }
    //    }

    void Update()
    {
        if (!paused)
        {
            if (playMode == Mode.Speedrun)
            {
                if (Input.GetButtonDown("Restart Level"))
                {
                    player.GetComponent<RespawnController>().Restart();
                    RestartSpeedrun();
                }
                if (playerController.enableInput)
                {
                    if (!complete)
                    {
                        time += Time.deltaTime;
                    }
                }
                else
                {
                    countdownRemaining -= Time.deltaTime;
                    if (countdownRemaining <= 0)
                    {
                        playerController.enableInput = true;
                    }

                }
            }

            if (Input.GetKeyDown(KeyCode.Home))
            {
                playerController.GetComponent<Rigidbody>().velocity = Vector3.zero;
                playerController.transform.position = spos;
                playerController.transform.rotation = srot;

                playerController.GetComponent<Recorder>().StopRecording();

                string run = playerController.GetComponent<Recorder>().SaveToString();
                if (submitRuns)
                {
                    StartCoroutine(PostRun(run));
                }

                playerController.GetComponent<Recorder>().ResetRecording();
            }
            if (Input.GetKeyDown(KeyCode.End))
            {
                playerController.GetComponent<Recorder>().StopRecording();

                string run = playerController.GetComponent<Recorder>().SaveToString();
                if (submitRuns)
                {
                    StartCoroutine(PostRun(run));
                }

                playerController.GetComponent<Recorder>().ResetRecording();
            }

            if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.PageDown))
            {
                playerIndex++;
            }
        }
    }

    public void RestartSpeedrun()
    {

        if (playMode == Mode.Speedrun)
        {
            countdownRemaining = speedrunCountdownTime;
            playerController.enableInput = false;
            complete = false;
            time = 0;
            playerController.GetComponent<Rigidbody>().velocity = Vector3.zero;
            playerController.transform.position = spos;
            playerController.transform.rotation = srot;
            playerController.GetComponent<Recorder>().ResetRecording();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            complete = true;
            playerController.GetComponent<Recorder>().StopRecording();

            string run = playerController.GetComponent<Recorder>().SaveToString();

            if (playMode == Mode.Speedrun)
            {
                player.GetComponent<Pause>().setEndLevelText("Time " + playerController.GetComponent<Recorder>().getTimeString());
            }
            else
                player.GetComponent<Pause>().setEndLevelText("Finish");
            player.GetComponent<Pause>().EndLevel();

            if (submitRuns)
            {
                StartCoroutine(PostRun(run));
            }
            playerController.GetComponent<Recorder>().ResetRecording();

        }
    }

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



    private IEnumerator PostRun(string run)
    {
        WWWForm form = new WWWForm();

        form.AddField("deviceUniqueIdentifier", SystemInfo.deviceUniqueIdentifier + playerIndex);
        form.AddField("systemInfo", SystemInfo.deviceName);
        form.AddField("deviceModel", SystemInfo.deviceModel);
        form.AddField("token", "jmLq5oA59pIu9Icg");


        form.AddField("level", Application.loadedLevelName);
        form.AddField("levelHash", LevelHash());
        form.AddField("mode", playMode.ToString());

        form.AddField("recording", run);

        if (postRunURL != null)
        {
            WWW w = new WWW(postRunURL, form);
            yield return w; // Better thread non-block?
        }
        yield return null;
    }


}