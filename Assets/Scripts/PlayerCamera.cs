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
			CameraTransform.position = value.position;
		}
	}
	/* Position of the camera. */
	Transform CameraTransform {get; set;}
	/* Camera component accessor.  */
	Camera Camera {get; set;}
	/* Zoom control variables. */
	public float zoomMod = 3;
	public float minZoom = -1;
	public float maxZoom = -10;

	Vector2 maxOffsetVector;
	Vector2 previousOffsetVector;

	Vector2 origin;

	bool targetIsMoving = false;
	float lerpTime = 0;

	public float stopTime = 0.1f;
	public float totalLerpTime = 1.5f;
	public float maxCameraOffset = 2;

	/* Early bird gets the early worm. */
	void Awake ()
	{
		Camera = GetComponent<Camera> ();
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
			origin = Target.position;
			Vector2 target = Target.position;

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
			float lerp = lerpTime/totalLerpTime;
			lerp = lerp*lerp * (3f - 2f*lerp);
			Vector3 pos = Vector2.Lerp (origin, target, lerp);

			// Set new camera position
			pos.z = Mathf.Clamp (CameraTransform.position.z + zoomDelta*zoomMod, maxZoom, minZoom);
			CameraTransform.position = pos;
		}
	}
}
