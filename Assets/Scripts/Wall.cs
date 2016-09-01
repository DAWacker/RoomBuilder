using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wall : MonoBehaviour
{
	List<GameObject> blocks = new List<GameObject> ();

	bool isDoorway = false;
	public bool IsDoorway
	{
		get { return isDoorway; }
		private set { isDoorway = value; }
	}

	public Door Door
	{
		get;
		set;
	}

	bool hasDoor = false;
	public bool HasDoor
	{
		get { return hasDoor; }
		set { hasDoor = value; }
	}

	public Room Room
	{
		get;
		set;
	}

	public Location Location
	{
		get;
		set;
	}

	bool invalid = false;
	public bool Invalid
	{
		get { return invalid; }
		set { invalid = value; }
	}

	public void MakeDoorway(GameObject block, Room room)
	{
		Room = room;
		IsDoorway = true;
		Invalid = true;

		// More sophisticated clear
		foreach (GameObject b in blocks)
			Destroy (b);
		blocks.Clear ();

		float largerWidth = 0.0f;
		float thickness = Constants.THICKNESS;
		float doorWidth = Constants.DOOR_WIDTH;
		float doorHeight = Constants.DOOR_HEIGHT;

		// Create three pieces for the doorway (assumes doorway is always in the center of the wall)
		switch (Location)
		{
			case Location.Front:

				this.transform.localPosition = new Vector3 (0.0f, room.Size.y / 2.0f, room.Size.z / 2.0f);
				largerWidth = (room.Size.x - doorWidth) / 2.0f;

				for (int i = 0; i < 3; i++)
				{
					GameObject clone = GameObject.Instantiate (block);
					clone.transform.parent = this.transform;

					if (i < 2)
					{
						int negation = i == 0 ? 1 : -1;
						clone.transform.localScale = new Vector3 (largerWidth, room.Size.y, thickness);
						clone.transform.localPosition = new Vector3 (negation * ((doorWidth + largerWidth) / 2.0f), 0.0f, 0.0f);
					} 
					else 
					{
						clone.transform.localScale = new Vector3 (doorWidth, room.Size.y - doorHeight, thickness);
						clone.transform.localPosition = new Vector3 (0.0f, (room.Size.y - clone.transform.localScale.y) / 2.0f, 0.0f);
					}

					blocks.Add (clone);
				}
				break;
			case Location.Back:
				this.transform.localPosition = new Vector3 (0.0f, room.Size.y / 2.0f, -room.Size.z / 2.0f);
				largerWidth = (room.Size.x - doorWidth) / 2.0f;

				for (int i = 0; i < 3; i++)
				{
					GameObject clone = GameObject.Instantiate (block);
					clone.transform.parent = this.transform;

					if (i < 2)
					{
						int negation = i == 0 ? 1 : -1;
						clone.transform.localScale = new Vector3 (largerWidth, room.Size.y, thickness);
						clone.transform.localPosition = new Vector3 (negation * ((doorWidth + largerWidth) / 2.0f), 0.0f, 0.0f);
					} 
					else 
					{
						clone.transform.localScale = new Vector3 (doorWidth, room.Size.y - doorHeight, thickness);
						clone.transform.localPosition = new Vector3 (0.0f, (room.Size.y - clone.transform.localScale.y) / 2.0f, 0.0f);
					}

					blocks.Add (clone);
				}
				break;
			case Location.Left:
				this.transform.localPosition = new Vector3 (-room.Size.x / 2.0f, room.Size.y / 2.0f, 0.0f);
				largerWidth = (room.Size.z - doorWidth) / 2.0f;

				for (int i = 0; i < 3; i++)
				{
					GameObject clone = GameObject.Instantiate (block);
					clone.transform.parent = this.transform;

					if (i < 2)
					{
						int negation = i == 0 ? 1 : -1;
						clone.transform.localScale = new Vector3 (thickness, room.Size.y, largerWidth);
					clone.transform.localPosition = new Vector3 (0.0f, 0.0f, negation * ((doorWidth + largerWidth) / 2.0f));
					} 
					else 
					{
						clone.transform.localScale = new Vector3 (thickness, room.Size.y - doorHeight, doorWidth);
						clone.transform.localPosition = new Vector3 (0.0f, (room.Size.y - clone.transform.localScale.y) / 2.0f, 0.0f);
					}

					blocks.Add (clone);
				}
				break;
			case Location.Right:
				this.transform.localPosition = new Vector3 (room.Size.x / 2.0f, room.Size.y / 2.0f, 0.0f);
				largerWidth = (room.Size.z - doorWidth) / 2.0f;

				for (int i = 0; i < 3; i++)
				{
					GameObject clone = GameObject.Instantiate (block);
					clone.transform.parent = this.transform;

					if (i < 2)
					{
						int negation = i == 0 ? 1 : -1;
						clone.transform.localScale = new Vector3 (thickness, room.Size.y, largerWidth);
						clone.transform.localPosition = new Vector3 (0.0f, 0.0f, negation * ((doorWidth + largerWidth) / 2.0f));
					} 
					else 
					{
						clone.transform.localScale = new Vector3 (thickness, room.Size.y - doorHeight, doorWidth);
						clone.transform.localPosition = new Vector3 (0.0f, (room.Size.y - clone.transform.localScale.y) / 2.0f, 0.0f);
					}

					blocks.Add (clone);
				}
				break;
			default:
				break;
		}
	}

	public void MakeWall(GameObject block, Room room)
	{
		Room = room;
		IsDoorway = false;

		float thickness = Constants.THICKNESS;

		// More sophisticated clear
		foreach (GameObject b in blocks)
			Destroy (b);
		blocks.Clear ();

		GameObject clone = GameObject.Instantiate (block);

		switch (Location)
		{
			case Location.Front:
				this.transform.localPosition = new Vector3 (0.0f, room.Size.y / 2.0f, room.Size.z / 2.0f);
				clone.transform.parent = this.transform;
				clone.transform.localScale = new Vector3 (room.Size.x, room.Size.y, thickness);
				clone.transform.localPosition = Vector3.zero;
				break;
			case Location.Back:
				this.transform.localPosition = new Vector3 (0.0f, room.Size.y / 2.0f, -room.Size.z / 2.0f);
				clone.transform.parent = this.transform;
				clone.transform.localScale = new Vector3 (room.Size.x, room.Size.y, thickness);
				clone.transform.localPosition = Vector3.zero;
				break;
			case Location.Left:
				this.transform.localPosition = new Vector3 (-room.Size.x / 2.0f, room.Size.y / 2.0f, 0.0f);
				clone.transform.parent = this.transform;
				clone.transform.localScale = new Vector3 (thickness, room.Size.y, room.Size.z);
				clone.transform.localPosition = Vector3.zero;
				break;
			case Location.Right:
				this.transform.localPosition = new Vector3 (room.Size.x / 2.0f, room.Size.y / 2.0f, 0.0f);
				clone.transform.parent = this.transform;
				clone.transform.localScale = new Vector3 (thickness, room.Size.y, room.Size.z);
				clone.transform.localPosition = Vector3.zero;
				break;
			default:
				break;
		}

		blocks.Add (clone);
	}
}