using UnityEngine;

[System.Serializable]
public class BlockInfo : System.Object
{
	public bool up;
	public bool right;
	public bool down;
	public bool left;

	public BlockInfo (bool up, bool right, bool down, bool left)
	{
		this.up = up;
		this.right = right;
		this.down = down;
		this.left = left;
	}
}
