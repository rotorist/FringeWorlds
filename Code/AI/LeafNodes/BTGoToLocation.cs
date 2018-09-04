using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTGoToLocation : BTLeaf
{

	public override void Initialize ()
	{

	}

	public override BTResult Process ()
	{
		if(MyAI.IsDocked)
		{
			return Exit(BTResult.Fail);
		}

		//Debug.Log("processing go to location, param " + Parameters[0]);
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

			if(Vector3.Distance(MyAI.MyShip.transform.position, MyParty.NextNode.Location.RealPos) < 30)
			{
				MyParty.PrevNode = MyParty.NextNode;
				if(!MyParty.CurrentTask.IsDestAStation && MyParty.DestNode.ID == MyParty.NextNode.ID)
				{
					//has reached dest node, but hasn't reached dest coord yet
					MyParty.HasReachedDestNode = true;
					MyParty.NextTwoNodes.Clear();
					return Exit(BTResult.Fail);
				}
				else
				{
					MyParty.PrevNode = MyParty.NextNode;
					MyParty.NextTwoNodes.Clear();
					return Exit(BTResult.Success);
				}
			}
			else
			{
				MyAI.Whiteboard.Parameters["Destination"] = MyParty.NextNode.Location.RealPos;
				//Debug.Log("BTGoToLocation: running, going to next node");
				return RunningNextNode(MyParty.NextNode.ID);
			}
		}
		else if(Parameters[0] == "DestCoord")
		{
			Debug.Log("curent task " + MyParty.CurrentTask.TaskType + MyParty.CurrentTask.IsDestAStation);
			if(MyParty == null || MyParty.CurrentTask == null || MyParty.CurrentTask.TaskType != MacroAITaskType.Travel 
				|| MyParty.CurrentTask.IsDestAStation || MyAI.MyShip.IsInPortal || MyAI.MyShip.DockedStationID != "")
			{
				return Exit(BTResult.Fail);
			}

			if(MyParty.NextNode != null && MyParty.NextNode != MyParty.DestNode)
			{
				return Exit(BTResult.Fail);
			}

			if(Vector3.Distance(MyParty.Location.RealPos, MyParty.CurrentTask.TravelDestCoord.RealPos) 
				< (Vector3.Distance(MyParty.Location.RealPos, MyParty.DestNode.Location.RealPos) + Vector3.Distance(MyParty.DestNode.Location.RealPos, MyParty.CurrentTask.TravelDestCoord.RealPos)))
			{
				MyParty.HasReachedDestNode = true;
				MyParty.NextTwoNodes.Clear();
			}
			else
			{
				return Exit(BTResult.Fail);
			}

			if(MyParty.HasReachedDestNode)
			{
				Vector3 destCoord = MyParty.CurrentTask.TravelDestCoord.RealPos;
				if(Vector3.Distance(MyAI.MyShip.transform.position, destCoord) < 30)
				{
					Debug.LogError("Completed goto location DestCoord");

					return Exit(BTResult.Success);
				}
				else
				{
					MyAI.Whiteboard.Parameters["Destination"] = destCoord;
					Debug.Log("BTGoToLocation: running, going to dest coord");
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
		Debug.Log("BTGoToLocation: " + result);
		return result;
	}

	public override BTResult Running ()
	{
		MyAI.RunningNodeHist.UniquePush("Go To Location");
		return BTResult.Running;
	}

	private BTResult RunningDestCoord(Vector3 coord)
	{
		MyAI.RunningNodeHist.UniquePush("Go To Location " + coord.ToString());
		return BTResult.Running;
	}

	private BTResult RunningNextNode(string nodeID)
	{
		MyAI.RunningNodeHist.UniquePush("Go To Location " + nodeID);
		return BTResult.Running;
	}
}
