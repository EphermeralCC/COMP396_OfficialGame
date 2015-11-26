using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public int currentScore;
    //public int highScore;
    private int currentLevel = 0;
    public int unlockedLevel;

    public GameObject deathParent;
    public static int keyCount = 0;
    public static int totalKeyCount = 0;
    public static int numPlayerInExit = 0;
    public static GameObject[] players;  

    public GUISkin skin;
    public GameObject playerPrefab;
    private bool paused = false;

    private Rect resumeButton = new Rect(150, 100, 200, 80);
    private Rect quitButton = new Rect(150, 210, 200, 80);

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        GameManager.totalKeyCount = 0;
        GameManager.keyCount = 0;
        GameManager.numPlayerInExit = 0;
        Network.Instantiate(playerPrefab, new Vector3(20f, 0.5f, -22f), Quaternion.identity, 0);
	}
	
	// Update is called once per frame
	void Update () {
        players = GameObject.FindGameObjectsWithTag("Player");

        if (Input.GetKeyDown(KeyCode.P))
        {
            paused = true;
            Time.timeScale = 0;
            Debug.Log("Where");
        }
	}

    public void CompleteLevel()
    {
        Debug.Log("current level: " + currentLevel);
        currentLevel += 1;
        Application.LoadLevel(currentLevel);
        Debug.Log("next level: " + currentLevel);
       // NetworkLevelLoader.Instance.LoadLevel("Level 2", currentLevel);
    }

    void OnGUI()
    {
        GUI.skin = skin;
        GUI.Label(new Rect(20, 20, 200, 200), "Total Keys Collect: " + keyCount.ToString() + "/" + totalKeyCount.ToString());
        GUI.Label(new Rect(750, 20, 200, 200), "Press 'P' to PAUSE");

        if (paused == true)
        {
            if (GUI.Button(resumeButton, "Resume"))
            {
                Debug.Log("Paused");
                paused = false;
                Time.timeScale = 1.0f;
            }

            if (GUI.Button(quitButton, "Quit"))
            {
                Application.Quit();
            }
        }
    }
}
