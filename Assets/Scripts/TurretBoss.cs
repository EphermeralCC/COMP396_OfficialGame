using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class TurretBoss : MonoBehaviour
{
	
	public enum FSMState
	{
		None,
		Look,
		Attack
	}
	
	public FSMState curState;
	public string enemyTagPoints; //Tag of enemy waypoints
	public GameObject bullet; //Prefab which will be shoot
	public Transform bulletSpawn; //BulletSpawn Object in turret
	public float fireRateMin; //Turret Fire rate
	public float fireRateMax; //Turret Fire rate
	
	private GameObject[] pointsEnemyWalk;
	private GameObject[] objPlayers;
	private GameObject objPlayer;
	private int indexPoint;
	private float nextFire;
	private bool randomAttack;

	private int randomPlayerIdx;
	private float[] playerDist;

	// Use this for initialization
	void Start()
	{
		StartCoroutine( RandomBehavior ());
	}

	IEnumerator RandomBehavior(){
		
		while(true){
			randomAttack = ( UnityEngine.Random.Range (0,2) == 1 ? true: false);
			yield return new WaitForSeconds(UnityEngine.Random.Range (5,10));
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
		case FSMState.None: ExecuteNoneState(); break;
		case FSMState.Look: ExecuteLookState(); break;
		case FSMState.Attack: ExecuteAttackState(); break;
		}
		
	}
	
	protected void ExecuteNoneState()
	{
		//Debug.Log ("NoneState");
		
		if (Vector3.Distance(transform.position, objPlayer.transform.position) < 13.0f)
		{
			curState = FSMState.Look;
		}
	}	
		
	protected void ExecuteLookState()
	{
		//Debug.Log ("LookState");
		
		Quaternion targetRotation = Quaternion.LookRotation(objPlayer.transform.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10.0f);
		curState = (Vector3.Distance (transform.position, objPlayer.transform.position) < 10.0f) ? FSMState.Attack : FSMState.Look;

	}
		
	protected void ExecuteAttackState()
	{
		//Debug.Log ("AttackState");
		
		if (Vector3.Distance(transform.position, objPlayer.transform.position) < 8.0f) {
			Quaternion targetRotation = Quaternion.LookRotation(objPlayer.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15.0f);
			
			if (Time.time > nextFire)
			{
				
				if (randomAttack) {
					nextFire = Time.time + UnityEngine.Random.Range (fireRateMin,fireRateMax);
					Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
				}
			}
		} else {
			curState = FSMState.None;
		}
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		if (stream.isWriting) {
			syncPosition = transform.GetComponent<Rigidbody>().position;
			stream.Serialize(ref syncPosition);
		} else {
			stream.Serialize(ref syncPosition);
			transform.GetComponent<Rigidbody>().position = syncPosition;
		}
	}
	
}
