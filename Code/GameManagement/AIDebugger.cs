using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDebugger : MonoBehaviour 
{
	public bool Enabled;
	public ShipBase TargetNPC;
	public int TargetPartyNumber;

	public string TaskInfo;
	public List<string> NPCRunningNodesHist;

	public List<string> PartyRunningNodesHist;
	public List<string> AllParties;

	// Update is called once per frame
	void Update () 
	{
		if(!Enabled)
		{
			return;
		}

		//list all parties and their system
		AllParties = new List<string>();
		foreach(MacroAIParty party in GameManager.Inst.NPCManager.AllParties)
		{
			string entry = party.PartyNumber + " " + party.CurrentSystemID;
			AllParties.Add(entry);
		}

		if(TargetNPC != null)
		{
			NPCRunningNodesHist = new List<string>(TargetNPC.MyAI.RunningNodeHist.DeepPeek(6));
			if(TargetNPC.MyAI.MyParty != null)
			{
				PartyRunningNodesHist = new List<string>(TargetNPC.MyAI.MyParty.RunningNodeHist.DeepPeek(6));
			}

			if(TargetNPC.MyAI.MyParty != null && TargetNPC.MyAI.MyParty.CurrentTask != null)
			{
				MacroAITask task = TargetNPC.MyAI.MyParty.CurrentTask;
				TaskInfo = getTaskInfo(task);
			}


		}
		else if(TargetPartyNumber > 0)
		{
			foreach(MacroAIParty party in GameManager.Inst.NPCManager.AllParties)
			{
				if(party.PartyNumber == TargetPartyNumber)
				{
					PartyRunningNodesHist = new List<string>(party.RunningNodeHist.DeepPeek(6));
					if(party.CurrentTask != null)
					{
						TaskInfo = getTaskInfo(party.CurrentTask);
					}
				}
			}
		}

			
	}

	private string getTaskInfo(MacroAITask task)
	{
		string taskInfo = task.TaskType.ToString();
		if(task.TaskType == MacroAITaskType.Stay)
		{
			taskInfo += " for " + task.StayDuration;
		}
		else if(task.TaskType == MacroAITaskType.Travel)
		{
			taskInfo += " to " + task.TravelDestSystemID;
			if(task.IsDestAStation)
			{
				taskInfo += "\\" + task.TravelDestNodeID;
			}
			else
			{
				taskInfo += "\\" + task.TravelDestCoord.RealPos.ToString();
			}
		}

		return taskInfo;
	}
}
