using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTGetNextNode : BTLeaf
{

	public override void Initialize ()
	{

	}

	public override BTResult Process ()
	{
		if(MyAI.MyParty == null)
		{
			return Exit(BTResult.Fail);
		}

		if(MyAI.MyParty.CurrentTask != null)
		{
			//MyAI.MyParty.NextNode = GameManager.Inst.NPCManager.MacroAI.FindNextNavNode(MyAI.MyParty.n
			//GameManager.Inst.NPCManager.MacroAI.AssignMacroAITask(MacroAITaskType.None, MyAI.MyParty);
			if(MyAI.MyParty.PrevNode == null && MyAI.MyParty.NextNode == null)
			{
				MyAI.MyParty.NextNode = GameManager.Inst.NPCManager.MacroAI.GetClosestNodeToLocation(MyAI.MyParty.Location, GameManager.Inst.WorldManager.AllSystems[MyAI.MyParty.CurrentSystemID]);

			}
			else if(MyAI.MyParty.PrevNode != null)
			{
				MyAI.MyParty.NextNode = GameManager.Inst.NPCManager.MacroAI.FindNextNavNode(MyAI.MyParty.PrevNode, MyAI.MyParty.DestNode);

			}
			Debug.Log("BTGetNextNode: " + MyAI.MyParty.NextNode.ID);
			return Exit(BTResult.Success);
		}
		else
		{
			return BTResult.Fail;
		}

		return BTResult.Fail;
	}

	public override BTResult Exit (BTResult result)
	{
		Debug.Log("BTGetNextNode: " + result);
		return result;
	}
}
