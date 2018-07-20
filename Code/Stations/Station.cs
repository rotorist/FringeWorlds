using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : StationBase
{
	public List<DockGate> DockGates;
	public List<MacroAIParty> DockedParties;

	private List<DockingSession> _dockingSessions;


	void FixedUpdate()
	{
		List<DockingSession> sessionsCopy = new List<DockingSession>(_dockingSessions);
		foreach(DockingSession session in sessionsCopy)
		{
			session.UpdateDockingSession();
			if(session.Stage == DockingSessionStage.Docked || session.Requester == null)
			{
				//remove the session, close the gate
				_dockingSessions.Remove(session);
			}
		}
	}

	public override void Initialize ()
	{
		_dockingSessions = new List<DockingSession>();

		//find all children's station component and assign to this
		StationComponent [] components = transform.GetComponentsInChildren<StationComponent>();
		foreach(StationComponent comp in components)
		{
			comp.ParentStation = this;
		}

		DockedParties = new List<MacroAIParty>();
	}

	public override DockRequestResult Dock (ShipBase requester, out DockSessionBase session)
	{
		//check if requester is allowed to dock

		//find an available gate
		DockGate selectedGate = FindAvailableGate();
		if(selectedGate == null)
		{
			session = null;
			return DockRequestResult.Busy;
		}

		DockingSession stationSession = new DockingSession(selectedGate, requester, this, false);
		_dockingSessions.Add(stationSession);
		session = stationSession;

		return DockRequestResult.Accept;
	}

	public override DockRequestResult Undock (ShipBase requester, out DockSessionBase session)
	{
		//find an available gate
		DockGate selectedGate = FindAvailableGate();

		if(selectedGate == null)
		{
			session = null;
			return DockRequestResult.Busy;
		}

		session = new DockingSession(selectedGate, requester, this, true);
		_dockingSessions.Add((DockingSession)session);

		return DockRequestResult.Accept;

	}

	public override void OnDetectDocking (string triggerID, ShipBase requester)
	{
		//only player will be able to trigger this

	}

	public override void OnDockingSessionComplete (DockingSession session)
	{
		session.Requester.IsInPortal = false;
		_dockingSessions.Remove(session);
	}

	private DockGate FindAvailableGate()
	{
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

		return selectedGate;
	}
}
