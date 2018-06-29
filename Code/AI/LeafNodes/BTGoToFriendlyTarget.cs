using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTGoToFriendlyTarget : BTLeaf
{

	public override void Initialize ()
	{

	}

	public override BTResult Process ()
	{
		//Debug.Log("GoToFriendlyTarget processing, I am " + MyAI.MyShip.name);
		ShipBase target = (ShipBase)MyAI.Whiteboard.Parameters[Parameters[0]];
		if(target != null)
		{
			if(MyParty.SpawnedShipsLeader == null || target.MyAI.IsDocked || MyAI.IsDocked)
			{
				Debug.LogError("leader is docked in station, or I'm docked in station, can't follow ");
				return Exit(BTResult.Fail);
			}

			if(MyParty.SpawnedShipsLeader.IsInPortal && MyParty.NextNode.NavNodeType == NavNodeType.Station)
			{
				Debug.Log("leader is docking on a station, stop following ");
				return BTResult.Running;
			}

			Vector3 dest = target.transform.position;
			if(MyParty.Formation.ContainsKey(MyAI.MyShip))
			{
				dest = MyParty.SpawnedShipsLeader.transform.TransformPoint(MyParty.Formation[MyAI.MyShip]);
				MyAI.Whiteboard.Parameters["Destination"] = dest;
				//Debug.Log("Going towards formation point ");
				return BTResult.Running;
			}
			else
			{

				float dist = Vector3.Distance(MyAI.MyShip.transform.position, dest);
				if(dist <= (float)MyAI.Whiteboard.Parameters["FriendlyFollowDist"])
				{
					return Exit(BTResult.Success);
				}
				else
				{
					//Debug.Log("Going towards target ");
					MyAI.Whiteboard.Parameters["Destination"] = dest;
					return BTResult.Running;
				}
			}
		}
		else
		{
			return Exit(BTResult.Fail);
		}

	}

	public override BTResult Exit (BTResult result)
	{
		Debug.Log("Go to friendly " + result + " " + MyAI.MyShip.name);
		return result;
	}
}
