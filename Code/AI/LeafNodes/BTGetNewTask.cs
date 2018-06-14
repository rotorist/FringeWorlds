using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTGetNewTask : BTLeaf
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

		MacroAITaskType prevType = MacroAITaskType.None;
		if(MyParty.CurrentTask != null)
		{
			prevType = MyParty.CurrentTask.TaskType;
		}
		GameManager.Inst.NPCManager.MacroAI.AssignMacroAITask(prevType, MyParty);


		return Exit(BTResult.Success);
	}

	public override BTResult Exit (BTResult result)
	{
		Debug.Log("BTGetDestination: " + result);
		return result;
	}
}
