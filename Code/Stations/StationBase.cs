using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationBase : MonoBehaviour 
{

	public string DisplayName;
	public string ID;

	public virtual DockRequestResult Dock(ShipBase requester)
	{
		return DockRequestResult.Deny;
	}

	public virtual void OnDetectDocking(string triggerID, ShipBase requester)
	{

	}
}

public enum DockRequestResult
{
	Accept,
	Busy,
	Deny,
}