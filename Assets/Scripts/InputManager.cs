using UnityEngine;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

/**
* Singleton, thread-safe class used for tracking player input.
* Checks for input each call to Update.
*/
public class InputManager : MonoBehaviour {
	/* singleton pattern implementation fields */
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
	// accessible is false when this object is destroyed, in order to prevent it from being created again.
	static bool accessible = true; 

	/* Input axes accessors */
	public float ThrustAxis {get; private set;}
	public float StrafeAxis {get; private set;}
	public float TorqueAxis {get; private set;}
	public float ZoomDelta {get; private set;}
	public Vector2 MousePosition {get; private set;}

	/* Internal array used to keep track of pressed buttons. */
	bool[] inputBits;

	/* Enum/string array combo for accessing keys */
	// NOTE: enum order MUST correspond to string array, other all is break
	//// shorthand names for external use
	public enum InputMap 
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
	//// holds InputMap values for quick reference - it is built at runtime and used internally - don't worry about it
	int[] inputMapValues;
	//// string names of every virtual button - MUST correspond to input manager names
	// These are not updated dynamically in order to make sure that the enum above corresponds to this array
	string[] inputButtons = new string[] 
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

	/**
	* On creation, computes the enum value list for quick reference (this is more efficient than computing it each time).
	* Also initialises input key array.
	*/
	void Awake ()
	{
		inputMapValues = (int[])InputMap.GetValues(typeof(InputMap));
		inputBits = new bool[inputMapValues.Length];
	}
	
	/**
	* At each update, fetches axes and button values and updates relevant fields.
	*/
	void Update ()
	{
		ThrustAxis = Input.GetAxis ("Thrust");
		StrafeAxis = Input.GetAxis ("Strafe");
		TorqueAxis = -Input.GetAxisRaw ("Torque");
		ZoomDelta = Input.GetAxis ("Camera Zoom");
		MousePosition = Input.mousePosition;

		for (int i = 0; i < inputButtons.Length; i++)
		{
			inputBits[inputMapValues[i]] = Input.GetButton(inputButtons[i]);
		}
	}

	/**
	* Use externally to get the value of the key you want.
	*/
	public bool CheckKey (InputMap key)
	{
		return inputBits[(int)key];
	}

	/**
	* Returns true if movement input axis are 0.
	*/
	public bool IsMoving ()
	{
		return StrafeAxis != 0 || ThrustAxis != 0;
	}

	/**
	* Returns the input vector for this frame.
	*/
	public Vector2 GetNormalizedInputVector ()
	{
		return new Vector2 (StrafeAxis, ThrustAxis).normalized;
	}

	/**
	* Sets 'accessible' to false in order to make sure that a new InputManager is not spawned upon its destruction.
	* Thus, the InputManager should only be destroyed when the game is shutting down, or something.
	*/
	void OnDestroy ()
	{
		accessible = false;
	}
}
