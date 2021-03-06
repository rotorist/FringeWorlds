﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTChaseEnemy : BTLeaf
{

	public override void Initialize ()
	{
		//Debug.Log("Initializing Chase Enemy");
	}

	public override BTResult Process ()
	{
		//Debug.Log("Processing Chase Enemy " + MyAI.name);

		ShipBase target = (ShipBase)MyAI.Whiteboard.Parameters[Parameters[0]];
		if(target != null)
		{
			float dist = Vector3.Distance(MyAI.MyShip.transform.position, target.transform.position);
			float angle = Vector3.Angle(target.transform.position - MyAI.MyShip.transform.position, MyAI.MyShip.transform.forward);
			if(dist <= 20)
			{
				return Exit(BTResult.Success);
			}
			if(dist <= (float)MyAI.Whiteboard.Parameters["FiringRange"] && angle < 20)
			{
				return Exit(BTResult.Success);
			}
			else
			{
				MyAI.Whiteboard.Parameters["Destination"] = target.transform.position;
				//Vector3 aimPoint = StaticUtility.FirstOrderIntercept(MyAI.MyShip.transform.position, MyAI.MyShip.RB.velocity,
				//													30, target.transform.position, target.RB.velocity);
				MyAI.Whiteboard.Parameters["AimPoint"] = Vector3.zero;

				return Running();
			}
		}
		else
		{
			return Exit(BTResult.Fail);
		}

	}

	public override BTResult Exit (BTResult result)
	{
		//Debug.Log("Chase enemy result " + result);
		return result;
	}

	public override BTResult Running ()
	{
		MyAI.RunningNodeHist.UniquePush("Chase Enemy");
		return BTResult.Running;
	}

}
