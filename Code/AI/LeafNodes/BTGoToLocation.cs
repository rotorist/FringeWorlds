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
			if(MyAI.MyParty == null || MyAI.MyParty.NextNode == null)
			{
				return Exit(BTResult.Fail);
			}

			if(Vector3.Distance(MyAI.MyShip.transform.position, MyAI.MyParty.NextNode.Location) < 30)
			{
				MyAI.MyParty.PrevNode = MyAI.MyParty.NextNode;
				if(!MyAI.MyParty.CurrentTask.IsDestAStation && MyAI.MyParty.DestNode.ID == MyAI.MyParty.NextNode.ID)
				{
					//has reached dest node, but hasn't reached dest coord yet
					MyAI.MyParty.HasReachedDestNode = true;
					return Exit(BTResult.Fail);
				}
				return Exit(BTResult.Success);
			}
			else
			{
				MyAI.Whiteboard.Parameters["Destination"] = MyAI.MyParty.NextNode.Location;
				Debug.Log("BTGoToLocation: running, going to next node");
				return BTResult.Running;
			}
		}
		else if(Parameters[0] == "DestCoord")
		{
			if(MyAI.MyParty == null || MyAI.MyParty.CurrentTask == null || MyAI.MyParty.CurrentTask.TaskType != MacroAITaskType.Travel || MyAI.MyParty.CurrentTask.IsDestAStation)
			{
				return Exit(BTResult.Fail);
			}

			Vector3 destCoord = MyAI.MyParty.CurrentTask.TravelDestCoord;
			if(Vector3.Distance(MyAI.MyShip.transform.position, destCoord) < 10)
			{
				return Exit(BTResult.Success);
			}
			else
			{
				MyAI.Whiteboard.Parameters["Destination"] = destCoord;
				Debug.Log("BTGoToLocation: running, going to dest coord");
				return BTResult.Running;
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
