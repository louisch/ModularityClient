using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ModuleInfo : MonoBehaviour
{
	public bool[] validJoints = new bool[4];

	ModuleInfo[] joints = new ModuleInfo[4];

	void Start ()
	{
		SetInvalidJoints ();
	}

	void SetInvalidJoints ()
	{
		foreach (bool validJoint in validJoints)
		{
			if (!validJoint)
			{

			}
		}
	}

	public void ChangeParent (GroupInfo parent, IntVector2 pos, int rot)
	{
		SetPosition (pos);
		SetRotation (rot);
		transform.SetParent (parent.gameObject.transform);
	}

	public void SetPosition (IntVector2 pos)
	{ // MAGIC NUMBER HAXX (because modules are currently exactly 1x1 units in size)
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

	//void Update ()
	//{
	//	transform.localEulerAngles = new Vector3 (0, 0, transform.localEulerAngles.z + 1);
	//	Debug.Log (transform.localEulerAngles.ToString ());
	//}
}
