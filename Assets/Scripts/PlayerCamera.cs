using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
	/* Position of the object the camera is tracking. Setting it also resets the camera. */
	public Transform player;
	/* Camera component accessor.  */
	Camera playerCamera;
	public Camera Camera
	{
		get
		{
			return playerCamera;
		}
		set
		{
			playerCamera = value;
			playerCamera.orthographic = true;
			playerCamera.orthographicSize = previousZoomValue = nextZoomValue = defaultZoom;
		}
	}

	/* Zoom control variables. */
	public float zoomMod = 500;
	public float minZoom = 100;
	public float maxZoom = 600;
	public float defaultZoom = 100;
	public float singleZoomTime = 0.5f; // time it takes to scroll over a single click of the scroll wheel
	float previousZoomValue;
	float nextZoomValue;
	float zoomLerp;
	float inverseTotalZoomTime;

	/* Smart camera movement. */
	public float edgeHotspot = 0.85f;

	/* Get camera to follow object. */
	void LateUpdate ()
	{
		/* Moving camera with player. */
		Vector3 pos = (Vector2)player.position;
		// Set new camera position (conserves current z)
		pos.z = gameObject.transform.position.z;
		gameObject.transform.position = pos;

		/* Zoom wheel zooming. */
		float zoomDelta = InputManager.Instance.ZoomDelta;
		// if palyer is using the scroll wheel update scroll-to value and lerp time
		if (zoomDelta != 0)
		{
			zoomLerp = 0;
			previousZoomValue = playerCamera.orthographicSize;
			nextZoomValue = previousZoomValue + Mathf.Clamp (zoomDelta*zoomMod, minZoom - playerCamera.orthographicSize, maxZoom - playerCamera.orthographicSize);
			// reset total time over which the lerp to the zoom value should take place
			inverseTotalZoomTime = 1 / Mathf.Abs (singleZoomTime * zoomDelta);
		}
		// tick for zoom lerp happens here to make the scroll more responsive (as it would begin on the same frame as player begins scrolling)
		zoomLerp += Time.deltaTime;
		// lerp the zoom into position
		playerCamera.orthographicSize = Mathf.Lerp (previousZoomValue, nextZoomValue, zoomLerp*inverseTotalZoomTime);
	}

	/**
	* Automatically destroys itself on disconnection.
	*/
	void OnLeftRoom ()
	{
		Destroy (gameObject);
	}
}
