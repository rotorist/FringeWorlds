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
				List<NavNode> nextTwoNodes = new List<NavNode>();
				NavNode nextNode = GameManager.Inst.NPCManager.MacroAI.GetClosestNodeToLocation(MyAI.MyParty.Location, GameManager.Inst.WorldManager.AllSystems[MyAI.MyParty.CurrentSystemID]);
				List<NavNode> nextNextTwoNodes = GameManager.Inst.NPCManager.MacroAI.FindNextNavNode(nextNode, MyAI.MyParty.DestNode);
				nextTwoNodes.Add(nextNode);
				if(nextNextTwoNodes.Count > 0)
				{
					nextTwoNodes.Add(nextNextTwoNodes[0]);
				}
				MyAI.MyParty.NextTwoNodes = nextTwoNodes;
			}
			else if(MyAI.MyParty.PrevNode != null)
			{
				MyAI.MyParty.NextTwoNodes = GameManager.Inst.NPCManager.MacroAI.FindNextNavNode(MyAI.MyParty.PrevNode, MyAI.MyParty.DestNode);

			}
			if(MyAI.MyParty.NextNode != null)
			{
				Debug.Log("BTGetNextNode: " + MyAI.MyParty.NextNode.ID);
			}
			else
			{
				Debug.Log("BTGetNextNode: Can't find next node");
			}
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
