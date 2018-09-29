using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTDropCountermeasure : BTLeaf
{


	public override void Initialize ()
	{
		
	}

	public override BTResult Process ()
	{
		//check if missile is present and is close
		GameObject closeMissile = null;
		foreach(GameObject missile in MyAI.MyShip.IncomingMissiles)
		{
			if(missile != null)
			{
				if(Vector3.Distance(MyAI.MyShip.transform.position, missile.transform.position) < 30)
				{
					closeMissile = missile;
				}
			}
		}


		GameObject currentCM = MyAI.MyShip.CurrentCountermeasure;

		if(currentCM == null && closeMissile != null && closeMissile.GetComponent<Missile>().Stage == MissileStage.Chasing)
		{
			
			GameObject cm = GameObject.Instantiate(Resources.Load("CountermeasureEffect")) as GameObject;
			cm.transform.parent = MyAI.MyShip.transform;
			cm.transform.localPosition = Vector3.zero;
			cm.transform.localEulerAngles = Vector3.zero;
			cm.transform.localScale = new Vector3(1, 1, 1);
			cm.transform.parent = null;
			MyAI.MyShip.CurrentCountermeasure = cm;

			return Exit(BTResult.Success);
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

	public override BTResult Running ()
	{
		MyAI.RunningNodeHist.UniquePush("Fighter Attack");
		return BTResult.Running;
	}

}
