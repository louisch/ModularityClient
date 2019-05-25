using UnityEngine;
using System.Collections;

public class ClickModule : MonoBehaviour {
	
	Renderer rend;
	Color oriColor;
	GameObject playerGroup;

	void Start ()
	{
		rend = GetComponent<Renderer> ();
		oriColor = rend.material.color;
		playerGroup = GameObject.FindWithTag ("Player");
		// POSSIBLE HAXX - relies on only one object tagged "Player"
	}

	void OnMouseDown ()
	{
		Debug.Log ("clicked stuff");
		SelectedObjectsScript.Select (gameObject);
		playerGroup.GetComponent<GroupInfo> ().showAPs();
	}

	public void Highlight ()
	{
		rend.material.color = Color.blue;
	}

	public void UnHighlight ()
	{
		rend.material.color = oriColor;
	}
}
