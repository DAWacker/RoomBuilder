using UnityEngine;
using System.Collections;

public class Walker : MonoBehaviour
{
	// Movement values
	public float horzRotSpeed;
	public float vertRotSpeed;
	public float moveSpeed;

	void Start ()
	{
	}

	void Update ()
	{
		// Rotational movement
		float horzRot = 0.0f; //Input.GetAxis ("Mouse X") * horzRotSpeed * Time.deltaTime;
		float vertRot = 0.0f; //Input.GetAxis ("Mouse Y") * vertRotSpeed * Time.deltaTime;

		if (Input.GetKey (KeyCode.Q))
			horzRot = -horzRotSpeed * Time.deltaTime;
		if (Input.GetKey (KeyCode.E))
			horzRot = horzRotSpeed * Time.deltaTime;
		
		transform.Rotate (-vertRot, horzRot, 0.0f);

		// Positional movement
		float moveX = 0.0f; //Mathf.Clamp(Input.GetAxis ("Horizontal"), -0.25f, 0.25f) * moveSpeed * Time.deltaTime;
		float moveZ = 0.0f; //Mathf.Clamp(Input.GetAxis ("Vertical"), -0.25f, 0.25f) * moveSpeed * Time.deltaTime;

		if (Input.GetKey (KeyCode.W))
			moveZ = moveSpeed * Time.deltaTime;
		if (Input.GetKey (KeyCode.S))
			moveZ = -moveSpeed * Time.deltaTime;
		if (Input.GetKey (KeyCode.A))
			moveX = -moveSpeed * Time.deltaTime;
		if (Input.GetKey (KeyCode.D))
			moveX = moveSpeed * Time.deltaTime;
		
		transform.Translate (new Vector3 (moveX, 0.0f, moveZ));
	}
}