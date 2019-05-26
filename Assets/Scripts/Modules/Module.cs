using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Module : MonoBehaviour {

	private List<Module> neighbors;
	public IntVector2[] occupiedBlocks;
	public BlockInfo[] occupiedBlockJoints;

	void Start() {}

	public void ChangeParent(ModuleGroup parent, IntVector2 pos, int rot) {
		SetPosition(pos);
		SetRotation(rot);
		transform.SetParent(parent.gameObject.transform);
	}

	public void SetPosition (IntVector2 pos) {
		// MAGIC NUMBER HAXX (implicit x1 normalisation because blocks are currently exactly 1x1 units in size)
		transform.localPosition.Set (
			Convert.ToSingle (pos.x),
			Convert.ToSingle (pos.y),
			transform.localPosition.z
		);
	}

	public IntVector2 GetPosition() {
		return new IntVector2(
			Convert.ToInt32(transform.position.x),
			Convert.ToInt32(transform.position.y)
		);
	}

	public void SetRotation(int rot) {
		transform.localEulerAngles.Set(0, 0, Convert.ToSingle(rot));
	}

	public int GetRotation() {
		return (Convert.ToInt32(transform.localEulerAngles.z) / 90) % 4;
	}

	public Module[] GetJoints() {
		return null;
	}
}
