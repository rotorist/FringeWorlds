using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingSession
{
	public DockingSessionStage Stage;
	public DockGate Gate;
	public ShipBase Requester;
	public StationBase ParentStation;

	private float _currentSpeed;
	private Vector3 _dockEnterTarget;

	public DockingSession(DockGate gate, ShipBase requester, StationBase parent)
	{
		Gate = gate;
		Requester = requester;
		ParentStation = parent;
		Stage = DockingSessionStage.Granted;

		//if requester is player, turn light green
		if(requester == GameManager.Inst.PlayerControl.PlayerShip)
		{
			gate.SetGreenLight();
			_dockEnterTarget = Gate.DockingTrigger.transform.position + Gate.DockingTrigger.transform.forward * 2;
		}
		else
		{
			gate.SetRedLight();
			_dockEnterTarget = Gate.DockingTrigger.transform.position + Gate.DockingTrigger.transform.forward * 20;
		}

		gate.Open();
		gate.DockingTrigger.isTrigger = false;
	}

	public void UpdateDockingSession()
	{
		if(Stage == DockingSessionStage.Granted)
		{
			if(Gate.IsDone)
			{
				//if is player then set docking trigger to trigger
				if(Requester == GameManager.Inst.PlayerControl.PlayerShip)
				{
					Gate.DockingTrigger.isTrigger = true;
				}

				GameObject.Find("Sphere").transform.position = _dockEnterTarget;

				float dist = Vector3.Distance(Requester.transform.position, _dockEnterTarget);
				if(dist < 3)
				{
					Stage = DockingSessionStage.Docking;
					Requester.IsInPortal = true;
					if(Requester == GameManager.Inst.PlayerControl.PlayerShip)
					{
						//trigger begin docking
						UIEventHandler.Instance.TriggerBeginDocking();
					}
				}
			}
		}

		if(Stage == DockingSessionStage.Docking)
		{
			
			//move requester to current lane's detector's position
			Requester.transform.position = Vector3.Lerp(Requester.transform.position, _dockEnterTarget, Time.fixedDeltaTime * 1);
			//make requester look towards trigger's reverse forward
			Quaternion lookRot = Quaternion.LookRotation(Gate.DockingTrigger.transform.forward * -1, Vector3.up);
			Requester.transform.rotation = Quaternion.Lerp(Requester.transform.rotation, lookRot, Time.fixedDeltaTime * 1);

			if(CheckRequesterInPosition())
			{
				Stage = DockingSessionStage.Entering;
			}
		}

		if(Stage == DockingSessionStage.Entering)
		{
			Vector3 enterTarget = Gate.DockingTrigger.transform.position - Gate.DockingTrigger.transform.forward * 4f;
			float totalDist = Vector3.Distance(enterTarget, Requester.transform.position);
			float topSpeed = 4f;
			float acceleration = 1f;
			if(totalDist < 3f)
			{
				acceleration = -0.6f;
			}

			_currentSpeed = Mathf.Clamp(_currentSpeed + acceleration * Time.fixedDeltaTime, 0, topSpeed);

			Vector3 direction = (enterTarget - Requester.transform.position).normalized;
			Requester.transform.position = Requester.transform.position + direction * _currentSpeed * Time.fixedDeltaTime;
			Requester.transform.LookAt(enterTarget);
			Requester.InPortalSpeed = _currentSpeed;

			if(totalDist < 0.2f)
			{
				Stage = DockingSessionStage.Docked;
				Gate.Close();
				Gate.SetRedLight();
				if(Requester == GameManager.Inst.PlayerControl.PlayerShip)
				{
					GameManager.Inst.PlayerControl.DockComplete();
				}
			}
		}
	}


	private bool CheckRequesterInPosition()
	{

		float angle = Vector3.Angle(Requester.transform.forward, Gate.DockingTrigger.transform.forward * -1);
		if(angle > 3)
		{
			return false;
		}
		if(Vector3.Distance(Requester.transform.position, _dockEnterTarget) > 0.1f)
		{
			return false;
		}


		return true;
	}
}

public enum DockingSessionStage
{
	Granted,
	Docking,
	Entering,
	Docked,
}