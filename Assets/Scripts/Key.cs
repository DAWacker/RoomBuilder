using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour
{
	public Transform key;

	void Update ()
	{
		key.transform.RotateAround (transform.position, Vector3.up, 200.0f * Time.deltaTime);
	}
}