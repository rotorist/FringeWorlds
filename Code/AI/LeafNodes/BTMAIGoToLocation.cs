using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTMAIGoToLocation : BTLeaf
{

	public override void Initialize ()
	{

	}

	public override BTResult Process ()
	{
		if(MyParty.DockedStationID != "")
		{
			return Exit(BTResult.Fail);
		}

		if(Parameters[0] == "NextNode")
		{
			//check if we are near next node
			if(MyParty == null || MyParty.NextNode == null)
			{
				return Exit(BTResult.Fail);
			}

			if(!MyParty.CurrentTask.IsDestAStation && MyParty.DestNode == MyParty.NextNode)
			{
				return Exit(BTResult.Fail);
			}

			if(Vector3.Distance(MyParty.Location.RealPos, MyParty.NextNode.Location.RealPos) < 10)
			{
				MyParty.PrevNode = MyParty.NextNode;
				if(!MyParty.CurrentTask.IsDestAStation && MyParty.DestNode.ID == MyParty.NextNode.ID)
				{
					//has reached dest node, but hasn't reached dest coord yet
					MyParty.NextTwoNodes.Clear();
					MyParty.HasReachedDestNode = true;
					return Exit(BTResult.Fail);
				}
				return Exit(BTResult.Success);
			}
			else
			{
				MyParty.Destination = MyParty.NextNode.Location.RealPos;
				//Debug.Log("BTMAIGoToLocation: running, going to next node " + MyParty.NextNode.ID);
				return RunningNextNode(MyParty.NextNode.ID);
			}
		}
		else if(Parameters[0] == "DestCoord")
		{
			if(MyParty == null || MyParty.CurrentTask == null || MyParty.CurrentTask.TaskType != MacroAITaskType.Travel || MyParty.CurrentTask.IsDestAStation || MyParty.IsInTradelane || MyParty.DockedStationID != "")
			{
				return Exit(BTResult.Fail);
			}

			if(MyParty.NextNode != null && MyParty.NextNode != MyParty.DestNode)
			{
				return Exit(BTResult.Fail);
			}

			if(Vector3.Distance(MyParty.Location.RealPos, MyParty.CurrentTask.TravelDestCoord.RealPos) < 
				(Vector3.Distance(MyParty.Location.RealPos, MyParty.DestNode.Location.RealPos) + Vector3.Distance(MyParty.DestNode.Location.RealPos, MyParty.CurrentTask.TravelDestCoord.RealPos)))
			{
				MyParty.NextTwoNodes.Clear();
				MyParty.HasReachedDestNode = true;
			}
			else
			{
				return Exit(BTResult.Fail);
			}

			if(MyParty.HasReachedDestNode)
			{
				Vector3 destCoord = MyParty.CurrentTask.TravelDestCoord.RealPos;
				if(Vector3.Distance(MyParty.Location.RealPos, destCoord) < 30)
				{
					return Exit(BTResult.Success);
				}
				else
				{
					MyParty.Destination = destCoord;
					Debug.Log("BTMAIGoToLocation: running, going to dest coord");
					return RunningDestCoord(destCoord);
				}
			}
			else
			{
				return Exit(BTResult.Fail);
			}
		}


		return Exit(BTResult.Fail);
	}

	public override BTResult Exit (BTResult result)
	{
		Debug.Log("MAIBTGoToLocation: " + result + " party " + MyParty.PartyNumber);
		if(MyParty != null)
		{
			MyParty.Destination = Vector3.zero;
		}
		return result;
	}

	public override BTResult Running ()
	{
		MyParty.RunningNodeHist.UniquePush("MAI Go To Location");
		return BTResult.Running;
	}

	private BTResult RunningDestCoord(Vector3 coord)
	{
		MyParty.RunningNodeHist.UniquePush("Go To Location " + coord.ToString());
		return BTResult.Running;
	}

	private BTResult RunningNextNode(string nodeID)
	{
		MyParty.RunningNodeHist.UniquePush("Go To Location " + nodeID);
		return BTResult.Running;
	}
}
