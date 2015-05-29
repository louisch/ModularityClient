using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurrentObjectScript : MonoBehaviour {

	static List<GameObject> currentObjects = new List<GameObject> ();
	static bool isShift = false;

	public static void Select (GameObject obj)
	{
		if (obj.tag == "Background")
		{
			if (!isShift)
			{
				Clear ();
			}
		}
		else
		{
			if (!isShift)
			{
				Clear ();
			}
			currentObjects.Add (obj);
			obj.GetComponent<SelectionScript> ().Highlight ();
		}
	}

	static void Clear ()
	{
			foreach (GameObject e in currentObjects)
			{
				e.GetComponent<SelectionScript> ().UnHighlight ();
			}
			currentObjects.Clear ();
	}

	public static void SetShift(bool value)
	{
		isShift = value;
	}
}
