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
		if(MyParty == null)
		{
			return Exit(BTResult.Fail);
		}

		if(MyParty.CurrentTask != null)
		{
			//MyParty.NextNode = GameManager.Inst.NPCManager.MacroAI.FindNextNavNode(MyParty.n
			//GameManager.Inst.NPCManager.MacroAI.AssignMacroAITask(MacroAITaskType.None, MyParty);
			if(MyParty.PrevNode == null && MyParty.NextNode == null)
			{
				List<NavNode> nextTwoNodes = new List<NavNode>();
				NavNode nextNode = GameManager.Inst.NPCManager.MacroAI.GetClosestNodeToLocation(MyParty.Location.RealPos, GameManager.Inst.WorldManager.AllSystems[MyParty.CurrentSystemID]);
				List<NavNode> nextNextTwoNodes = GameManager.Inst.NPCManager.MacroAI.FindNextNavNode(nextNode, MyParty.DestNode);
				nextTwoNodes.Add(nextNode);
				if(nextNextTwoNodes.Count > 0)
				{
					nextTwoNodes.Add(nextNextTwoNodes[0]);
				}
				MyParty.NextTwoNodes = nextTwoNodes;
			}
			else if(MyParty.PrevNode != null)
			{
				if(!MyParty.HasReachedDestNode)
				{
					MyParty.NextTwoNodes = GameManager.Inst.NPCManager.MacroAI.FindNextNavNode(MyParty.PrevNode, MyParty.DestNode);
				}
				else
				{
					MyParty.NextTwoNodes.Clear();
					return BTResult.Success;
				}
			}

			if(MyParty.NextNode != null && MyParty.PrevNode != null && MyParty.NextNode.NavNodeType == NavNodeType.Tradelane && MyParty.PrevNode.NavNodeType == NavNodeType.Tradelane)
			{
				MyParty.IsInTradelane = true;
			}
			else
			{
				MyParty.IsInTradelane = false;
			}

			if(MyParty.NextNode != null)
			{
				Debug.LogError("BTGetNextNode: " + MyParty.NextNode.ID + " for party " + MyParty.PartyNumber);
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
		Debug.Log("BTGetNextNode: " + result + " for party " + MyParty.PartyNumber);
		return result;
	}

	public override BTResult Running ()
	{
		MyAI.RunningNodeHist.UniquePush("Get Next Node");
		return BTResult.Running;
	}
}
