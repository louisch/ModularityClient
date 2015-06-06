using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody2D))]
public class ObjectStatusController : MonoBehaviour {
	PhotonView view;
	public PhotonView View
	{
		get
		{
			return view;
		}
		set
		{
			view = value;
		}
	}
	public int TrackerID
	{
		get
		{
			return view.viewID;
		}
		set
		{
			view.viewID = value;
		}
	}
	Rigidbody2D rb;
	public Rigidbody2D Rb
	{
		set
		{
			rb = value;
		}
	}


	void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (!stream.isWriting)
		{
			string got = "Some string";
			stream.Serialize(ref got);
			Debug.Log (got);
		}
		else if (stream.isWriting)
		{
			Debug.LogError ("Server object is receiving positional info from client");
		}
	}
}
