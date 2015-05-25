using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public Transform map;

	public void createMap ()
	{
		Debug.Log ("Map creation routine recieved");
		Time.timeScale = 0; // pause time, ie, stop physics from simulating
		Instantiate(map, new Vector3 (0,0,0), Quaternion.identity);
	}
}
