using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTHoldPosition : BTLeaf
{

	public override void Initialize ()
	{

	}

	public override BTResult Process ()
	{
		Vector3 velocity = MyAI.RB.velocity;
		MyAI.Whiteboard.Parameters["Destination"] = Vector3.zero;
		if(velocity.magnitude > 0.05f)
		{
			MyAI.RB.AddForce(-1 * velocity);
			return BTResult.Running;
		}
		else
		{
			return Exit(BTResult.Success);
		}
	}

	public override BTResult Exit (BTResult result)
	{
		return result;
	}

}
