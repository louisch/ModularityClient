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
	public float zoomMod = 10;
	public float minZoom = 1;
	public float maxZoom = 8;
	public float defaultZoom = 4;
	public float singleZoomTime = 0.5f; // time it takes to scroll over a single click of the scroll wheel
	float previousZoomValue;
	float nextZoomValue;
	float zoomLerp;
	float inverseTotalZoomTime;

	/* Have camera offset towards mouse */
	public float offsetToMouse = 0.25f;
	public float shiftLimit = 0.15f;
	Vector2 offset = Vector2.zero;

	/* Get camera to follow object. */
	void LateUpdate ()
	{
		Rect screen = new Rect (0,0, Screen.width,Screen.height);
		/* Moving camera with player. */
		Vector3 pos = (Vector2)player.position;
		Vector2 mouse = Input.mousePosition;

		Vector2 newOffset = offset;
		if (screen.Contains (mouse))
		{
			newOffset = Camera.ScreenToWorldPoint(mouse) - pos;
		}

		if ((newOffset - offset).magnitude > shiftLimit)
		{
			offset = newOffset;
		}
		pos += (Vector3)offset*offsetToMouse;

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
