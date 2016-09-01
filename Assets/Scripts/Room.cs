using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Room : MonoBehaviour
{
	public class RoomWallConnection
	{
		public Room room;
		public Wall wall;

		public RoomWallConnection(Room room, Wall wall)
		{
			this.room = room;
			this.wall = wall;
		}
	}

	List<Wall> walls 						= new List<Wall> ();
	List<Wall> doorways 					= new List<Wall> ();
	List<Door> doors 						= new List<Door> ();
	List<RoomWallConnection> connections 	= new List<RoomWallConnection> ();
	List<GameObject> hallwayConnections 	= new List<GameObject> ();

	GameObject ceiling;

	bool first = false;

	Vector3 size;
	Vector3 position;

	public List<RoomWallConnection> Connections
	{
		get { return connections; }
	}

	public List<GameObject> HallwayConnections
	{
		get { return hallwayConnections; }
	}

	public bool First
	{
		get { return first; }
		set { first = value; }
	}

	public Vector3 Size
	{
		get { return size; }
		set { size = value; }
	}

	public List<Wall> Doorways
	{
		get { return doorways; }
		private set { doorways = value; }
	}

	public Vector3 Position
	{
		get { return position; }
		set 
		{ 
			transform.localPosition = value;
			position = value; 
		}
	}

	void Start ()
	{
	}

	Wall GetWall(Location location)
	{
		return walls.Single (wall => wall.Location == location);
	}

	public List<Wall> ValidDoorwayWalls()
	{
		return walls.Where (wall => !wall.IsDoorway && !wall.Invalid).ToList ();
	}

	public void Connect(GameObject block, int length, Room other, Location location, bool angled = false)
	{
		float thickness = Constants.THICKNESS;
		float doorWidth = Constants.DOOR_WIDTH;
		float doorHeight = Constants.DOOR_HEIGHT;

		GameObject connection = new GameObject (
			"Connection from " + name [name.Length - 1] + " to " + other.name [other.name.Length - 1]);
		connection.transform.parent = transform.parent.transform;

		switch (location) {
		case Location.Front:
			connection.transform.localPosition = new Vector3 (Position.x, Position.y + (doorHeight / 2.0f), Position.z + (Size.z + length) / 2.0f);
			break;
		case Location.Back:
			connection.transform.localPosition = new Vector3 (Position.x, Position.y + (doorHeight / 2.0f), Position.z - (Size.z + length) / 2.0f);
			break;
		case Location.Left:
			connection.transform.localPosition = new Vector3 (Position.x - (Size.x + length) / 2.0f, Position.y + (doorHeight / 2.0f), Position.z);
			break;
		case Location.Right:
			connection.transform.localPosition = new Vector3 (Position.x + (Size.x + length) / 2.0f, Position.y + (doorHeight / 2.0f), Position.z);
			break;
		}

		// Create a hallway for the two rooms
		for (int i = 0; i < 4; i++)
		{
			GameObject clone = GameObject.Instantiate (block);
			clone.transform.parent = connection.transform;

			if (location == Location.Front || location == Location.Back) {
				if (i < 2) {
					// Create two walls of the connecting hallway
					int negation = i == 0 ? 1 : -1;
					clone.transform.localScale = new Vector3 (thickness, doorHeight, length);
					clone.transform.localPosition = new Vector3 ( negation * ((doorWidth + thickness) / 2.0f), 0.0f, 0.0f);
				} else {
					// Create ceiling and floor of the hallway
					int negation = i == 3 ? 1 : -1;
					clone.transform.localScale = new Vector3 (doorWidth + (thickness * 2.0f), thickness, length);
					clone.transform.localPosition = new Vector3 ( 0.0f, negation * ((doorHeight + thickness) / 2.0f), 0.0f);
				}
			} else {
				if (i < 2) {
					// Create two walls of the connecting hallway
					int negation = i == 0 ? 1 : -1;
					clone.transform.localScale = new Vector3 (length, doorHeight, thickness);
					clone.transform.localPosition = new Vector3 (
						0.0f,
						0.0f,
						negation * ((doorWidth + thickness) / 2.0f));
				} else {
					// Create the ceiling and floor of the hallway
					int negation = i == 3 ? 1 : -1;
					clone.transform.localScale = new Vector3 (length, thickness, doorWidth + (thickness * 2.0f));
					clone.transform.localPosition = new Vector3 (
						0.0f,
						negation * ((doorHeight + thickness) / 2.0f),
						0.0f);
				}
			}
		}
		hallwayConnections.Add (connection);

		// Make a doorway for this room and then make a doorway on the other room and position it
		// and then create the room connection
		MakeDoorway (block, location);
		CreateConnection (other, location);

		switch (location) {
		case Location.Front:
			other.MakeDoorway (block, Location.Back);
			other.Position = new Vector3 (Position.x, Position.y, Position.z + ((Size.z + other.Size.z) / 2.0f) + length);

			// Create other room connection
			other.CreateConnection (this, Location.Back);

			break;
		case Location.Back:
			other.MakeDoorway (block, Location.Front);
			other.Position = new Vector3 (Position.x, Position.y, Position.z - (((Size.z + other.Size.z) / 2.0f) + length));

			// Create other room connection
			other.CreateConnection (this, Location.Front);

			break;
		case Location.Left:
			other.MakeDoorway (block, Location.Right);
			other.Position = new Vector3 (Position.x - (((Size.x + other.Size.x) / 2.0f) + length), Position.y, Position.z);

			// Create other room connection
			other.CreateConnection (this, Location.Right);

			break;
		case Location.Right:
			other.MakeDoorway (block, Location.Left);
			other.Position = new Vector3 (Position.x + ((Size.x + other.Size.x) / 2.0f) + length, Position.y, Position.z);

			// Create other room connection
			other.CreateConnection (this, Location.Left);

			break;
		}
	}

	public void MakeDoor(GameObject block, Location location)
	{
		Wall wall = GetWall (location);

		// Safety checks
		if (!wall.IsDoorway || wall.HasDoor)
			return;

		// Create the door and give it an useful name
		GameObject doorObject = GameObject.Instantiate (block);
		Door door = doorObject.AddComponent<Door> ();
		door.name = "Door at " + wall.Location + " wall";
		door.transform.parent = wall.transform;

		BoxCollider col = doorObject.GetComponent<BoxCollider> ();
		BoxCollider trigger = doorObject.AddComponent<BoxCollider> ();
		trigger.isTrigger = true;

		if (location == Location.Front || location == Location.Back) {
			col.size = new Vector3 (1.0f, 1.0f, 2.0f);
			door.transform.localScale = new Vector3 (Constants.DOOR_WIDTH, Constants.DOOR_HEIGHT, Constants.THICKNESS);
			door.transform.localPosition = new Vector3 (
				0.0f, 
				(Constants.DOOR_HEIGHT - Size.y) / 2.0f,
				location == Location.Front ? Constants.DOOR_OFFSET : -Constants.DOOR_OFFSET);
			trigger.size = new Vector3 (0.75f, 1.0f, 6.0f);
		} else {
			col.size = new Vector3 (2.0f, 1.0f, 1.0f);
			door.transform.localScale = new Vector3 (Constants.THICKNESS, Constants.DOOR_HEIGHT, Constants.DOOR_WIDTH);
			door.transform.localPosition = new Vector3 (
				location == Location.Left ? -Constants.DOOR_OFFSET : Constants.DOOR_OFFSET, 
				(Constants.DOOR_HEIGHT - Size.y) / 2.0f,
				0.0f);
			trigger.size = new Vector3 (6.0f, 1.0f, 0.75f);
		}

		wall.Door = door;
		wall.HasDoor = true;
		doors.Add (door);
		door.Room = this;
	}

	public void MakeDoorway(GameObject block, Location location)
	{
		Wall wallToMakeDoorway = GetWall (location);
		wallToMakeDoorway.MakeDoorway (block, this);
		doorways.Add (wallToMakeDoorway);
	}

	public void PlaceKey (Material color, System.Random rand)
	{
		GameObject clone = GameObject.Instantiate (Resources.Load<GameObject> ("Key"));
		clone.transform.parent = transform;
		clone.name = color.name + " key";
		clone.transform.localPosition = new Vector3 (
			((Size.x - 1.0f) * (float)rand.NextDouble ()) - (Size.x / 2.0f) + 0.5f,
			0.75f,
			((Size.z - 1.0f) * (float)rand.NextDouble ()) - (Size.z / 2.0f) + 0.5f);

	}

	public void FixLastDoor(GameObject block)
	{
		if (doorways.Count > 0) {
			doorways [doorways.Count - 1].MakeWall (block, this);
		}
	}

	public void AddCeilingLight()
	{
		GameObject lightObject = new GameObject ("Ceiling light");
		Light light = lightObject.AddComponent<Light> ();
		light.type = LightType.Spot;
		light.intensity = 4.0f;
		light.bounceIntensity = 0.0f;
		light.spotAngle = 120.0f;


		lightObject.transform.parent = ceiling.transform;
		lightObject.transform.localScale = Vector3.one;
		lightObject.transform.localPosition = new Vector3 (0.0f, -Constants.THICKNESS / 2.0f, 0.0f);
		lightObject.transform.localEulerAngles = new Vector3 (90.0f, 0.0f, 0.0f);
	}

	public void GenerateRoom(GameObject block, int length, int width, int height)
	{
		Size = new Vector3 (width, height, length);
		Position = Vector3.zero;

		float thickness = Constants.THICKNESS;

		// Create floor
		GameObject floor = GameObject.Instantiate (block);
		floor.transform.parent = this.gameObject.transform;
		floor.transform.localScale = new Vector3 (size.x, thickness, size.z);
		floor.transform.localPosition = new Vector3 (0.0f, -thickness / 2.0f, 0.0f);

		// Create ceiling
		GameObject ceiling = GameObject.Instantiate(block);
		ceiling.transform.parent = this.gameObject.transform;
		ceiling.transform.localScale = new Vector3 (size.x, thickness, size.z);
		ceiling.transform.localPosition = new Vector3 (0.0f, size.y + thickness / 2.0f, 0.0f);
		this.ceiling = ceiling;

		// Create walls
		for (int i = 0; i < 4; i++)
		{
			Location location = 
				i == 0 ? Location.Front :
				i == 1 ? Location.Back :
				i == 2 ? Location.Left :
				Location.Right;

			GameObject wallObj = new GameObject ("Awesome wall " + location.ToString());
			wallObj.transform.parent = this.transform;

			Wall wall = wallObj.AddComponent<Wall> ();
			wall.Location = location;
			wall.MakeWall (block, this);
			walls.Add (wall);
		}
	}

	void CreateConnection(Room other, Location location)
	{
		RoomWallConnection connection = new RoomWallConnection (other, GetWall (location));
		Connections.Add (connection);
	}
}