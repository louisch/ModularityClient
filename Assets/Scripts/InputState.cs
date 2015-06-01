using UnityEngine;

// simple class for saving player input information at timestamp
// this currently only holds the movement delta vector
// note that this means that client rotation is NOT synchrnised
public class InputState {
	public double timestamp;
	public Vector2 movedBy;
	
	public InputState ()
	{
		timestamp = 0;
		movedBy = Vector3.zero;
	}

	public InputState (double timestamp, Vector3 movedBy)
	{
		this.timestamp = timestamp;
		this.movedBy = movedBy;
	}
}
