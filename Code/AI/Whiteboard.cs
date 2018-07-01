using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard 
{

	public Dictionary<string, object> Parameters;

	public void Initialize()
	{
		Parameters = new Dictionary<string, object>();

		//static variables
		Parameters.Add("FriendlyFollowDist", 10f);
		Parameters.Add("EnemyCloseRange", 40f);
		Parameters.Add("MinEnemyRange", 10f);
		Parameters.Add("FiringRange", 60f);

		//dynamic variables
		Parameters.Add("FriendlyTarget", null);
		Parameters.Add("Destination", Vector3.zero);
		Parameters.Add("InterceptDest", Vector3.zero);
		Parameters.Add("AvoidanceVector", Vector3.zero);
		Parameters.Add("EvadeDir", Vector3.zero);
		Parameters.Add("AimPoint", Vector3.zero);
		Parameters.Add("IsThrusting", false);
		Parameters.Add("IsEngineKilled", false);
		Parameters.Add("TargetEnemy", null);
		Parameters.Add("AimTarget", null);
		Parameters.Add("SpeedLimit", 0f);
		Parameters.Add("StrafeForce", 0f);
		Parameters.Add("IgnoreAvoidance", false);

	}
}
