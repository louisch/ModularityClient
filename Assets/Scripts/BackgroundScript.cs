using UnityEngine;
using System.Collections;

public class BackgroundScript : MonoBehaviour {

  public GameObject currentGameObjectHandler;

  CurrentObjectScript objScript;

  void Start()
  {
    objScript = currentGameObjectHandler.GetComponent<CurrentObjectScript>();  
  }
  void OnMouseDown()
  {
    objScript.Select(gameObject); 
  }
}
