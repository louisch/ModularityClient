using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupInfo : MonoBehaviour
{
	Dictionary<IntVector2, ModuleInfo> OccupiedBlocks = new Dictionary<IntVector2, ModuleInfo> ();
	Dictionary<IntVector2, ModuleInfo> APs = new Dictionary<IntVector2, ModuleInfo> ();

	// bool IsValidAttachment (ModuleInfo module, IntVector2 groupPosition, IntVector2 );

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
			ModuleInfo module = value.Value;
			ModuleInfo[] joints = module.GetJoints ();
			for (int i = 0; i < joints.Length; i++)
			{
				//if (module.validJoints[i])
				//{

				//}
			}
		}
		Debug.Log (OccupiedBlocks.Count);
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
