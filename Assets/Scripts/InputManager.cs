using UnityEngine;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

public class InputManager : MonoBehaviour {
	// singleton pattern implementation
	private static volatile InputManager instance;
	private static object _lock = new Object ();
	public static InputManager Instance
	{
		get
		{
			if (!accessible)
			{
				Debug.LogWarning ("Input manager has already been destroyed.");
				return null;
			}
			else if (instance == null)
			{
				lock (_lock)
				{
					if (instance == null)
					{
						instance = (InputManager) FindObjectOfType(typeof(InputManager));
						if (FindObjectsOfType(typeof(InputManager)).Length > 1)
						{
							Debug.LogError ("Multiple input manager were spawned!");
						}
						if (instance == null)
						{
							GameObject inputManager = new GameObject();
							instance = inputManager.AddComponent<InputManager> ();
							inputManager.name = "(singleton)InputManager";
							DontDestroyOnLoad (inputManager);

							Debug.Log ("Created input manager");
						}
						else
						{
							Debug.Log ("Input manager already exists");
						}
					}
				}
			}
			return instance;
		}
	}

	static bool accessible = true;

	public float ThrustAxis {get; private set;}
	public float StrafeAxis {get; private set;}
	public float TorqueAxis {get; private set;}
	public Vector3 MousePosition {get; private set;}
	bool[] inputBits;

	// NOTE: enum MUST correspond to string array
	public enum InputMap // shorthand names for all virtual buttons - for internal use
	{
		FIRE,
		SELECT,
		SHIFT,
		CTRL,
		MENU,
		ENTER,
		CANCEL,
		G1,
		G2,
		G3,
		G4,
		G5,
		G6,
		G7,
		G8,
		G9,
		G0
	};
	int[] inputMapValues; // holds InputMap values for quick reference - it is built at runtime
	string[] inputButtons = new string[] // string names for every virtual button - they correspond to input manager names
	{
		"Fire",
		"Select",
		"Shift",
		"Ctrl",
		"Menu",
		"Enter",
		"Cancel",
		"Group 1",
		"Group 2",
		"Group 3",
		"Group 4",
		"Group 5",
		"Group 6",
		"Group 7",
		"Group 8",
		"Group 9",
		"Group 0"
	};


	void Awake ()
	{
		inputMapValues = (int[])InputMap.GetValues(typeof(InputMap));
		inputBits = new bool[inputMapValues.Length];
	}
	
	void Update ()
	{
		ThrustAxis = Input.GetAxis ("Thrust");
		StrafeAxis = Input.GetAxis ("Strafe");
		TorqueAxis = -Input.GetAxisRaw ("Torque");

		for (int i = 0; i < inputButtons.Length; i++)
		{
			inputBits[inputMapValues[i]] = Input.GetButton(inputButtons[i]);
		}
	}

	public bool CheckKey (InputMap key)
	{
		return inputBits[(int)key];
	}

	void OnDestroy ()
	{
		accessible = false;
	}
}
