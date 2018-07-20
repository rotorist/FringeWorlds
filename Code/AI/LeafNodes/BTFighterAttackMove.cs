using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTFighterAttackMove : BTLeaf
{
	public enum MoveStates
	{
		LongRange,
		CloseChase,
		CloseJoust,
		TooClose,
	}

	public MoveStates State;
	private ShipBase _target;
	private float _targetDist;
	private float _targetLosAngle;
	private float _targetFacingAngle;
	private Vector3 _evadeDir;
	private float _timer;
	private float _prevDist;

	public override void Initialize ()
	{
		//Debug.Log("Initializing Fighter Attack Move");
		State = MoveStates.LongRange;
		this.LastRunTime = 0;
	}

	public override BTResult Process ()
	{
		if(this.LastRunTime <= 0)
		{
			this.LastRunTime = Time.time;
		}
		float deltaTime = Time.time - this.LastRunTime;
		this.LastRunTime = Time.time;


		ShipBase target = (ShipBase)MyAI.Whiteboard.Parameters[Parameters[0]];
		//Debug.Log("Processing FighterAttackMove current state " + State.ToString() + " dist " + _targetDist + " is target null? " + (target == null));
		if(target != null)
		{
			_target = target;
			_targetDist = Vector3.Distance(MyAI.MyShip.transform.position, target.transform.position);
			_targetLosAngle = Vector3.Angle(target.transform.position - MyAI.MyShip.transform.position, MyAI.MyShip.transform.forward);
			_targetFacingAngle = Vector3.Angle(target.transform.position - MyAI.MyShip.transform.position, target.transform.forward);

			if(_targetLosAngle > 150 || _targetDist < (float)MyAI.Whiteboard.Parameters["MinEnemyRange"])
			{
				return BTResult.Fail;
			}

			switch(State)
			{
			case MoveStates.LongRange:
				HandleMoveStateLongRange();
				break;
			case MoveStates.CloseChase:
				HandleMoveStateCloseChase();
				break;
			case MoveStates.CloseJoust:
				HandleMoveStateCloseJoust();
				break;
			}

			_timer += deltaTime;
			_prevDist = _targetDist;

			return BTResult.Running;
		}
		else
		{
			return Exit(BTResult.Fail);
		}

	}

	public override BTResult Exit (BTResult result)
	{
		return result;
	}
		

	private void HandleMoveStateLongRange()
	{
		if(_targetDist <= (float)MyAI.Whiteboard.Parameters["EnemyCloseRange"])
		{
			if(_targetFacingAngle > 120)
			{
				ChangeState(MoveStates.CloseJoust);
				return;
			}
			else
			{
				ChangeState(MoveStates.CloseChase);
				return;
			}


		}


		//strafe left and right
		if(_timer > 2)
		{
			float strafeForce = (float)MyAI.Whiteboard.Parameters["StrafeForce"];
			if(strafeForce == 0)
			{
				strafeForce = (2 + UnityEngine.Random.value * 3) * StaticUtility.FlipCoin();
			}
			else
			{
				strafeForce *= -1;
			}
			_timer = 0;
		}
		MyAI.Whiteboard.Parameters["IsThrusting"] = true;
		MyAI.Whiteboard.Parameters["AimTarget"] = _target;
		//set destination to target
		MyAI.Whiteboard.Parameters["Destination"] = _target.transform.position;
	}

	private void HandleMoveStateCloseChase()
	{
		if(_targetDist > (float)MyAI.Whiteboard.Parameters["EnemyCloseRange"])
		{
			ChangeState(MoveStates.LongRange);
			return;
		}


		if(_targetFacingAngle > 120)
		{
			ChangeState(MoveStates.CloseJoust);
			return;
		}

		MyAI.Whiteboard.Parameters["IsThrusting"] = true;
		MyAI.Whiteboard.Parameters["AimTarget"] = _target;
		//set destination to target
		MyAI.Whiteboard.Parameters["Destination"] = _target.transform.position;
		//match speed
		MyAI.Whiteboard.Parameters["SpeedLimit"] = _target.RB.velocity.magnitude;
	}

	private void HandleMoveStateCloseJoust()
	{
		if(_targetDist > (float)MyAI.Whiteboard.Parameters["EnemyCloseRange"])
		{
			ChangeState(MoveStates.LongRange);
			return;
		}


		if(_targetFacingAngle <= 120)
		{
			ChangeState(MoveStates.CloseChase);
			return;
		}

		if(_evadeDir == Vector3.zero)
		{
			_evadeDir = StaticUtility.FindEvadeDirWithTarget(MyAI.MyShip, _target.transform.position, 0.4f);
			if(Vector3.Angle(MyAI.RB.velocity, _target.transform.position - MyAI.MyShip.transform.position) > 145)
			{
				MyAI.Whiteboard.Parameters["IsEngineKilled"] = true;
			}
			MyAI.Whiteboard.Parameters["Destination"] = MyAI.MyShip.transform.position + _evadeDir * 30f;
		}

		Debug.DrawRay(MyAI.MyShip.transform.position, _evadeDir.normalized * 10, Color.red);
		MyAI.Whiteboard.Parameters["EvadeDir"] = _evadeDir;
		MyAI.Whiteboard.Parameters["AimTarget"] = _target;
	}

	private void HandleStateTooClose()
	{
		if(_targetDist >= (float)MyAI.Whiteboard.Parameters["MinEnemyRange"] * 1.5f)
		{
			ChangeState(MoveStates.LongRange);
			return;
		}

		if(_evadeDir == Vector3.zero)
		{
			_evadeDir = StaticUtility.FindEvadeDirWithTarget(MyAI.MyShip, _target.transform.position, 0.4f);


		}
		MyAI.Whiteboard.Parameters["IsThrusting"] = true;
		MyAI.Whiteboard.Parameters["Destination"] = MyAI.MyShip.transform.position + _evadeDir * 30f;
		MyAI.Whiteboard.Parameters["EvadeDir"] = _evadeDir;
		MyAI.Whiteboard.Parameters["AimTarget"] = null;
	}

	private void ChangeState(MoveStates newState)
	{
		_timer = 0;
		_evadeDir = Vector3.zero;
		MyAI.Whiteboard.Parameters["SpeedLimit"] = -1f;
		MyAI.Whiteboard.Parameters["StrafeForce"] = 0f;
		MyAI.Whiteboard.Parameters["IsEngineKilled"] = false;
		MyAI.Whiteboard.Parameters["IsThrusting"] = false;
		State = newState;
	}
}
