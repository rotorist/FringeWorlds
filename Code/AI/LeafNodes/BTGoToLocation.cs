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
				return Exit(BTResult.Success);
			}
			else
			{
				MyAI.Whiteboard.Parameters["Destination"] = MyAI.MyParty.NextNode.Location;
				Debug.Log("BTGoToLocation: running");
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
