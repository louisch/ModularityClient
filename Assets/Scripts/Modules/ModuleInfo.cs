using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModuleInfo : MonoBehaviour, IModuleInfo {

	// joints
	// other stats e.g. rotation

	public int posX;
	public int posY;

	public int rotation = 0;

	public ModuleInfo[] joints = new ModuleInfo[4];

	void Start ()
	{

	}

	public void ChangeParent (GroupInfo parent, Vector2 pos)
	{
		SetPosition (pos);
		transform.SetParent (parent.gameObject.transform);
	}

	public void SetPosition (Vector2 pos)
	{
//		this.posX = pos.x;
//		this.posY = pos.y;
//		transform.localPosition.x = posX * 2.5; // MAGIC NO HAXX
//		transform.localPosition.y = posY * 2.5; // MAGIC NO HAXX
	}
}
