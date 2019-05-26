using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModuleGroup : MonoBehaviour {

	Dictionary<IntVector2, Module> OccupiedBlocks = new Dictionary<IntVector2, Module>();
	Dictionary<IntVector2, Module> APs = new Dictionary<IntVector2, Module>();

	void Start() {
		// add all children to OccupiedBlocks
		foreach (Transform child in transform) {
			Module childMI = child.gameObject.GetComponent<Module>();
			OccupiedBlocks.Add(childMI.GetPosition(), childMI);
		}

		// generate APs - iterate through all children, generate AP if joint is free
		foreach (KeyValuePair<IntVector2, Module> value in APs) {
			Module module = value.Value;
			Module[] joints = module.GetJoints();
			for (int i = 0; i < joints.Length; i++) {
			}
		}
		Debug.Log(OccupiedBlocks.Count);
	}

	public void showAPs() {
		foreach (KeyValuePair<IntVector2, Module> value in APs) {
		}

		foreach (Transform child in transform) {
		}
	}
}
