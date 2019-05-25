using UnityEngine;
using System.Collections;

public class TitleMenu : MonoBehaviour {

    private const string LOBBY_LEVEL_NAME = "Lobby";

	// Use this for initialization
	void Start () {}

	// Update is called once per frame
	void Update () {}

    public void OnPressPvPButton ()
    {
        LoadLobby ();
    }

    public void OnPressSettingsButton ()
    {
    }

    private void LoadLobby ()
    {
        Application.LoadLevel (LOBBY_LEVEL_NAME);
    }
}
