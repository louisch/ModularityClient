using UnityEngine;

// simple class for saving player input information at timestamp
// this currently only holds the movement delta vector
// note that this means that client rotation is NOT synchrnised
public class InputState {
	public double Timestamp {get;private set;}
	public Vector2 MovementDelta {get;private set;}
	public float RotationDelta {get;private set;}
	
	public InputState ()
	{
		Timestamp = 0;
		MovementDelta = Vector2.zero;
		RotationDelta = 0;
	}

	public InputState (double timestamp, Vector2 movementDelta, float rotationDelta)
	{
		Timestamp = timestamp;
		MovementDelta = movementDelta;
		RotationDelta = rotationDelta;
	}
}
