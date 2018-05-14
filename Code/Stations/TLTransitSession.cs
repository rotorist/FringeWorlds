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
		Passengers.Add(leader);
		Direction = direction;
		Stage = TLSessionStage.None;
		CurrentTradelane = currentTradelane;

		if(leader == GameManager.Inst.PlayerControl.PlayerShip)
		{
			GameManager.Inst.PlayerControl.CurrentTradelaneSession = this;
		}
	}

	public void UpdateTransit()
	{
		if(Stage == TLSessionStage.Initializing)
		{
			
			if(Direction == -1)
			{
				CurrentTrigger = CurrentTradelane.TriggerA;
			}
			else
			{
				CurrentTrigger = CurrentTradelane.TriggerB;
			}

			if(!PassengerTargetRotations.ContainsKey(LeaderPassenger))
			{
				PassengerTargetRotations.Add(LeaderPassenger, Quaternion.LookRotation(CurrentTrigger.transform.up, Vector3.up));
			}
			if(!PassengerTargetPositions.ContainsKey(LeaderPassenger))
			{
				PassengerTargetPositions.Add(LeaderPassenger, CurrentTrigger.transform.position);
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
			LeaderPassenger.InPortalSpeed = _currentSpeed;
			//if 1/3 way in, unbusy the current lane
			float myDist = Vector3.Distance(LeaderPassenger.transform.position, NextTrigger.transform.position);

			if(myDist / totalDist < 0.66f && myDist / totalDist > 0.2f)
			{
				CurrentTradelane.ClearSession(Direction);
				((Tradelane)NextTrigger.ParentStation).AssignLiveSession(Direction, this);
			}
			//if 4/5 way in, go back to FindingDest
			else if(myDist / totalDist <= 0.2f)
			{
				CurrentTradelane = ((Tradelane)NextTrigger.ParentStation);
				Stage = TLSessionStage.FindingDest;
			}
		}
		else if(Stage == TLSessionStage.Exiting)
		{
			float acceleration = -40f;
			float slowAcceleration = -25f;
			Vector3 direction = (NextTrigger.transform.position - LeaderPassenger.transform.position).normalized;
			float distToTrigger = Vector3.Distance(NextTrigger.transform.position, LeaderPassenger.transform.position);

			direction = NextTrigger.transform.up;
			
			if(Vector3.Angle((LeaderPassenger.transform.position - NextTrigger.transform.position), NextTrigger.transform.up) < 90)
			{
				_currentSpeed = Mathf.Clamp(_currentSpeed + acceleration * Time.fixedDeltaTime, 0, 100);
			}
			else
			{
				_currentSpeed = Mathf.Clamp(_currentSpeed + slowAcceleration * Time.fixedDeltaTime, 0, 100);
			}
			LeaderPassenger.transform.position = LeaderPassenger.transform.position + direction * _currentSpeed * Time.fixedDeltaTime;
			LeaderPassenger.InPortalSpeed = _currentSpeed;

			if(_currentSpeed <= 0)
			{
				LeaderPassenger.InPortalSpeed = 0;
				LeaderPassenger.IsInPortal = false;
				CurrentTradelane.ClearSession(Direction);
				Stage = TLSessionStage.None;

				if(LeaderPassenger == GameManager.Inst.PlayerControl.PlayerShip)
				{
					GameManager.Inst.PlayerControl.CurrentTradelaneSession = null;
				}
			}
		}
		else if(Stage == TLSessionStage.Cancelling)
		{
			float acceleration = -40f;
			Vector3 direction = LeaderPassenger.transform.forward;
			_currentSpeed = Mathf.Clamp(_currentSpeed + acceleration * Time.fixedDeltaTime, 0, 100);
			LeaderPassenger.transform.position = LeaderPassenger.transform.position + direction * _currentSpeed * Time.fixedDeltaTime;
			LeaderPassenger.InPortalSpeed = _currentSpeed;

			if(_currentSpeed <= 0)
			{
				LeaderPassenger.InPortalSpeed = 0;
				LeaderPassenger.IsInPortal = false;
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
			}
		}


	}

	public void StartSession()
	{
		Debug.Log("Starting tradelane session");
		Stage = TLSessionStage.Initializing;
		LeaderPassenger.IsInPortal = true;
	}



	private bool CheckPassengersInPosition()
	{
		foreach(ShipBase passenger in Passengers)
		{
			float angle = Quaternion.Angle(passenger.transform.rotation, PassengerTargetRotations[passenger]);
			if(angle > 3)
			{
				return false;
			}
			if(Vector3.Distance(passenger.transform.position, PassengerTargetPositions[passenger]) > 0.2f)
			{
				return false;
			}
		}

		return true;
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