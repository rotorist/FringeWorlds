using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTFail : BTLeaf
{

	public override void Initialize ()
	{
		
	}

	public override BTResult Process ()
	{
		//Debug.Log("Processing Fail action");
		return BTResult.Fail;
	}

	public override BTResult Exit (BTResult result)
	{
		return result;	
	}

	public override BTResult Running ()
	{
		MyAI.RunningNodeHist.UniquePush("Fail Action");
		return BTResult.Running;
	}
}
