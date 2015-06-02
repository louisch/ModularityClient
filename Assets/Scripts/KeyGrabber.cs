using UnityEngine;
using System.Collections;

public class KeyGrabber : MonoBehaviour {

	void Update ()
	{
		SelectedObjectsScript.SetShift (Input.GetButton ("Shift"));      
	}
}
