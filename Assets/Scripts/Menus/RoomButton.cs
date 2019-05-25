using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

[RequireComponent (typeof (Button))]
public class RoomButton : MonoBehaviour {

    // TODO: change string to more appropriate type
    private Action<string> Action;

	void Awake ()
    {
        GetComponent<Button> ().onClick.AddListener(OnPress);
    }

	// Use this for initialization
    void Start () {}
	// Update is called once per frame
	void Update () {}

    /**
     * TODO: Change action to appropriate typed argument to connect to server.
     */
    public void AddAction (Action<string> Action)
    {
        this.Action = Action;
    }

    /**
     * TODO: Pass connection details.
     */
    private void OnPress ()
    {
        Action("test");
    }
}
