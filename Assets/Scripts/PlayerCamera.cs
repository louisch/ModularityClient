using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
	/* Position of the object the camera is tracking. Setting it also resets the camera. */
	private Transform player;
	public Transform Player
	{
		get
		{
			return player;
		}
		set
		{
			player = value;
			CameraTransform.position = (Vector2)value.position;
		}
	}
	/* Position of the camera. */
	Transform CameraTransform {get; set;}
	/* Camera component accessor.  */
	Camera Camera {get; set;}

	/* Zoom control variables. */
	public float zoomMod = 2;
	public float minZoom = .3f;
	public float maxZoom = 1.5f;
	public float defaultZoom = 1;
	public float singleZoomTime = 0.5f; // time it takes to scroll over a single click of the scroll wheel
	float previousZoomValue;
	float nextZoomValue;
	float zoomLerp = 0;
	float inverseTotalZoomTime;

	/* Smart camera movement. */
	public float edgeHotspot = 0.85f;

	/* Early bird catches the early worm. */
	void Awake ()
	{
		Camera = GetComponent<Camera> ();
		Camera.orthographic = true;
		Camera.orthographicSize = previousZoomValue = nextZoomValue = defaultZoom;
		CameraTransform = GetComponent<Transform> ();
	}

	/* Get camera to follow object. */
	void LateUpdate ()
	{
		if (Player)
		{
			/* Moving camera with player. */
			Vector3 pos = (Vector2)Player.position;
			// Set new camera position (conserves current z)
			pos.z = CameraTransform.position.z;
			CameraTransform.position = pos;


			/* Zoom out as mouse pointer leaves the screen. (unimplemented) */
			// offset between mouse pointer and player world coordinates
			// Vector2 mouseOffset = Camera.ScreenToWorldPoint (InputManager.Instance.MousePosition);

			// float offsetMagnitude = mouseOffset.magnitude;

			// Vector2 thing = Camera.ScreenToWorldPoint (new Vector2 (Screen.width, Screen.height));
			// float maxScreenVector = Mathf.Min (thing.x, thing.y);


			// Debug.Log (offsetMagnitude + " " + maxScreenVector + " " + (maxZoom - Camera.orthographicSize));
			// float mod = Mathf.Clamp (offsetMagnitude/maxScreenVector, 0, 1);
			// float zoomExtendDistance = maxScreenVector - edgeHotspot*maxScreenVector;

			// mod = Mathf.Clamp (zoomExtendDistance - (maxScreenVector - offsetMagnitude), 0, zoomExtendDistance)/Mathf.Abs (zoomExtendDistance);
			// Camera.orthographicSize = Mathf.Clamp (Camera.orthographicSize + (maxZoom - Camera.orthographicSize) * mod, minZoom, maxZoom);

			/* Zoom wheel zooming. */
			float zoomDelta = InputManager.Instance.ZoomDelta;
			// if palyer is using the scroll wheel update scroll-to value and lerp time
			if (zoomDelta != 0)
			{
				zoomLerp = 0;
				previousZoomValue = Camera.orthographicSize;
				nextZoomValue = previousZoomValue + Mathf.Clamp (zoomDelta*zoomMod, minZoom - Camera.orthographicSize, maxZoom - Camera.orthographicSize);
				// reset total time over which the lerp to the zoom value should take place
				inverseTotalZoomTime = 1 / Mathf.Abs (singleZoomTime * zoomDelta);
			}
			// tick for zoom lerp happens here to make the scroll more responsive (as it would begin on the same frame as player begins scrolling)
			zoomLerp += Time.deltaTime;

			// lerp the zoom into position
			Camera.orthographicSize = Mathf.Lerp (previousZoomValue, nextZoomValue, zoomLerp*inverseTotalZoomTime);
		}
	}
}
