using UnityEngine;
using System.Collections;
using System.Net;
using System.Threading;

using UnityToRails;

public class LoginManager : MonoBehaviour {

    private const string TITLE_MENU_LEVEL_NAME = "TitleMenu";

    private Domain Root { get; set; }
    private LoginData Data { get; set; }

	// Use this for initialization
	void Start ()
    {
        Root = new Domain ("localhost:3000");
        Data = new LoginData ();
	}

	// Update is called once per frame
	void Update () {}

    public void SetEmail (string email)
    {
        Data.email = email;
    }

    public void SetPassword (string password)
    {
        Data.password = password;
    }

    public void InitiateLogin ()
    {
        Login.To (Root, Data);
        Application.LoadLevel (TITLE_MENU_LEVEL_NAME);
    }
}
