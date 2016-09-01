using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
	Room room;

	bool animateDoorOpen;
	bool open;

	float openAnimationDuration;
	float elapsedTime;

	Vector3 startPos;

	public Room Room
	{
		get { return room; }
		set { room = value; }
	}

	void Start () 
	{
		open = false;
		animateDoorOpen = false;

		openAnimationDuration = 1.5f;
		elapsedTime = 0.0f;
		startPos = transform.localPosition;
	}

	public void Open()
	{
		// Don't open if we are already opening
		if (animateDoorOpen)
			return;
		
		open = true;
		animateDoorOpen = true;

		elapsedTime = 0.0f;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Walker> ()) {
			Open ();
		}
	}

	void Update()
	{
		if (animateDoorOpen) {
			elapsedTime += Time.deltaTime;

			// Animation is complete
			if (elapsedTime >= openAnimationDuration) {
				animateDoorOpen = false;
				transform.localPosition = new Vector3 (
					startPos.x,
					startPos.y + Constants.DOOR_HEIGHT,
					startPos.z);
				gameObject.SetActive (false);

			// Still animating
			} else {
				float percentComplete = elapsedTime / openAnimationDuration;
				transform.localPosition = new Vector3 (
					startPos.x,
					startPos.y + (Constants.DOOR_HEIGHT * percentComplete),
					startPos.z);
			}
		}
	}
}