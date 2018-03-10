using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Incident
{
	public IncidentType Type;
	public List<Factions> Factions;
	public Vector3 Location;
	public string DestStationID;
}

public enum IncidentType
{
	LeaveStation,
	EnterStation,
	Patrol,
	Battle,
	Ambush,
}