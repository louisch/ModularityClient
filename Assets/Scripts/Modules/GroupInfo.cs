using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupInfo : MonoBehaviour
{
	Dictionary<IntVector2, ModuleInfo> OccupiedBlocks = new Dictionary<IntVector2, ModuleInfo> ();
	Dictionary<IntVector2, ModuleInfo> APs = new Dictionary<IntVector2, ModuleInfo> ();

    void Start ()
	{
		// add all children to OccupiedBlocks
		foreach (Transform child in transform)
		{
			ModuleInfo childMI = child.gameObject.GetComponent<ModuleInfo> ();
			OccupiedBlocks.Add (childMI.GetPosition (), childMI);
		}

		// generate APs - iterate through all children, generate AP if joint is free
		foreach (KeyValuePair<IntVector2, ModuleInfo> value in APs)
		{
			//ModuleInfo[] joints = value.Value.GetJoints ();
			//foreach (ModuleInfo joint in joints)
			//{
			//	if (joint == null)
			//	{
			//		//if 
			//	}
			//}
		}
		//Debug.Log (OccupiedBlocks.Count);
	}

    public void showAPs ()
    {
		foreach (KeyValuePair<IntVector2, ModuleInfo> value in APs)
        {

        }

        foreach (Transform child in transform)
        {
			//child.gameObject.SetActive (false);
        }

    }
}
