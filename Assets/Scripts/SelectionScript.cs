using UnityEngine;
using System.Collections;

public class SelectionScript : MonoBehaviour {
	
	Renderer rend;
	Color originalColor;

	void Start ()
	{
		rend = GetComponent<Renderer> ();
		originalColor = rend.material.color;
		rend.enabled = true;
	}

	void OnMouseDown ()
	{
		CurrentObjectScript.Select (gameObject);
	}

	public void Highlight ()
	{
		rend.material.color = Color.red;
	}

	public void UnHighlight ()
	{
		rend.material.color = originalColor;
	}
}
