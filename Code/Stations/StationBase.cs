using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationBase : MonoBehaviour 
{

	public string DisplayName;
	public string ID;
	public StationType StationType;

	public virtual void Initialize()
	{

	}

	public virtual DockRequestResult Dock(ShipBase requester)
	{
		return DockRequestResult.Deny;
	}

	public virtual DockRequestResult Undock(ShipBase requester)
	{
		return DockRequestResult.Deny;
	}

	public virtual void OnDetectDocking(string triggerID, ShipBase requester)
	{

	}

	public virtual void OnDockingSessionComplete(DockingSession session)
	{

	}
}

public class StationData
{
	public string DisplayName;
	public string ID;
	public Vector3 Location;
	public Vector3 EulerAngles;
	public StationType StationType;

}

public enum DockRequestResult
{
	Accept,
	Busy,
	Deny,
}

public enum StationType
{
	Station,
	Tradelane,
	JumpGate,
	JumpHole,
}