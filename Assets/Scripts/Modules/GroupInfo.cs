using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupInfo : MonoBehaviour {
	Dictionary<Vector2, IModuleInfo> OccupiedBlocks = new Dictionary<Vector2, IModuleInfo>();
	Dictionary<Vector2, IModuleInfo> APs = new Dictionary<Vector2, IModuleInfo>();

	void Start ()
	{
		// add all children to OccupiedBlocks
		// generate APs - iterate through all children, generate AP if joint is free
	}

	public void showAPs ()
	{
		foreach (KeyValuePair<Vector2, IModuleInfo> value in APs)
		{
			
		}

		foreach (Transform child in transform)
		{
//			child.gameObject.SetActive(false);

		}

	}
}
