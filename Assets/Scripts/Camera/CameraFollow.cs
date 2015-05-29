﻿using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target; // target for camera to follow
	public float smoothing = 0.01f;
	
	Vector3 offset; // persistant relative distance betwen camera and player
	
	void Start()
	{
		offset = transform.position - target.position;
	}
	
	void FixedUpdate() // every physics update
	{
		Vector3 targetCamPos = target.position + offset;
//		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.fixedDeltaTime);
		
	}
}