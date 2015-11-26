using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    private GameManager manager;

    public float moveSpeed;
    public float rotationSpeed = 60f;
    public GameObject deathParticles;  

    private float maxSpeed = 5f;
    private Vector3 input;
    private Vector3 spawn;

    private int myKeyCollect = 0;
    private int deathCount;

	// Use this for initialization
	void Start () {

        GameManager.totalKeyCount += 4;

        spawn = this.transform.position;
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        if (GetComponent<NetworkView>().isMine)
        {
            GetComponentInChildren<Camera>().enabled = true;
        }
        else
        {
            GetComponentInChildren<AudioListener>().enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (GetComponent<NetworkView>().isMine)
        {
            input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            if (this.transform.GetComponent<Rigidbody>().velocity.magnitude < maxSpeed)
            {
                this.transform.GetComponent<Rigidbody>().AddRelativeForce(input * moveSpeed);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                this.transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0.0f);
            }

            else if (Input.GetKey(KeyCode.E))
            {
                this.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0.0f);
            }

            if (transform.position.y < -5)
            {
                Death();
            }
        }
	
	}

    void OnCollisionEnter(Collision other)
    {
        if(other.transform.tag == "Enemy")
        {
            Death();
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameManager.numPlayerInExit -= 1;
        Debug.Log("Player leave, remain player: " + GameManager.numPlayerInExit);
    }

    void OnTriggerEnter(Collider other)
    {
        //manager.CompleteLevel();
		if (other.transform.tag == "Goal")
        {
            GameManager.numPlayerInExit += 1;
            Debug.Log("Player enter, remain player: " + GameManager.numPlayerInExit);
            Debug.Log("Toal player: " + GameManager.players.Length);
            if (GameManager.numPlayerInExit == GameManager.players.Length && GameManager.keyCount == GameManager.totalKeyCount)
            {
                manager.CompleteLevel();
            }            
        }

		if (other.transform.tag == "Key")
        {
            if (myKeyCollect < 4)
            {
                GameManager.keyCount += 1; ;
                Destroy(other.gameObject);
                myKeyCollect += 1;
            }  
        }

		if (other.transform.tag == "Bullet")
        {
            Death();
        }

        if (other.transform.tag == "Enemy")
        {
            Death();
        }

        if (other.transform.tag == "trap")
        {
            Death();
        }
    }

    public void Death()
    {
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        transform.position = spawn;
        deathCount += 1;
    }

    void OnGUI()
    {
        if (GetComponent<NetworkView>().isMine)
        {
            GUI.Label(new Rect(20, 40, 200, 200), "Deaths: " + deathCount.ToString());
            GUI.Label(new Rect(20, 60, 200, 200), "My Keys: " + myKeyCollect.ToString());
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
