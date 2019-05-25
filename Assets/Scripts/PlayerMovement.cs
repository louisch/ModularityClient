using UnityEngine;

[System.Serializable]
public class PlayerMovement {
	public float strafeModifier = 20;
	public float thrustModifier = 20;
	public float torqueModifier = 7.5f;

	/* Move given rigidbody by applying input modified by some value. */
	public void Move (ref Rigidbody2D player, float inputStrafe, float inputThrust, float inputTorque)
	{
		Transform t = player.gameObject.transform;

		player.AddForce (t.right * inputStrafe * strafeModifier);
		player.AddForce (t.up * inputThrust * thrustModifier);
		player.AddTorque (inputTorque * torqueModifier);
	}
}
