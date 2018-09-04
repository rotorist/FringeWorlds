using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTCombatFollow : BTLeaf
{
	private ShipBase _followTarget;

	public override void Initialize ()
	{
		//Debug.Log("Initializing combat follow");
	}

	public override BTResult Process ()
	{
		if(Parameters[0] == "Leader")
		{

			if(MyAI.MyShip == MyParty.SpawnedShipsLeader || MyParty.SpawnedShipsLeader == null)
			{
				return Exit(BTResult.Success);
			}



			_followTarget = MyParty.SpawnedShipsLeader;


			float dist = Vector3.Distance(MyAI.MyShip.transform.position, _followTarget.transform.position);
			if(dist > 40)
			{
				MyAI.Whiteboard.Parameters["Destination"] = _followTarget.transform.position;
				MyAI.Whiteboard.Parameters["AimPoint"] = Vector3.zero;

				return Running();
			}
			else
			{
				return Exit(BTResult.Success);
			}
		}
		else if(Parameters[0] == "Escorts")
		{
			//find one of the followers
			if(MyParty.SpawnedShips.Count <= 1)
			{
				return Exit(BTResult.Success);
			}

			if(_followTarget == null)
			{
				foreach(ShipBase ship in MyParty.SpawnedShips)
				{
					if(ship != MyAI.MyShip)
					{
						_followTarget = ship;
						break;
					}
				}
			}

			float dist = Vector3.Distance(MyAI.MyShip.transform.position, _followTarget.transform.position);
			if(dist < 30)
			{
				MyAI.Whiteboard.Parameters["Destination"] = Vector3.zero;
				return Exit(BTResult.Success);
			}
			else if(dist > 40)
			{
				MyAI.Whiteboard.Parameters["Destination"] = _followTarget.transform.position;
				MyAI.Whiteboard.Parameters["AimPoint"] = Vector3.zero;


			}

			return Running();

		}

		return Exit(BTResult.Success);

	}

	public override BTResult Exit (BTResult result)
	{

		_followTarget = null;
		return result;
	}

	public override BTResult Running ()
	{
		MyAI.RunningNodeHist.UniquePush("Combat Follow");
		return BTResult.Running;
	}

}
