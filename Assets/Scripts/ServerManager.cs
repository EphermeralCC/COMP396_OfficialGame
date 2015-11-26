using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour {

	private const string typeName = "UniqueGameName-Room";
    private const string gameName = "In the room";

    //public GameObject key;
    //private int randomIdx;
	
	public void StartServer()
	{
		Network.InitializeServer(3, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}

    void Start()
    {
        //MasterServer.ipAddress = "192.168.1.100";
    }
	
	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
        //SpawnPlayer();
	}
	
    //void OnGUI()
    //{
    //    if (!Network.isClient && !Network.isServer)
    //    {
    //        if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
    //            StartServer();

    //        if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
    //            RefreshHostList();

    //        if (hostList != null)
    //        {
    //            for (int i = 0; i < hostList.Length; i++)
    //            {
    //                if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
    //                    JoinServer(hostList[i]);
    //            }
    //        }
    //    }
    //}

	public static HostData[] hostList;
	
	public void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	public void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}
	
	void OnConnectedToServer()
	{
		Debug.Log("Server Joined");
        SpawnPlayer();
	}


    public GameObject playerPrefab;

    public void SpawnPlayer()
    {
        Network.Instantiate(playerPrefab, new Vector3(20f, 0.5f, -22.5f), Quaternion.identity, 0);
    }
}
