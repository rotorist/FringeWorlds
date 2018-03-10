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
		ShipBase target = (ShipBase)MyAI.Whiteboard.Parameters[Parameters[0]];
		if(target != null)
		{
			float dist = Vector3.Distance(MyAI.MyShip.transform.position, target.transform.position);
			if(dist <= (float)MyAI.Whiteboard.Parameters["FriendlyFollowDist"])
			{
				return BTResult.Success;
			}
			else
			{
				MyAI.Whiteboard.Parameters["Destination"] = target.transform.position;
				return BTResult.Running;
			}
		}
		else
		{
			return Exit(BTResult.Fail);
		}

	}

	public override BTResult Exit (BTResult result)
	{
		return result;
	}
}
