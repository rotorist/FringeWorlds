using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLTransitSession : DockSessionBase
{
	public TLSessionStage Stage;
	public int Direction; //-1 = towards A, 1 = towards B
	public Tradelane CurrentTradelane;
	public DockingTrigger NextTrigger;
	public DockingTrigger CurrentTrigger;
	public List<ShipBase> Passengers;
	public Dictionary<ShipBase, Quaternion> PassengerTargetRotations;
	public Dictionary<ShipBase, Vector3> PassengerTargetPositions;
	public ShipBase LeaderPassenger;

	private float _currentSpeed;

	public TLTransitSession(ShipBase leader, int direction, Tradelane currentTradelane)
	{
		Passengers = new List<ShipBase>();
		PassengerTargetRotations = new Dictionary<ShipBase, Quaternion>();
		PassengerTargetPositions = new Dictionary<ShipBase, Vector3>();
		LeaderPassenger = leader;
		if(leader.MyAI != null && leader.MyAI.MyParty != null)
		{
			foreach(ShipBase ship in leader.MyAI.MyParty.SpawnedShips)
			{
				Passengers.Add(ship);
			}
		}
		else
		{
			Passengers.Add(leader);
		}

		Direction = direction;
		Stage = TLSessionStage.None;
		CurrentTradelane = currentTradelane;

		if(Direction == -1)
		{
			CurrentTrigger = CurrentTradelane.TriggerA;
		}
		else
		{
			CurrentTrigger = CurrentTradelane.TriggerB;
		}

		if(leader == GameManager.Inst.PlayerControl.PlayerShip)
		{
			GameManager.Inst.PlayerControl.CurrentTradelaneSession = this;
		}
	}

	public void UpdateTransit()
	{
		//Debug.Log("TLTransit stage " + Stage + " parent lane " + CurrentTradelane.ID);
		if(Stage == TLSessionStage.Initializing)
		{
			foreach(ShipBase ship in Passengers)
			{
				if(!PassengerTargetRotations.ContainsKey(ship))
				{
					PassengerTargetRotations.Add(ship, Quaternion.LookRotation(CurrentTrigger.transform.forward, Vector3.up));
				}
				if(!PassengerTargetPositions.ContainsKey(ship))
				{
					if(ship == LeaderPassenger)
					{
						PassengerTargetPositions.Add(LeaderPassenger, CurrentTrigger.transform.position);
					}
					else
					{
						Vector3 pos = CurrentTrigger.transform.TransformPoint(ship.MyAI.MyParty.Formation[ship]);
						Debug.LogError(pos);
						PassengerTargetPositions.Add(ship, pos);
					}
				}
			}

			Stage = TLSessionStage.Entering;
		}
		else if(Stage == TLSessionStage.Entering)
		{
			//move leader to current lane's detector's position
			LeaderPassenger.transform.position = Vector3.Lerp(LeaderPassenger.transform.position, PassengerTargetPositions[LeaderPassenger], Time.fixedDeltaTime * 1);
			//make leader look towards trigger's up
			LeaderPassenger.transform.rotation = Quaternion.Lerp(LeaderPassenger.transform.rotation, PassengerTargetRotations[LeaderPassenger], Time.fixedDeltaTime * 1);



			//check if all passengers are in place
			if(CheckPassengersInPosition())
			{
				foreach(ShipBase passenger in Passengers)
				{
					passenger.IsInPortal = true;
				}
				Stage = TLSessionStage.FindingDest;
			}
		}
		else if(Stage == TLSessionStage.FindingDest)
		{
			if(Direction == -1)
			{
				if(CurrentTradelane.NeighborToA != null)
				{
					NextTrigger = CurrentTradelane.NeighborToA.TriggerA;
					Stage = TLSessionStage.Sending;
				}
				else
				{
					//this is the terminal
					NextTrigger = CurrentTradelane.TriggerA;
					Stage = TLSessionStage.Exiting;

				}

			}
			else
			{
				if(CurrentTradelane.NeighborToB != null)
				{
					NextTrigger = CurrentTradelane.NeighborToB.TriggerB;
					Stage = TLSessionStage.Sending;

				}
				else
				{
					Debug.Log("exiting!");
					//this is the terminal
					NextTrigger = CurrentTradelane.TriggerB;
					Stage = TLSessionStage.Exiting;
				}
			}


		}
		else if(Stage == TLSessionStage.Sending)
		{
			float topSpeed = 90f;
			float acceleration = 20f;
			_currentSpeed = Mathf.Clamp(_currentSpeed + acceleration * Time.fixedDeltaTime, 0, topSpeed);

			float totalDist = Vector3.Distance(NextTrigger.ParentStation.transform.position, CurrentTradelane.transform.position);
			Vector3 direction = (NextTrigger.transform.position - LeaderPassenger.transform.position).normalized;
			LeaderPassenger.transform.position = LeaderPassenger.transform.position + direction * _currentSpeed * Time.fixedDeltaTime;
			LeaderPassenger.transform.LookAt(NextTrigger.transform);
			UpdatePassengersPositionRotation();
			LeaderPassenger.InPortalSpeed = _currentSpeed;
			//unbusy the current lane
			float myDist = Vector3.Distance(LeaderPassenger.transform.position, NextTrigger.transform.position);

			if(myDist / totalDist < 0.66f && myDist / totalDist > 0.2f)
			{
				if(CurrentTradelane.IsSessionOnMe(this))
				{
					CurrentTradelane.ClearSession(Direction);
				}
				((Tradelane)NextTrigger.ParentStation).AssignLiveSession(Direction, this);
			}
			//if 4/5 way in, go back to FindingDest
			if(myDist / totalDist <= 0.2f)
			{
				CurrentTradelane = ((Tradelane)NextTrigger.ParentStation);
				Stage = TLSessionStage.FindingDest;
			}
		}
		else if(Stage == TLSessionStage.Exiting)
		{
			float acceleration = -40f;
			float slowAcceleration = -25f;
			Vector3 direction = (NextTrigger.transform.position - LeaderPassenger.transform.position);
			//float distToTrigger = Vector3.Distance(NextTrigger.transform.position, LeaderPassenger.transform.position);

			if(direction.magnitude < 50)
			{
				direction = NextTrigger.transform.forward;
			}
			
			if(Vector3.Angle((LeaderPassenger.transform.position - NextTrigger.transform.position), NextTrigger.transform.forward) < 90)
			{
				_currentSpeed = Mathf.Clamp(_currentSpeed + acceleration * Time.fixedDeltaTime, 0, 100);
			}
			else
			{
				_currentSpeed = Mathf.Clamp(_currentSpeed + slowAcceleration * Time.fixedDeltaTime, 20, 100);
			}

			LeaderPassenger.transform.position = LeaderPassenger.transform.position + direction.normalized * _currentSpeed * Time.fixedDeltaTime;
			UpdatePassengersPositionRotation();
			LeaderPassenger.InPortalSpeed = _currentSpeed;

			if(_currentSpeed <= 0)
			{
				LeaderPassenger.InPortalSpeed = 0;
				LeaderPassenger.IsInPortal = false;
				foreach(ShipBase passenger in Passengers)
				{
					passenger.IsInPortal = false;
				}
				CurrentTradelane.ClearSession(Direction);
				Stage = TLSessionStage.None;

				if(LeaderPassenger == GameManager.Inst.PlayerControl.PlayerShip)
				{
					GameManager.Inst.PlayerControl.CurrentTradelaneSession = null;
				}
				else
				{
					LeaderPassenger.MyAI.MyParty.CurrentTLSession = null;
				}
			}
		}
		else if(Stage == TLSessionStage.Cancelling)
		{
			float acceleration = -60f;
			Vector3 direction = (NextTrigger.transform.position - LeaderPassenger.transform.position);
			if(Vector3.Angle((LeaderPassenger.transform.position - NextTrigger.transform.position), NextTrigger.transform.forward) < 90)
			{
				direction = NextTrigger.transform.forward;
			}

			_currentSpeed = Mathf.Clamp(_currentSpeed + acceleration * Time.fixedDeltaTime, 0, 100);
			LeaderPassenger.transform.position = LeaderPassenger.transform.position + direction.normalized * _currentSpeed * Time.fixedDeltaTime;
			UpdatePassengersPositionRotation();
			LeaderPassenger.InPortalSpeed = _currentSpeed;
			//Debug.LogError(_currentSpeed);
			if(_currentSpeed <= 0)
			{
				LeaderPassenger.InPortalSpeed = 0;
				LeaderPassenger.IsInPortal = false;
				foreach(ShipBase passenger in Passengers)
				{
					passenger.IsInPortal = false;
				}
				CurrentTradelane.ClearSession(Direction);
				if(NextTrigger != null)
				{
					((Tradelane)NextTrigger.ParentStation).ClearSession(Direction);
				}
				Stage = TLSessionStage.None;

				if(LeaderPassenger == GameManager.Inst.PlayerControl.PlayerShip)
				{
					GameManager.Inst.PlayerControl.CurrentTradelaneSession = null;
				}
				else
				{
					LeaderPassenger.MyAI.MyParty.CurrentTLSession = null;
				}
			}
		}


	}

	public void StartSession()
	{
		Debug.Log("Starting tradelane session " + CurrentTradelane.ID);
		Stage = TLSessionStage.Initializing;
		LeaderPassenger.IsInPortal = true;

	}

	public void StartMidwaySession()
	{
		if(Direction == -1)
		{
			NextTrigger = CurrentTradelane.NeighborToA.TriggerA;

		}
		else
		{
			NextTrigger = CurrentTradelane.NeighborToB.TriggerB;
		}
		Debug.Log("Starting tradelane session midway" + CurrentTradelane.ID);
		CurrentTradelane.ClearSession(Direction);
		((Tradelane)NextTrigger.ParentStation).AssignLiveSession(Direction, this);
		Stage = TLSessionStage.Sending;
		LeaderPassenger.IsInPortal = true;
	}



	private bool CheckPassengersInPosition()
	{
		float angle = Quaternion.Angle(LeaderPassenger.transform.rotation, PassengerTargetRotations[LeaderPassenger]);
		if(angle > 3)
		{
			return false;
		}
		foreach(ShipBase passenger in Passengers)
		{
			float dist = Vector3.Distance(passenger.transform.position, PassengerTargetPositions[passenger]);
			Debug.Log(dist + " " + passenger.name);
			if(dist > 3f)
			{
				return false;
			}
		}

		return true;
	}

	private void UpdatePassengersPositionRotation()
	{
		foreach(ShipBase passenger in Passengers)
		{
			if(passenger != LeaderPassenger)
			{
				passenger.transform.position = LeaderPassenger.transform.TransformPoint(LeaderPassenger.MyAI.MyParty.Formation[passenger]);
				passenger.transform.rotation = LeaderPassenger.transform.rotation;
			}
		}
	}
}

public enum TLSessionStage
{
	None,
	Initializing,
	Entering,
	FindingDest,
	Sending,
	Exiting,
	Cancelling,
}