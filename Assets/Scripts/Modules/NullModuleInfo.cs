using UnityEngine;
using System.Collections;

public class NullModuleInfo : MonoBehaviour, IModuleInfo {
	// This class is attached to a ModuleInfo's joint to prevent anything else from being attached (e.g. if a module only has 3 attachment points)	
}
