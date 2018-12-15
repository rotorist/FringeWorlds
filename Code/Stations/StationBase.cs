using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationBase : MonoBehaviour 
{

	public string DisplayName;
	public string ID;
	public StationType StationType;
	public List<StationBase> NeighborStations;

	public virtual void Initialize()
	{

	}

	public virtual DockRequestResult Dock(ShipBase requester, out DockSessionBase session)
	{
		session = null;
		return DockRequestResult.Deny;
	}

	public virtual DockRequestResult Undock(ShipBase requester, out DockSessionBase session)
	{
		session = null;
		return DockRequestResult.Deny;
	}

	public virtual void OnDetectDocking(string triggerID, ShipBase requester)
	{

	}

	public virtual void OnDockingSessionComplete(DockingSession session)
	{

	}
}

public class StationData : NavNode
{
	public string DisplayName;
	public Vector3 EulerAngles;
	public StationType StationType;
	public DockableStationData DockableStationData;


	public StationData()
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
	Station,
	Tradelane,
	JumpGate,
	JumpHole,
}