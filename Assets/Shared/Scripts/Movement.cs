using UnityEngine;

[System.Serializable]
public class PlayerMovement {
	public float strafeModifier = 10;
	public float thrustModifier = 10;
	public float torqueModifier = .25f;

	/* Move given rigidbody by applying input modified by some value. */
	public void Move (ref Rigidbody2D player, Vector2 inputVector, float inputTorque)
	{
		Vector2 moveForce = new Vector2(inputVector.x * strafeModifier, inputVector.y * thrustModifier);
		float torqueValue = inputTorque * torqueModifier;

		player.AddForce (moveForce);
		player.AddTorque (torqueValue);
	}
}
