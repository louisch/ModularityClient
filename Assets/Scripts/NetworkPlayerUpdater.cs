using UnityEngine;
using System.Collections;

// updater for object not owned by player
public class NetworkPlayerUpdater : ObjectUpdater {
	public double alpha = 0.5; // weight attributed to previous update delta
	double previousUpdateTS; // timestamp of previous update

	// initialise time of previous update TS to time of initialisation
	public override void Awake ()
	{
		previousUpdateTS = Network.time;
		base.Awake ();
	}

	// updates lerp time at each network update
	public override void UpdatePos (ref Vector3 serverPos, double updateTS)
	{
		// lerp time is a weighted average between current update delta and previous update delta
		base.LerpTime = alpha * base.LerpTime + (1 - alpha) * (updateTS - previousUpdateTS);
		previousUpdateTS = updateTS;
	}
}
