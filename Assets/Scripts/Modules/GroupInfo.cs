using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupInfo : MonoBehaviour
{
	Dictionary<IntVector2, Module> OccupiedBlocks = new Dictionary<IntVector2, Module> ();
	Dictionary<IntVector2, Module> APs = new Dictionary<IntVector2, Module> ();

	// bool IsValidAttachment (Module module, IntVector2 groupPosition, IntVector2 );

    void Start ()
	{
		// add all children to OccupiedBlocks
		foreach (Transform child in transform)
		{
			Module childMI = child.gameObject.GetComponent<Module> ();
			OccupiedBlocks.Add (childMI.GetPosition (), childMI);
		}

		// generate APs - iterate through all children, generate AP if joint is free
		foreach (KeyValuePair<IntVector2, Module> value in APs)
		{
			Module module = value.Value;
			Module[] joints = module.GetJoints ();
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
		foreach (KeyValuePair<IntVector2, Module> value in APs)
        {

        }

        foreach (Transform child in transform)
        {
			//child.gameObject.SetActive (false);
        }

    }
}
