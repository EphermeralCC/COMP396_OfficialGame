using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class EnemyScript : MonoBehaviour
{
	
	public enum FSMState
	{
		None,
		Patrol,
		Look,
		Chase,
		Attack,
		Dead,
		Run
	}
	
	public FSMState curState;
	public string enemyTagPoints; //Tag of enemy waypoints
	public float speed; //Enemy Speed
	public bool isAgressive; //Chase player
	public bool isTurret; //Look and attack
	public GameObject bullet; //Prefab which will be shoot
	public Transform bulletSpawn; //BulletSpawn Object in turret
	public float fireRate; //Turret Fire rate
	
	private GameObject[] pointsEnemyWalk;
	private GameObject[] objPlayers;
	private GameObject objPlayer;
	private int indexPoint;
	private float nextFire;
	
	private int randomPlayerIdx;
	private float[] playerDist;
	
	protected void Initialize()
	{
		
		if (enemyTagPoints != "")
		{
			pointsEnemyWalk = GameObject.FindGameObjectsWithTag(enemyTagPoints);
		}
		indexPoint = 0;
	}
	
	// Use this for initialization
	void Start()
	{
		Initialize();
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
		case FSMState.None: ExecuteNoneState(); break;
		case FSMState.Patrol: ExecutePatrolState(); break;
		case FSMState.Look: ExecuteLookState(); break;
		case FSMState.Chase: ExecuteChaseState(); break;
		case FSMState.Attack: ExecuteAttackState(); break;
		case FSMState.Dead: ExecuteDeadState(); break;
		case FSMState.Run: ExecuteRunState(); break;
		}
		
	}
	
	protected void ExecuteNoneState()
	{
		//Debug.Log ("NoneState");
		
		if (Vector3.Distance(transform.position, objPlayer.transform.position) < 10.0f)
		{
			curState = FSMState.Look;
		}
	}
	
	protected void ExecutePatrolState()
	{
		//Debug.Log ("PatrolState");
		
		if (Vector3.Distance(transform.position, objPlayer.transform.position) < 10.0f)
		{
			curState = FSMState.Look;
		}
		else
		{
			
			Quaternion targetRotation = Quaternion.LookRotation(pointsEnemyWalk[indexPoint].transform.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);
			transform.Translate(Vector3.forward * Time.deltaTime * speed);
			
			if (Vector3.Distance(transform.position, pointsEnemyWalk[indexPoint].transform.position) < 2.0f)
			{
				indexPoint = (indexPoint != pointsEnemyWalk.Length - 1) ? indexPoint + 1 : 0;
			}
		}
	}
	
	protected void ExecuteLookState()
	{
		//Debug.Log ("LookState");
		
		Quaternion targetRotation = Quaternion.LookRotation(objPlayer.transform.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10.0f);
		
		if (Vector3.Distance(transform.position, objPlayer.transform.position) < 7.0f)
		{
			curState = isTurret ? FSMState.Attack : (isAgressive ? FSMState.Chase : FSMState.Patrol);
		}
		else
		{
			curState = isTurret ? FSMState.Look : FSMState.Patrol;
		}
		
	}
	
	protected void ExecuteChaseState()
	{
		//Debug.Log ("ChaseState");
		
		if (Vector3.Distance(transform.position, objPlayer.transform.position) < 11.0f)
		{
			Quaternion targetRotation = Quaternion.LookRotation(objPlayer.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);
			transform.Translate(Vector3.forward * Time.deltaTime * speed);
		}
		else
		{
			curState = FSMState.Look;
		}
		
	}
	
	protected void ExecuteAttackState()
	{
		//Debug.Log ("AttackState");
		
		if (Vector3.Distance(transform.position, objPlayer.transform.position) < 7.0f)
		{
			Quaternion targetRotation = Quaternion.LookRotation(objPlayer.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15.0f);
			
			if (Time.time > nextFire)
			{
				//Debug.Log ("Shot");
				nextFire = Time.time + fireRate;
				Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
			}
		}
		else
		{
			curState = FSMState.None;
		}
	}
	
	protected void ExecuteDeadState()
	{
	}
	
	protected void ExecuteRunState()
	{
		//Debug.Log ("RunState");
		if (Vector3.Distance(transform.position, objPlayer.transform.position) < 15.0f)
		{
			Quaternion targetRotation = Quaternion.LookRotation(pointsEnemyWalk[indexPoint].transform.position - transform.position * -1);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);
			transform.Translate(Vector3.forward * Time.deltaTime * 9.0f);
		}
		else
		{
			curState = FSMState.Look;
		}
		
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
