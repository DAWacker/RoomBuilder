using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Generator : MonoBehaviour
{
	public GameObject block;

	public Material defaultMat;
	public Material wallMat;
	public Material doorMat;

	public int roomCount;
	public bool disableLighting;
	public bool disableTextures;

	public HUD hud;

	Vector3 lastPos;
	Vector3 lastSize;

	List<Room> allRooms;

	Room prevRoom;

	GameObject dungeon;

	void Start ()
	{
		dungeon = new GameObject ("Dungeon");
		allRooms = new List<Room> ();

		// Set textures based on settings
		block.GetComponent<MeshRenderer> ().material = disableTextures ? defaultMat : wallMat;

		GenerateLevel (0);
		PlacePlayer ();

//		block.GetComponent<MeshRenderer> ().material = doorMat;
//		Room room = allRooms.Single(r => r.First);
//		foreach (Wall wall in room.Doorways)
//			room.MakeDoor(block, wall.Location);

		hud.CreateKeyImages (5);

		block.gameObject.SetActive (false);
	}

	void GenerateLevel(int doors)
	{
		List<Room> checkedRooms = new List<Room> ();

		System.Random rand = new System.Random ();
		float buffer = 3f;

		for (int i = 0; i < roomCount; i++)
		{
			GameObject newRoom = new GameObject ("Room awesomesauce " + i);
			Room room = newRoom.AddComponent<Room> ();
			newRoom.transform.parent = dungeon.transform;

			int length = rand.Next (5, 10);
			int width = rand.Next (5, 10);
			int height = rand.Next (4, 7);

			room.GenerateRoom (block, length, width, height);
			if (i > 0) {
				bool success = false;

				while (!success) {
					List<Wall> validWalls = prevRoom.ValidDoorwayWalls ();
					if (validWalls.Count > 0) {
						Wall randomWall = validWalls [rand.Next (0, validWalls.Count)];
						prevRoom.Connect (block, 5, room, randomWall.Location);
						success = true;

						foreach (Room r in allRooms) {
							Vector2 diff = DistanceBetween (r, room);
							if ((diff.x < buffer + (room.Size.x + prevRoom.Size.x) / 2.0f) &&
								(diff.y < buffer + (room.Size.z + prevRoom.Size.z) / 2.0f)) {
								success = false;

								// Fix the wall that created a connection to this invalid room placement
								prevRoom.FixLastDoor (block);
								room.FixLastDoor (block);

								// Remove the invalid hallway connection
								List<GameObject> connections = prevRoom.HallwayConnections;
								GameObject invalidConnection = connections [connections.Count - 1];

								connections.Remove (invalidConnection);
								Destroy (invalidConnection);
								break;
							}
						}
						randomWall.Invalid = true;
					} else {
						checkedRooms.Add (prevRoom);
						List<Room> roomsToCheck = allRooms.Except (checkedRooms).ToList ();

						if (roomsToCheck.Count <= 0) {
							Debug.Log ("Something really bad happened");
							roomCount = 0;
							break;
						}
						prevRoom = roomsToCheck [rand.Next (0, roomsToCheck.Count)];
					}
				}
			} else {
				room.First = true;
			}

			allRooms.Add (room);
			if (!disableLighting)
				room.AddCeilingLight ();

//			if (allRooms.Count == 2)
//				room.PlaceKey (defaultMat, rand);

			List<Room> validRooms = allRooms.Except (checkedRooms).ToList ();
			prevRoom = validRooms [rand.Next (0, validRooms.Count)];
		}
	}

	void PlacePlayer()
	{
		// Find and place the walker in the first room
		Vector3 firstRoomPos = allRooms.First (room => room.First).Position;
		Walker walker = FindObjectOfType<Walker> ();
		walker.transform.parent = dungeon.transform;
		walker.transform.localPosition = new Vector3 (firstRoomPos.x, 2.0f, firstRoomPos.z);
	}

	Vector2 DistanceBetween(Room r1, Room r2)
	{
		return new Vector2 (
			Mathf.Abs (r1.Position.x - r2.Position.x),
			Mathf.Abs (r1.Position.z - r2.Position.z));
	}
}