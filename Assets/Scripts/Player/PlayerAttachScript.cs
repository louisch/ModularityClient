using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttachScript : MonoBehaviour {

	void OnMouseDown ()
	{
		Debug.Log ("Clicked player");
		List<GameObject> currentObjects = CurrentObjectScript.GetCurrentObjects();
		if (currentObjects.Count == 1)
		{
			Debug.Log ("Only one object selected, attaching");
			FixedJoint joint = gameObject.AddComponent<FixedJoint> ();
			joint.connectedBody = currentObjects[0].GetComponent<Rigidbody> ();

			// increase mass
			gameObject.GetComponent<Rigidbody> ().mass += 1;
		}
		CurrentObjectScript.Clear ();
	}
}
