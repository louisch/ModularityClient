using UnityEngine;
using System.Collections;

public class SelectionScript : MonoBehaviour {

    GameObject currentGameObjectHandler;

    Renderer rend;
    Color originalColor;
    Rigidbody rb;
    float force = 1000f;
    CurrentObjectScript objScript;

	// Use this for initialization
	void Start () {
        currentGameObjectHandler = GameObject.FindWithTag("CurrentObject");
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        rend.enabled = true;
        rb = GetComponent<Rigidbody>();
        objScript = currentGameObjectHandler.GetComponent<CurrentObjectScript>();
	}
	
    void OnMouseDown()
    {   
        Debug.Log("Part");
        objScript.Select(gameObject);
    }

    public void Highlight()
    {
        rend.material.color = Color.red;
    }

    public void UnHighlight()
    {
        rend.material.color = originalColor;
    }


}
