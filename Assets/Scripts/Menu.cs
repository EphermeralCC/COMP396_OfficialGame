using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour
{
    public Canvas insMenu;
    public GameObject startButton;
    public GameObject exitButton;
    public GameObject insButton;
    public GameObject refreshButton;
    public GameObject serverStartButton;
    public GameObject roomButton;

    public GUISkin skin;

    private ServerManager svrmgr;
    private GameManager gm;

    void OnGUI()
    {
       
        GUI.skin = skin;
    }

    // Use this for initialization
    void Start()
    {
        insMenu = insMenu.GetComponent<Canvas>();
        insMenu.enabled = false;
        refreshButton.GetComponent<Text>().enabled = false;
        serverStartButton.GetComponent<Text>().enabled = false;
        roomButton.GetComponent<Text>().enabled = false;

        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        svrmgr = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<ServerManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InstructionPress()
    {
        insMenu.enabled = true;
        startButton.GetComponent<Text>().enabled = false;
        exitButton.GetComponent<Text>().enabled = false;
        insButton.GetComponent<Text>().enabled = false;
        refreshButton.GetComponent<Text>().enabled = false;
        serverStartButton.GetComponent<Text>().enabled = false;
        roomButton.GetComponent<Text>().enabled = false;
    }
    public void ContinuePress()
    {
        insMenu.enabled = false;
        startButton.GetComponent<Text>().enabled = true;
        exitButton.GetComponent<Text>().enabled = true;
        insButton.GetComponent<Text>().enabled = true;
        refreshButton.GetComponent<Text>().enabled = false;
        serverStartButton.GetComponent<Text>().enabled = false;
        roomButton.GetComponent<Text>().enabled = false;
    }

    public void ExitPress()
    {
        Application.Quit();
    }

    public void PlayPress()
    {
        insMenu.enabled = false;
        startButton.GetComponent<Text>().enabled = false;
        exitButton.GetComponent<Text>().enabled = false;
        insButton.GetComponent<Text>().enabled = false;
        refreshButton.GetComponent<Text>().enabled = true;
        serverStartButton.GetComponent<Text>().enabled = true;
        roomButton.GetComponent<Text>().enabled = false;
    }

    public void RefreshPress()
    {
        insMenu.enabled = false;
        startButton.GetComponent<Text>().enabled = false;
        exitButton.GetComponent<Text>().enabled = false;
        insButton.GetComponent<Text>().enabled = false;
        refreshButton.GetComponent<Text>().enabled = true;

        svrmgr.RefreshHostList();

        if (!Network.isClient && !Network.isServer)
        {
            if (ServerManager.hostList != null)
            {
                roomButton.GetComponent<Text>().enabled = true;
                roomButton.GetComponent<Text>().text = ServerManager.hostList[0].gameName;

            }
        }
    }

    public void StartServerPress()
    {
        svrmgr.StartServer();
        gm.CompleteLevel();
    }

    public void RoomPress()
    {
        svrmgr.JoinServer(ServerManager.hostList[0]);
        gm.CompleteLevel();
    }
}
