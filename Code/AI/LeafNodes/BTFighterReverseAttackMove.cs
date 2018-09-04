using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTFighterReverseAttackMove : BTLeaf
{
	public int State;

	private float _timeout;
	private float _timer;
	private Vector3 _evadeDir;

	public override void Initialize ()
	{
		//Debug.Log("Initializing fighter reverse attack move");
		MyAI.Whiteboard.Parameters["IsEngineKilled"] = true;

		_timeout = UnityEngine.Random.Range(3f, 5f);
		_timer = 0;
		State = 0;
		_evadeDir = Vector3.zero;
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

		//Debug.Log("Processing Fighter Reverse Attack state " + State + " dir " + _evadeDir);
		Debug.DrawRay(MyAI.MyShip.transform.position, _evadeDir * 30, Color.blue);

		ShipBase target = (ShipBase)MyAI.Whiteboard.Parameters["TargetEnemy"];

		if(State == 0)
		{
			MyAI.Whiteboard.Parameters["AimTarget"] = target;
		}
		else
		{
			MyAI.Whiteboard.Parameters["AimTarget"] = null;
		}


		if(_timer >= _timeout)
		{
			if(State == 0)
			{
				

				if(target != null)
				{
					if(_evadeDir == Vector3.zero)
					{
						_evadeDir = StaticUtility.FindEvadeDirWithTarget(MyAI.MyShip, target.transform.position, 0.3f) * -1;
					}
				}
				else
				{
					_evadeDir = MyAI.MyShip.transform.forward;
				}

				MyAI.Whiteboard.Parameters["IsEngineKilled"] = false;
				MyAI.Whiteboard.Parameters["IsThrusting"] = true;
				MyAI.Whiteboard.Parameters["Destination"] = MyAI.MyShip.transform.position + _evadeDir * 60f;
				MyAI.Whiteboard.Parameters["EvadeDir"] = _evadeDir;

				State = 1;
				_timeout = UnityEngine.Random.Range(3f, 4f);
				_timer = 0;
			}
			else
			{
				
				return Exit(BTResult.Fail);
			}
		}



		_timer += deltaTime;

		return Running();
	}

	public override BTResult Exit (BTResult result)
	{
		MyAI.Whiteboard.Parameters["IsEngineKilled"] = false;
		MyAI.Whiteboard.Parameters["IsThrusting"] = false;
		return result;
	}

	public override BTResult Running ()
	{
		MyAI.RunningNodeHist.UniquePush("Fighter Reverse Attack");
		return BTResult.Running;
	}
}
