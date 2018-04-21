using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tradelane : StationBase
{
	public int IsTerminalAorB;//0 = not terminal, -1 = A, 1 = B
	public DockingTrigger TriggerA;
	public DockingTrigger TriggerB;
	public TLDockingEffect DockingEffectA;
	public TLDockingEffect DockingEffectB;
	public Renderer LaneARightDirLight;
	public Renderer LaneAWrongDirLight;
	public Renderer LaneBRightDirLight;
	public Renderer LaneBWrongDirLight;

	public Tradelane NeighborToA;
	public Tradelane NeighborToB;
	public string NeighborAID;
	public string NeighborBID;

	private int _dockingStageA;//0 docking not initiated; 1 only ring; 2 all ready; 3 transporting
	private int _dockingStageB;
	private float _dockingTimerA;
	private float _dockingTimerB;

	private TLTransitSession _sessionA;
	private TLTransitSession _sessionB;


	void FixedUpdate()
	{
		if(_dockingStageA == 1)
		{
			_dockingTimerA += Time.fixedDeltaTime;
			if(_dockingTimerA > 2)
			{
				_dockingStageA = 2;
				DockingEffectA.SetStage(2);
			}
		}

		if(_dockingStageB == 1)
		{
			_dockingTimerB += Time.fixedDeltaTime;
			if(_dockingTimerB > 2)
			{
				_dockingStageB = 2;
				DockingEffectB.SetStage(2);
			}
		}

		if(_sessionA != null)
		{
			_sessionA.UpdateTransit();
		}

		if(_sessionB != null)
		{
			_sessionB.UpdateTransit();
		}
	}

	public override void Initialize ()
	{
		DockingEffectA.SetStage(0);
		DockingEffectB.SetStage(0);
		SetNormalLightStates();
	}

	public override DockRequestResult Dock (ShipBase requester)
	{
		//determine direction from requester's location
		float angleA = Vector3.Angle(TriggerA.transform.up, TriggerA.transform.position - requester.transform.position);
		float angleB = Vector3.Angle(TriggerB.transform.up, TriggerB.transform.position - requester.transform.position);



		if(angleA < 90 && IsTerminalAorB >= 0)
		{
			Debug.Log("Docking requested to go to A");
			//direction is going towards A
			//check if anyone is already docking A
			if(_dockingStageA > 0 || _sessionA != null)
			{
				return DockRequestResult.Busy;
			}
			DockingEffectA.SetStage(1);
			_dockingStageA = 1;
			_sessionA = new TLTransitSession(requester, -1, this);
			return DockRequestResult.Accept;

		}
		else if(angleB < 90 && IsTerminalAorB <= 0)
		{
			Debug.Log("Docking requested to go to B");
			//directoin is going towards B
			if(_dockingStageB > 0 || _sessionB != null)
			{
				return DockRequestResult.Busy;
			}
			DockingEffectB.SetStage(1);
			_dockingStageB = 1;
			_sessionB = new TLTransitSession(requester, 1, this);
			return DockRequestResult.Accept;
		}

		return DockRequestResult.Deny;
	}

	public override void OnDetectDocking (string triggerID, ShipBase requester)
	{
		if(_sessionA != null && _sessionA.LeaderPassenger == requester && _dockingStageA == 2)
		{
			//start session A
			_sessionA.StartSession();
		}
		else if(_sessionB != null && _sessionB.LeaderPassenger == requester && _dockingStageB == 2)
		{
			//start session B
			_sessionB.StartSession();
		}
	}


	public void ClearSession(int direction)
	{
		if(direction < 0)
		{
			_sessionA = null;
			DockingEffectA.SetStage(0);
			_dockingStageA = 0;
			_dockingTimerA = 0;
		}
		else
		{
			_sessionB = null;
			DockingEffectB.SetStage(0);
			_dockingStageB = 0;
			_dockingTimerB = 0;
		}
	}

	public void AssignLiveSession(int direction, TLTransitSession session)
	{
		if(direction < 0)
		{
			_sessionA = session;
			DockingEffectA.SetStage(1);
			_dockingStageA = 3;
			_dockingTimerA = 0;
		}
		else
		{
			_sessionB = session;
			DockingEffectB.SetStage(1);
			_dockingStageB = 3;
			_dockingTimerB = 0;
		}
	}


	private void SetNormalLightStates()
	{
		LaneAWrongDirLight.sharedMaterial = GameManager.Inst.MaterialManager.TradelaneLightRed;
		LaneBWrongDirLight.sharedMaterial = GameManager.Inst.MaterialManager.TradelaneLightRed;
			
		if(IsTerminalAorB == -1)
		{
			LaneARightDirLight.sharedMaterial = GameManager.Inst.MaterialManager.TradelaneLightGreen;
			LaneBRightDirLight.sharedMaterial = GameManager.Inst.MaterialManager.TradelaneLightRed;
		}
		else if(IsTerminalAorB == 1)
		{
			LaneARightDirLight.sharedMaterial = GameManager.Inst.MaterialManager.TradelaneLightRed;
			LaneBRightDirLight.sharedMaterial = GameManager.Inst.MaterialManager.TradelaneLightGreen;
		}
		else
		{
			LaneARightDirLight.sharedMaterial = GameManager.Inst.MaterialManager.TradelaneLightGreen;
			LaneBRightDirLight.sharedMaterial = GameManager.Inst.MaterialManager.TradelaneLightGreen;
		}
	}

	private void SetShutdownLightStates()
	{
		LaneAWrongDirLight.sharedMaterial = GameManager.Inst.MaterialManager.TradelaneLightDown;
		LaneBWrongDirLight.sharedMaterial = GameManager.Inst.MaterialManager.TradelaneLightDown;
		LaneARightDirLight.sharedMaterial = GameManager.Inst.MaterialManager.TradelaneLightDown;
		LaneBRightDirLight.sharedMaterial = GameManager.Inst.MaterialManager.TradelaneLightDown;
	}
}


public class TradelaneData : NavNode
{
	public string DisplayName;
	public string NeighborAID;
	public string NeighborBID;
	public int IsTerminalAorB;
	public Vector3 EulerAngles;
}