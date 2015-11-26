using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		GetComponent<Rigidbody> ().velocity = transform.forward * 10.0f;
	}

	void OnTriggerEnter(Collider other)
	{
		Destroy(gameObject);
		/// fuck this shit
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
