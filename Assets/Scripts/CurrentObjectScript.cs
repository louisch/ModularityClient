using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurrentObjectScript : MonoBehaviour {

  List<GameObject> currentObjects = new List<GameObject>();
  bool isShift = false;

  void Update()
  {
    if(Input.GetButton("Shift"))
    {
      isShift = true;      
    }
    else
    {
      isShift = false;
    }
  }
  public void Select (GameObject obj)
  {
    if(obj.tag == "Background")
    {
      if(isShift)
      {
        return;
      }
      else
      {
        Clear();
      }
    }
    else
    {
      if(!isShift)
      {
        Clear();
      }
      currentObjects.Add(obj);
      obj.GetComponent<SelectionScript>().Highlight();
    }
  }

  void Clear()
  {
      foreach (GameObject e in currentObjects)
      {
        e.GetComponent<SelectionScript>().UnHighlight();
      }
      currentObjects.Clear();
  }
}
