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
		if(MyParty.IsPlayerParty)
		{
			if(MyAI.MyShip == GameManager.Inst.PlayerControl.PlayerShip)
			{
				MyAI.OnTravelCompletion();
			}
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
		Debug.Log("BTGetNewTask: " + result);
		return result;
	}

	public override BTResult Running ()
	{
		MyAI.RunningNodeHist.UniquePush("Get New Task");
		return BTResult.Running;
	}
}
