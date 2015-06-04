using UnityEngine;
using System.Collections;

interface IModuleInfo
{
	//int GetRotation ();
	string PartyTime ();
	Vector2 GetPosition ();
	int GetRotation ();
	IModuleInfo[] GetJoints ();
}
