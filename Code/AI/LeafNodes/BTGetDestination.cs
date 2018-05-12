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
		if(MyAI.MyParty == null)
		{
			return BTResult.Fail;
		}

		if(MyAI.MyParty.CurrentTask == null)
		{
			GameManager.Inst.NPCManager.MacroAI.AssignMacroAITask(MacroAITaskType.None, MyAI.MyParty);
		}

		return BTResult.Success;
	}

	public override BTResult Exit (BTResult result)
	{
		return result;
	}
}
