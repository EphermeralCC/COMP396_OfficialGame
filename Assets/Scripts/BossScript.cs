using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class BossScript : MonoBehaviour
{
	
	public enum FSMState
	{
		Patrol,
		Chase
	}
	
	public FSMState curState;
	public string enemyTagPoints; //Tag of enemy waypoints
	public float speedMin; //Enemy Speed
	public float speedMax; //Enemy Speed

	private GameObject[] pointsEnemyWalk;
	private GameObject[] objPlayers;
	private GameObject objPlayer;
	private int indexPoint;

	private int randomPlayerIdx;
	private float[] playerDist;
	

	
	// Use this for initialization
	void Start()
	{
		Initialize();
	}

	protected void Initialize()
	{
		
		if (enemyTagPoints != "")
		{
			pointsEnemyWalk = GameObject.FindGameObjectsWithTag(enemyTagPoints);
		}
		indexPoint = 0;

		StartCoroutine( RandomBehavior ());

	}

	IEnumerator RandomBehavior(){

		while(true){
			curState = ( UnityEngine.Random.Range (0,2) == 1 ? FSMState.Patrol : FSMState.Chase);
			yield return new WaitForSeconds(UnityEngine.Random.Range (4,8));
		}	
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		objPlayers = GameObject.FindGameObjectsWithTag("Player");
		if (objPlayers.Length == 0)
		{
			objPlayer = GameObject.FindGameObjectWithTag("DummyPlayer");
		}
		else
		{
			playerDist = new float[objPlayers.Length];
			for (int i = 0; i < objPlayers.Length; i ++)
			{
				playerDist[i] = Vector3.Distance(transform.position, objPlayers[i].transform.position);
			}
			
			objPlayer = objPlayers[Array.IndexOf(playerDist, playerDist.Min())];
			
		}
		
		
		switch (curState)
		{
		case FSMState.Patrol: ExecutePatrolState(); break;
		case FSMState.Chase: ExecuteChaseState(); break;
		}
		
	}
	
		
	protected void ExecutePatrolState()
	{
		Quaternion targetRotation = Quaternion.LookRotation(pointsEnemyWalk[indexPoint].transform.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);
		transform.Translate(Vector3.forward * Time.deltaTime * 15f);
		
		if (Vector3.Distance(transform.position, pointsEnemyWalk[indexPoint].transform.position) < 0.5f)
		{
			indexPoint = (indexPoint != pointsEnemyWalk.Length - 1) ? indexPoint + 1 : 0;
		}

	}

	
	protected void ExecuteChaseState()
	{
		//Debug.Log ("ChaseState");
		
		Quaternion targetRotation = Quaternion.LookRotation(objPlayer.transform.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);
		transform.Translate(Vector3.forward * Time.deltaTime * UnityEngine.Random.Range (speedMin,speedMax));

	}
	

	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		if (stream.isWriting)
		{
			syncPosition = transform.GetComponent<Rigidbody>().position;
			stream.Serialize(ref syncPosition);
		}
		else
		{
			stream.Serialize(ref syncPosition);
			transform.GetComponent<Rigidbody>().position = syncPosition;
		}
	}
	
}
