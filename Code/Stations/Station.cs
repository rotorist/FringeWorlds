using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : StationBase
{
	public List<DockGate> DockGates;

	private List<DockingSession> _dockingSessions;

	void Start()
	{
		_dockingSessions = new List<DockingSession>();
	}

	void FixedUpdate()
	{
		List<DockingSession> sessionsCopy = new List<DockingSession>(_dockingSessions);
		foreach(DockingSession session in sessionsCopy)
		{
			session.UpdateDockingSession();
			if(session.Stage == DockingSessionStage.Docked)
			{
				//remove the session, close the gate
			}
		}
	}

	public override DockRequestResult Dock (ShipBase requester)
	{
		//check if requester is allowed to dock

		//find an available gate
		DockGate selectedGate = null;
		foreach(DockGate gate in DockGates)
		{
			selectedGate = gate;

			foreach(DockingSession s in _dockingSessions)
			{
				if(s.Gate == selectedGate)
				{
					selectedGate = null;
				}
			}

			if(selectedGate != null)
			{
				break;
			}
		}

		if(selectedGate == null)
		{
			return DockRequestResult.Busy;
		}

		DockingSession session = new DockingSession(selectedGate, requester, this);
		_dockingSessions.Add(session);

		return DockRequestResult.Accept;
	}

	public override void OnDetectDocking (string triggerID, ShipBase requester)
	{
		//only player will be able to trigger this

	}
}
