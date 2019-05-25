using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Canvas))]
public class RoomList : MonoBehaviour {

    public RoomButton roomButtonPrefab;

    private float buttonHeight;
    private int buttonCount;

	// Use this for initialization
	void Start ()
    {
        buttonCount = 0;
        buttonHeight = roomButtonPrefab.GetComponent<RectTransform> ()
            .rect.height;

        // For debugging
        AddButton ("hello");
        AddButton ("hello");
        AddButton ("hello");
    }

	// Update is called once per frame
	void Update () {}

    /**
     * Instantiate a button and
     */
    private void AddButton (string text)
    {
        RoomButton button = Instantiate<RoomButton> (roomButtonPrefab);
        button.transform.SetParent (transform);

        // Set where the button is and the text it displays
        SetButtonPos (button.GetComponent<RectTransform> ());
        buttonCount += 1;
        SetButtonText (button, text);

        button.AddAction (OnRoomButtonPressed);
    }

    /**
     * TODO: Change notifyString to appropriate typed argument to connect to server.
     * This is a callback that gets called by any cloned buttons when they are pressed.
     */
    public void OnRoomButtonPressed (string notifyString)
    {
        Debug.Log(notifyString);
    }

    private void SetButtonPos (RectTransform buttonRect)
    {
        buttonRect.anchoredPosition = Vector3.zero;
        buttonRect.anchoredPosition += -1 * Vector2.up * buttonHeight * buttonCount;
    }

    private void SetButtonText (RoomButton button, string text)
    {
        Text[] buttonTexts = button.GetComponentsInChildren<Text> ();
        if (buttonTexts.Length != 1)
        {
            Debug.LogErrorFormat ("Number of Text components in button: {0}. " +
                                  "This should be one.", buttonTexts.Length);
        }
        buttonTexts[0].text = text;
    }
}
