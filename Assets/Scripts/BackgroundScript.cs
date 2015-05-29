using UnityEngine;
using System.Collections;

public class BackgroundScript : MonoBehaviour {

	CurrentObjectScript objScript;

	void OnMouseDown ()
	{
		CurrentObjectScript.Select(gameObject);
	}
}
