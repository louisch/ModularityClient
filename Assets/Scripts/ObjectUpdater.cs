using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public abstract class ObjectUpdater : MonoBehaviour {
	// player's network info
	public PhotonPlayer Owner {get; set;}
	public PhotonView View {get; private set;}

	public virtual double LerpTime { get; protected set; }

	// public accessors for viewID
	public int ViewID
	{
		get
		{
			return View.viewID;
		}
		set
		{
			View.viewID = value;
		}
	}

	public abstract void UpdatePos (ref Vector3 serverPos, double updateTS);

	public virtual void Awake ()
	{
		View = GetComponent<PhotonView> ();
	}
}
