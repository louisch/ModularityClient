using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupInfo : MonoBehaviour {
	Dictionary<Vector2, GameObject> OccupiedBlocks = new Dictionary<Vector2, GameObject>();
	Dictionary<Vector2, GameObject> APs = new Dictionary<Vector2, GameObject>();

	void Start ()
	{
		// add all children to OccupiedBlocks
		// generate APs - iterate through all children, generate AP if joint is free
	}

	public void showAPs ()
	{
		foreach (KeyValuePair<Vector2, GameObject> value in APs)
		{
			
		}

	}
}
