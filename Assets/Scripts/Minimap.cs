using UnityEngine;
using System.Collections;

public class Minimap : MonoBehaviour
{
	public Camera mapCamera;

	// Key bindings
	public KeyCode zoomOutKey 		= KeyCode.X;
	public KeyCode zoomInKey 		= KeyCode.Z;
	public KeyCode defaultZoomKey 	= KeyCode.Alpha0;

	// Zoom values
	public float minZoom;
	public float defaultZoom;
	public float maxZoom;
	public float zoomSpeed;

	void Start ()
	{
		if (mapCamera != null) {
			mapCamera.orthographicSize = defaultZoom;
		} else {
			Debug.Log ("No camera assigned to Minimap controller");
		}
	}

	void Update ()
	{
		float zoomAdjust = 0.0f;
		if (Input.GetKey (zoomOutKey)) {
			zoomAdjust += zoomSpeed;
		}
		if (Input.GetKey (zoomInKey)) {
			zoomAdjust -= zoomSpeed;
		}
		mapCamera.orthographicSize = Mathf.Clamp (mapCamera.orthographicSize + (zoomAdjust * Time.deltaTime), minZoom, maxZoom);

		if (Input.GetKeyDown (defaultZoomKey))
			mapCamera.orthographicSize = defaultZoom;
	}
}