using UnityEngine;

// simple class for saving player input information at timestamp
// this currently only holds the movement delta vector
// note that this means that client rotation is NOT synchrnised
public class InputState {
	public double timestamp;
	public Vector3 moveBy;
	
	public InputState ()
	{
		timestamp = 0;
		moveBy = Vector3.zero;
	}

	public InputState (double timestamp, Vector3 moveBy)
	{
		this.timestamp = timestamp;
		this.moveBy = moveBy;
	}
}
