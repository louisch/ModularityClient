using UnityEngine;

/**
* This class should be used to keep track of *changes* to a player's state.
* Extensions must follows the same model.
* Should record changes like massDelta and dragDelta when tracking them becomes valid.
*/
public class InputState {
	/* Note Timestamp must always be set internally and cannot be changed externally. */
	public double Timestamp {get; private set;}
	/* Note that these accessors are fully accessible. */
	public Vector2 MovementDelta {get; set;}
	public float RotationDelta {get; set;}
	
	/**
	* Default empty constructor.
	*/
	public InputState (double timestamp)
	{
		Timestamp = timestamp;
		MovementDelta = Vector2.zero;
		RotationDelta = 0;
	}

	/**
	* Default full constructor.
	*/
	public InputState (double timestamp, Vector2 movementDelta, float rotationAtTimeStamp)
	{
		Timestamp = timestamp;
		MovementDelta = movementDelta;
		RotationDelta = rotationAtTimeStamp;
	}
}
