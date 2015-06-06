using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class InputFieldNavigator : MonoBehaviour {

    private const KeyCode SWITCH_FIELDS_KEY = KeyCode.Tab;
    private const KeyCode SWITCH_FIELDS_MODIFIER = KeyCode.LeftShift;
    private EventSystem eventSystem;

	// Use this for initialization
	void Start ()
    {
        eventSystem = EventSystem.current;
	}

	// Update is called once per frame
	void Update ()
    {
        // Until Navigation option for Input Fields is fixed, this is necessary.
        if (Input.GetKeyDown (SWITCH_FIELDS_KEY) &&
            eventSystem.currentSelectedGameObject == gameObject)
        {
            Selectable next = !Input.GetKeyDown (SWITCH_FIELDS_MODIFIER) ?
                gameObject.GetComponent<Selectable> ().FindSelectableOnDown () :
                gameObject.GetComponent<Selectable> ().FindSelectableOnUp ();
            if (next != null)
            {
                eventSystem.SetSelectedGameObject (next.gameObject);
                next.Select ();
                InputField field = (InputField)next;
                if (field != null)
                {
                    field.OnPointerClick (new PointerEventData (eventSystem));
                }
            }
        }
    }
}
