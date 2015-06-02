using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectedObjectsScript : MonoBehaviour {

	static List<GameObject> selectedObjects = new List<GameObject> ();
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
			selectedObjects.Add (obj);
			obj.GetComponent<ClickModuleScript> ().Highlight ();
		}
	}

	public static void Clear ()
	{
			foreach (GameObject e in selectedObjects)
			{
			e.GetComponent<ClickModuleScript> ().UnHighlight ();
			}
			selectedObjects.Clear ();
	}

	public static void SetShift(bool value)
	{
		isShift = value;
	}

	public static List<GameObject> GetSelectedObjects ()
	{
		return selectedObjects;
	}
}
