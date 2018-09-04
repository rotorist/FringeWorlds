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
				//Debug.Log("BTUndock: success! " + MyAI.MyShip.name);
				//check if all members of the party have undocked
				if(MyAI.MyShip == MyParty.SpawnedShipsLeader)
				{
					
					bool allUndocked = true;
					foreach(ShipBase ship in MyParty.SpawnedShips)
					{
						//Debug.Log("docked station id " + ship.DockedStationID + " ship = " + ship.name);
						if(ship != MyAI.MyShip && ship.DockedStationID != "")
						{
							allUndocked = false;
						}
					}
					if(allUndocked)
					{
						MyParty.PrevNode = MyParty.NextNode;
						MyParty.NextTwoNodes.Clear();
						Debug.LogError("All party has undocked! " + MyAI.MyShip.name);
						return Exit(BTResult.Success);
					}
					else
					{
						return Running();
					}
				}
				else
				{
					
					return Exit(BTResult.Success);
				}
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
				return Running();
			}
		}
		else
		{
			//Debug.Log("BTUndock: running");
			return Running();
		}


	}

	public override BTResult Exit (BTResult result)
	{
		_undockSession = null;
		return result;
	}

	public override BTResult Running ()
	{
		MyAI.RunningNodeHist.UniquePush("Undock Station");
		return BTResult.Running;
	}
}
