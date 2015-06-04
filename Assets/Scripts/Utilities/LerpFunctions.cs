using UnityEngine;

/**
* Utility class for all sorts of lerp functions, for maximum lerpiness
*/
public class LerpFunctions {

	/* Simple "ease out". */
	public static float Sin (float lerp)
	{
		return Mathf.Sin (lerp * Mathf.PI * .5f);
	}

	/* Simple "ease in". */
	public static float Cos (float lerp)
	{
		return 1 - Mathf.Cos (lerp * Mathf.PI * .5f);
	}

	/* Ease-in with a deeper curve. */
	public static float Quadratic (float lerp)
	{
		return lerp*lerp;
	}

	/* Smooth start and end, with linear middle. */
	public static float SmoothStep (float lerp)
	{
		return lerp*lerp * (3 - 2*lerp);
	}

	/* Smoother version of SmoothSteperp. */
	public static float UltraSmoothStep (float lerp)
	{
		return lerp*lerp*lerp * (lerp * (6*lerp - 15) + 10);
	}
}
