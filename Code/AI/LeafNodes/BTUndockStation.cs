using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTUndockStation : BTLeaf
{
	private DockSessionBase _undockSession;

	public override void Initialize ()
	{
		_undockSession = null;
	}

	public override BTResult Process ()
	{
		if(!MyAI.IsDocked || MyAI.MyShip.DockedStationID == "")
		{
			if(_undockSession == null)
			{
				Debug.Log("BTUndock: Not docked!");
				return Exit(BTResult.Fail);
			}
			else
			{
				Debug.Log("BTUndock: success!");
				return Exit(BTResult.Success);
			}
		}

		StationBase station = GameManager.Inst.WorldManager.CurrentSystem.GetStationByID(MyParty.NextNode.ID);

		if(_undockSession == null)
		{
			DockRequestResult result = station.Undock(MyAI.MyShip, out _undockSession);
			if(result == DockRequestResult.Deny)
			{
				Debug.Log("BTUndock: Failed to undock");
				return Exit(BTResult.Fail);
			}
			else
			{
				//Debug.Log("BTUndock: running");
				return BTResult.Running;
			}
		}
		else
		{
			//Debug.Log("BTUndock: running");
			return BTResult.Running;
		}


	}

	public override BTResult Exit (BTResult result)
	{
		_undockSession = null;
		return result;
	}
}
