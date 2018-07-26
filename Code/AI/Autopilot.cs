using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autopilot : AI 
{
	public override void Initialize (MacroAIParty party, Faction faction)
	{
		MyShip = transform.GetComponent<ShipBase>();
		AvoidanceDetector = MyShip.MyReference.AvoidanceDetector;
		AvoidanceDetector.ParentShip = MyShip;

		Whiteboard = new Whiteboard();
		Whiteboard.Initialize();

		//AttackTarget = GameManager.Inst.PlayerControl.PlayerShip;

		//Whiteboard.Parameters["TargetEnemy"] = AttackTarget;
		Whiteboard.Parameters["SpeedLimit"] = -1f;

		MyParty = party;
		MyFaction = faction;

		TreeSet = new Dictionary<string, BehaviorTree>();
		TreeSet.Add("BaseBehavior", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("BaseBehavior", this, party));
		TreeSet.Add("APTravel", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("APTravel", this, party));
		TreeSet.Add("FollowFriendly", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("FollowFriendly", this, party));
	}

	public override void OnTravelCompletion ()
	{
		GameManager.Inst.PlayerControl.CancelAutopilot();
	}

	public override void Update ()
	{
		
	}

	public override void FixedUpdate ()
	{
		
	}

	public void APUpdate ()
	{
		MyParty.Location.RealPos = MyParty.SpawnedShipsLeader.transform.position;

		if(IsActive)
		{

			TreeSet["APTravel"].Run();

			if(!IsDocked)
			{
				Turn();

				UpdateSensor();
				AvoidanceDetector.State = AvoidanceState.Travel;

			}
		}
	}

	public void APFixedUpdate ()
	{
		if(IsActive)
		{
			if(!IsDocked)
			{
				Move();

				UpdateAvoidance();
			}
		}
	}

}
