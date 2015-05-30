using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocalPlayerUpdater : ObjectUpdater {
	// info saved before sending network updates
	float pHInput;
	float pVInput;

	// movement related refs
	Transform trans;
	Rigidbody rb;

	// movement vars
	public float hSpeed = 10;
	public float vSpeed = 10;

	// state reconciliation vars
	LinkedList<InputState> previousInputs = new LinkedList<InputState> ();
	public double lerpTime = 4;

	public override void Awake ()
	{
		rb = GetComponent<Rigidbody> ();
		trans = GetComponent<Transform> ();
		base.Awake ();
	}

		// movement update for this object
	void FixedUpdate ()
	{
		if (!base.Owner.isLocal)
		{
			Debug.LogError ("Player script attached to non-player object!");
			return;
		}
		// send updates to server
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		// only send an update if input has changed
		if (h != pHInput || v != pVInput)
		{
			Debug.Log ("Sending position update to server");
			base.View.RPC ("UpdateInput", PhotonTargets.MasterClient, h, v);
			pHInput = h;
			pVInput = v;
		}

		// normalise input vecor
		Vector3 moveBy = new Vector3 (h,0,v).normalized;
		// compute movement delta vector (frame independent)
		moveBy = new Vector3(moveBy.x * hSpeed * Time.fixedDeltaTime, 0, moveBy.z * vSpeed * Time.fixedDeltaTime);
		// save vector with current timestamp
		previousInputs.AddLast (new InputState (Network.time, moveBy));
		// apply movement vector to position
		rb.MovePosition(trans.position + moveBy);
	}

	// applies inputs in order to bring reported serverPos up to current time
	public override void UpdatePos (ref Vector3 serverPos, double updateTS)
	{
		if (!base.Owner.isLocal)
		{
			Debug.LogError ("Player script attached to non-player object!");
			return;
		}
		Debug.Log ("looking for input timestamp");
		// discard inputs that are too old
		while (previousInputs.Count > 0 && previousInputs.First.Value.timestamp < updateTS)
		{
			previousInputs.RemoveFirst ();
		}
		Debug.Log ("Applying past inputs to server update");
		// apply inputs to position received
		foreach (InputState input in previousInputs)
		{
			serverPos += input.moveBy;
		}
	}

	// override get accessor to return locally defined lerp time
	public override double LerpTime
	{
		get
		{
			return lerpTime;
		}
	}
}
