using UnityEngine;
using System.Collections;

public class CharaterController : MonoBehaviour {
	private CharaterController cc;

	// Use this for initialization
	void Start ()
	{
		cc = GetComponent<CharacterController> ();
	}

}
