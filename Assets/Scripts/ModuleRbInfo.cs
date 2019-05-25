using UnityEngine;

public abstract class RigidbodyInfo
{
	abstract public float mass {get;}
	abstract public float drag {get;}
	abstract public float angularDrag {get;}
	public float gravityScale  {get{return 0;}}
}

public class PilotModuleRigidbodyInfo : RigidbodyInfo
{
	override public float mass {get{return 1;}}
	override public float drag {get{return 4;}}
	override public float angularDrag {get{return 10;}}
}

public class RandomModuleRigidbodyInfo : RigidbodyInfo
{
	override public float mass {get{return 0.5f;}}
	override public float drag {get{return 2;}}
	override public float angularDrag {get{return 4;}}
}

public class InfoFromRigidbody : RigidbodyInfo
{
	float _mass;
	float _drag;
	float _angularDrag;
	override public float mass {get{return _mass;}}
	override public float drag {get{return _drag;}}
	override public float angularDrag {get{return _angularDrag;}}

	public InfoFromRigidbody (Rigidbody2D body)
	{
		_mass = body.mass;
		_drag = body.drag;
		_angularDrag = body.angularDrag;
	}
}
