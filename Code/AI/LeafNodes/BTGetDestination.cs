using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTGetDestination : BTLeaf
{

	public override void Initialize ()
	{
		
	}

	public override BTResult Process ()
	{
		if(MyParty == null)
		{
			return Exit(BTResult.Fail);
		}

		if(MyParty.CurrentTask != null)
		{
			if(MyParty.CurrentTask.TaskType == MacroAITaskType.Travel)
			{
				return Exit(BTResult.Success);
			}
			else
			{
				return Exit(BTResult.Fail);
			}
		}
		else
		{
			return Exit(BTResult.Fail);
		}

		return Exit(BTResult.Success);
	}

	public override BTResult Exit (BTResult result)
	{
		if(result != BTResult.Fail)
			Debug.Log("BTGetDestination: " + result);
		return result;
	}

	public override BTResult Running ()
	{
		MyAI.RunningNodeHist.UniquePush("Get Destination");
		return BTResult.Running;
	}
}
