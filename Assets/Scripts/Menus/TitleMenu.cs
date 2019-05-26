using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleMenu : MonoBehaviour {

	private const string LOBBY_LEVEL_NAME = "Lobby";

	// Use this for initialization
	void Start() {}

	// Update is called once per frame
	void Update() {}

	public void OnPressPvPButton()
	{
		LoadLobby();
	}

	public void OnPressSettingsButton()
	{
	}

	private void LoadLobby()
	{
		SceneManager.LoadScene(LOBBY_LEVEL_NAME);
	}
}
