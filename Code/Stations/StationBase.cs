using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationBase : MonoBehaviour 
{

	public string DisplayName;
	public string ID;

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

public enum DockRequestResult
{
	Accept,
	Busy,
	Deny,
}

public enum StationType
{
	Tradelane,
	JumpGate,
	Station,
	JumpHole,
}