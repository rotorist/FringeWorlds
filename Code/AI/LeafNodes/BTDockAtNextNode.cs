using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTDockAtNextNode : BTLeaf
{
	private DockSessionBase _currentSession;


	public override void Initialize ()
	{
		_currentSession = null;
	}

	public override BTResult Process ()
	{
		if(MyAI.MyParty == null || MyAI.MyParty.NextNode == null)
		{
			return BTResult.Fail;
		}

		if(MyAI.MyParty.NextNode.NavNodeType == NavNodeType.Station)
		{
			//if not destination then don't dock
			if(MyAI.MyParty.CurrentTask.TravelDestNodeID != MyAI.MyParty.NextNode.ID)
			{
				return BTResult.Fail;
			}

			//if too far away from station then dock fail
			if(Vector3.Distance(MyAI.MyParty.Location, MyAI.MyParty.NextNode.Location) > 40)
			{
				return BTResult.Fail;
			}

			if(_currentSession == null)
			{
				DockRequestResult result = GameManager.Inst.WorldManager.CurrentSystem.GetStationByID(MyAI.MyParty.NextNode.ID).Dock(MyAI.MyShip, out _currentSession);
				if(result == DockRequestResult.Busy)
				{
					return BTResult.Running;
				}
				else if(result == DockRequestResult.Deny)
				{
					return BTResult.Fail;
				}
				else
				{
					return BTResult.Running;
				}
			}
			else
			{
				if(!MyAI.MyShip.IsInPortal)
				{
					//fly towards docking target
					DockingSession session = (DockingSession)_currentSession;
					Vector3 dockTarget = session.GetDockEnterTarget();
				}
			}



			
		}


		return BTResult.Success;
	}

	public override BTResult Exit (BTResult result)
	{
		return result;
	}
}
