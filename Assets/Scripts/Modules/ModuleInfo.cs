using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ModuleInfo : MonoBehaviour, IModuleInfo
{

	// joints
	// other stats e.g. rotation

	public int rotation = 0;
	public IModuleInfo[] joints = new ModuleInfo[4];

	Vector2 position;

	void Start ()
	{

	}

	public void ChangeParent (GroupInfo parent, Vector2 pos, int rot)
	{
		SetPosition (pos, rot);
		transform.SetParent (parent.gameObject.transform);
	}

	public void SetPosition (Vector2 pos, int rot)
	{
		this.position = pos;
		this.rotation = rot;
		transform.localPosition.Set (
			Convert.ToSingle (pos.x),
			Convert.ToSingle (pos.y),
			transform.localPosition.z
		); // MAGIC NUMBER HAXX (because modules are currently exactly 1x1 units in size)
		transform.localEulerAngles.Set (0, 0, Convert.ToSingle (rot));
	}

	//void Update ()
	//{
	//	transform.localEulerAngles = new Vector3 (0, 0, transform.localEulerAngles.z + 1);
	//	Debug.Log (transform.localEulerAngles.ToString ());
	//}


	public string PartyTime ()
	{
		return "yay";
	}

	public Vector2 GetPosition ()
	{
		return position;
	}

	public int GetRotation ()
	{
		return rotation;
	}


	public IModuleInfo[] GetJoints ()
	{
		throw new NotImplementedException ();
	}
}
