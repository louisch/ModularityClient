using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
	/* Position of the object the camera is tracking. Setting it also resets the camera. */
	private Transform target;
	public Transform Target
	{
		get
		{
			return target;
		}
		set
		{
			target = value;
			origin = value.position;
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
	public float defaultSize = 1;

	Vector2 maxOffsetVector;
	Vector2 previousOffsetVector;
	Vector2 origin;

	bool targetIsMoving = false;
	float lerpTime = 0;

	float previousZoomValue;
	float nextZoomValue;
	float zoomLerp = 0;
	public float totalZoomLerp = 0.35f;

	public float stopTime = 0.1f;
	public float totalLerpTime = 1.5f;
	public float maxCameraOffset = 2;
	public float weightToMouse = 0.5f;

	/* Early bird gets the early worm. */
	void Awake ()
	{
		Camera = GetComponent<Camera> ();
		Camera.orthographic = true;
		Camera.orthographicSize = previousZoomValue = nextZoomValue = defaultSize;
		CameraTransform = GetComponent<Transform> ();


		maxOffsetVector = Vector2.zero;
		previousOffsetVector = Vector2.zero;
	}

	/* Get camera to follow object. */
	void LateUpdate ()
	{
		if (Target)
		{
			float zoomDelta = InputManager.Instance.ZoomDelta;
			Vector2 mouseOffset = Camera.ScreenToWorldPoint (InputManager.Instance.MousePosition);



			origin = (Vector2)Target.position + weightToMouse * mouseOffset;
			Vector2 target = (Vector2)Target.position + weightToMouse * mouseOffset;

			// if (InputManager.Instance.IsMoving ())
			// {
			// 	//update input only if input is still happening
			// 	maxOffsetVector = InputManager.Instance.GetNormalizedInputVector() * maxCameraOffset;
			// 	if (false && previousOffsetVector != maxOffsetVector)
			// 	{
			// 		previousOffsetVector = maxOffsetVector;
			// 		lerpTime = 0;
			// 	}
			// 	if (lerpTime < totalLerpTime)
			// 	{
			// 		lerpTime += Time.deltaTime;
			// 	}
			// }
			// else
			// {
			// 	//origin = Target.position;
			// 	if (lerpTime > 0)
			// 	{
			// 		lerpTime -= Time.deltaTime;
			// 	}

			// }

			// setup lerp targets/origins
			//Vector2 target = (Vector2)Target.position - maxOffsetVector;

			// silky-smooth transition here
			float lerp; // we need this
			lerp = LerpFunctions.SmoothStep (lerpTime/totalLerpTime);
			Vector3 pos = Vector2.Lerp (origin, target, lerp);


			zoomLerp += Time.deltaTime;
			if (zoomDelta != 0)
			{
				previousZoomValue = Camera.orthographicSize;
				nextZoomValue = Mathf.Clamp (Camera.orthographicSize - zoomDelta*zoomMod, minZoom, maxZoom); 
				zoomLerp = 0;
			}
			if (zoomLerp > totalZoomLerp)
			{
				previousZoomValue = nextZoomValue = Camera.orthographicSize;
			}


			// Set new camera position (conserves current z)
			pos.z = CameraTransform.position.z;
			CameraTransform.position = pos;

			// zoom into position
			lerp = LerpFunctions.SmoothStep (zoomLerp/totalZoomLerp);
			Camera.orthographicSize = Mathf.Lerp (previousZoomValue, nextZoomValue, lerp);
		}
	}
}
