using UnityEngine;
using System.Collections;

public class BackgroundScript : MonoBehaviour {
	
	void OnMouseDown ()
	{
		SelectedObjectsScript.Select(gameObject);
	}
}
