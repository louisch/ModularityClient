using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Module : MonoBehaviour
{

	// public:
	// list of occupied blocks
	// list of attachment points (think GUI)

	// private
	// list of joints (in same order as joint positions)
	//


	//public bool[] validJoints = new bool[4]; // Values should be set in Unity editor
	//public IntVector2[] stuff;
	//Module[] joints = new Module[4];

	public IntVector2[] occupiedBlocks;
	public BlockInfo[] occupiedBlockJoints;

	void Start ()
	{

	}

	public void ChangeParent (ModuleGroup parent, IntVector2 pos, int rot)
	{
		SetPosition (pos);
		SetRotation (rot);
		transform.SetParent (parent.gameObject.transform);
	}

	public void SetPosition (IntVector2 pos)
	{ // MAGIC NUMBER HAXX (implicit x1 normalisation because blocks are currently exactly 1x1 units in size)
		transform.localPosition.Set (
			Convert.ToSingle (pos.x),
			Convert.ToSingle (pos.y),
			transform.localPosition.z
		);
	}

	public IntVector2 GetPosition ()
	{
		return new IntVector2 (
			Convert.ToInt32 (transform.position.x),
			Convert.ToInt32 (transform.position.y)
		);
	}

	public void SetRotation (int rot)
	{
		transform.localEulerAngles.Set (0, 0, Convert.ToSingle (rot));
	}

	public int GetRotation ()
	{
		return (Convert.ToInt32 (transform.localEulerAngles.z) / 90) % 4;
	}

	public Module[] GetJoints ()
	{
		return null;
	}

	//public IntVector2 GetGlobalNeighbour ()
	//{
	//	IntVector2 result = GetPosition ();
	//	switch (GetRotation ())
	//	{
	//		case 0:
	//			result.y
	//	}
	//}


	//void Update ()
	//{
	//	transform.localEulerAngles = new Vector3 (0, 0, transform.localEulerAngles.z + 1);
	//	Debug.Log (transform.localEulerAngles.ToString ());
	//}
}
