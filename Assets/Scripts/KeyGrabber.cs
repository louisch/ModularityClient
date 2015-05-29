using UnityEngine;
using System.Collections;

public class KeyGrabber : MonoBehaviour {

	void Update ()
	{
		CurrentObjectScript.SetShift (Input.GetButton ("Shift"));      
	}
}
