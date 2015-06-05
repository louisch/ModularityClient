using UnityEngine;

[System.Serializable]
public class BlockInfo : MonoBehaviour
{
	public int x = 0;
	public int y = 0;

	public BlockInfo (int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}
