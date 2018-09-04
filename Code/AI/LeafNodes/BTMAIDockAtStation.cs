using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTMAIDockAtStation : BTLeaf
{

	public override void Initialize ()
	{
		
	}

	public override BTResult Process ()
	{
		if(MyParty == null || MyParty.CurrentTask == null || MyParty.NextNode == null ||  MyParty.DockedStationID != "")
		{
			return Exit(BTResult.Fail);
		}



		if(Vector3.Distance(MyParty.Location.RealPos, MyParty.NextNode.Location.RealPos) > 10)
		{
			return Exit(BTResult.Fail);
		}

		if(MyParty.NextNode.NavNodeType == NavNodeType.Station && MyParty.CurrentTask.IsDestAStation && MyParty.CurrentTask.TravelDestNodeID == MyParty.NextNode.ID)
		{

			MyParty.DockedStationID = MyParty.NextNode.ID;

			foreach(ShipBase ship in MyParty.SpawnedShips)
			{
				if(ship.MyAI != null)
				{
					ship.MyAI.IsDocked = true;
					ship.DockedStationID = MyParty.NextNode.ID;
				}
			}
			Debug.Log("MAI Docked at " + MyParty.NextNode.ID);

			return Exit(BTResult.Success);
		}
		else if(MyParty.NextNode.NavNodeType == NavNodeType.JumpGate)
		{
			//change party system
			Debug.Log("MAI Docked at jumpgate " + MyParty.NextNode.ID);
			JumpGateData jg = (JumpGateData)MyParty.NextNode;
			JumpGateData otherJG = (JumpGateData)GameManager.Inst.WorldManager.AllNavNodes[jg.ExitGateID];
			MyParty.CurrentSystemID = jg.TargetSystem;
			MyParty.Location.RealPos = otherJG.Location.RealPos + otherJG.SpawnDisposition;
			MyParty.PrevNode = otherJG;
			MyParty.NextTwoNodes = new List<NavNode>();

			return Exit(BTResult.Success);
		}

		return Exit(BTResult.Fail);
	}

	public override BTResult Exit (BTResult result)
	{
		
		return result;
	}

	public override BTResult Running ()
	{
		string message = "MAIDockAtStation ";
		if(MyParty.NextNode != null)
		{
			message += MyParty.NextNode.ID;
		}
		MyParty.RunningNodeHist.UniquePush(message);
		return BTResult.Running;
	}
}
