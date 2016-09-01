using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	List<Key> keys = new List<Key> ();

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Key> ()) {
			Key key = other.GetComponent<Key> ();
			keys.Add (key);
			key.gameObject.SetActive (false);
		}
	}
}