using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTFighterEvade : BTLeaf
{
	private float _evadeTimer;
	private float _evadeTimeout;

	private Vector3 _rawDest;

	public override void Initialize ()
	{
		//Debug.Log("Initializing Fighter Evade");
		//MyAI.Whiteboard.Parameters["EvadeDir"] = Vector3.zero;
		MyAI.Whiteboard.Parameters["AimTarget"] = null;
		_evadeTimer = 0;
		_evadeTimeout = UnityEngine.Random.Range(3f, 4f);
		_rawDest = Vector3.zero;
		this.LastRunTime = 0;
	}

	public override BTResult Process ()
	{
		//Debug.Log("processing fighter evade");

		if(this.LastRunTime <= 0)
		{
			this.LastRunTime = Time.time;
		}
		float deltaTime = Time.time - this.LastRunTime;
		this.LastRunTime = Time.time;



		Vector3 threatPos = Vector3.zero;
		ShipBase target = (ShipBase)MyAI.Whiteboard.Parameters["TargetEnemy"];
		if(target != null)
		{
			threatPos = target.transform.position;
		}

		Vector3 evadeDir = (Vector3)MyAI.Whiteboard.Parameters["EvadeDir"];
		if(evadeDir == Vector3.zero)
		{
			//Debug.Log("Recalculating evade dir");
			_rawDest = MyAI.MyShip.transform.position + new Vector3(UnityEngine.Random.value * 30, UnityEngine.Random.value * 30, UnityEngine.Random.value * 30);
			//find a random direction perpendicular to los
			Vector3 planeNormal = MyAI.MyShip.transform.forward;
			if(threatPos != Vector3.zero)
			{
				planeNormal = MyAI.MyShip.transform.position - threatPos;
			}
			Vector3 projVec = Vector3.ProjectOnPlane((_rawDest - MyAI.MyShip.transform.position), planeNormal);
			if(projVec == Vector3.zero)
			{
				projVec = MyAI.MyShip.transform.right;
			}
			evadeDir = projVec.normalized - planeNormal.normalized * UnityEngine.Random.Range(0.0f, 0.3f);
			MyAI.Whiteboard.Parameters["EvadeDir"] = evadeDir;
		}

		MyAI.Whiteboard.Parameters["IsThrusting"] = true;

		Debug.DrawRay(MyAI.MyShip.transform.position, evadeDir * 30, Color.blue);
		MyAI.Whiteboard.Parameters["Destination"] = MyAI.MyShip.transform.position + evadeDir.normalized * 30;

		_evadeTimer += deltaTime;

		if(_evadeTimer < _evadeTimeout)
		{
			return Running();
		}
		else
		{
			
			return Exit(BTResult.Success);
		}


	}

	public override BTResult Exit (BTResult result)
	{
		MyAI.Whiteboard.Parameters["EvadeDir"] = Vector3.zero;
		MyAI.Whiteboard.Parameters["IsThrusting"] = false;
		return result;
	}

	public override BTResult Running ()
	{
		MyAI.RunningNodeHist.UniquePush("Fighter Evade");
		return BTResult.Running;
	}
}
