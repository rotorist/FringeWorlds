using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingSession
{
	public DockingSessionStage Stage;
	public DockGate Gate;
	public ShipBase Requester;
	public StationBase ParentStation;

	public DockingSession(DockGate gate, ShipBase requester, StationBase parent)
	{
		Gate = gate;
		Requester = requester;
		ParentStation = parent;
		Stage = DockingSessionStage.Granted;

		//if requester is player, turn light green

	}

	public void UpdateDockingSession()
	{
		if(Stage == DockingSessionStage.Granted)
		{

		}
	}
}

public enum DockingSessionStage
{
	Granted,
	Docking,
	Docked,
}