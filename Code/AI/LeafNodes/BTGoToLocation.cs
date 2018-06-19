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
				return BTResult.Running;
			}
		}
		else if(Parameters[0] == "DestCoord")
		{
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
					return BTResult.Running;
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
}
