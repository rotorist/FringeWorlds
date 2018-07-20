using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidanceDetector : MonoBehaviour 
{
	public ShipBase ParentShip;
	public Transform RaySource1;
	public Transform RaySource2;
	public Transform RaySource3;

	public Vector3 Avoidance = Vector3.zero;

	public AvoidanceState State;
	
	public void AvoidanceUpdate()
	{
		Avoidance = Vector3.zero;
		if(ParentShip.MyReference.ShipType == ShipType.Fighter)
		{
			Vector3 velocity = ParentShip.RB.velocity;
			if(velocity == Vector3.zero)
			{
				velocity = ParentShip.ShipModel.transform.forward;
			}

			//always point me to velocity direction
			transform.rotation = Quaternion.LookRotation(velocity);

			//shoot a ray from each source
			RaycastHit hit;
			//ignore pickups, shields
			int mask = ~(1<<2 | 1<<8 | 1<<9 | 1<<10);

			float rayDist = 0;
			if(State == AvoidanceState.Travel)
			{
				rayDist = Mathf.Lerp(10, 30, Mathf.Clamp01(velocity.magnitude / 10));
			}
			else if(State == AvoidanceState.Combat)
			{
				rayDist = Mathf.Lerp(20, 40, Mathf.Clamp01(velocity.magnitude / 10));
			}

			//if there's a hit, add 1 * dist/rayDist to avoidance x
			//if there's no hit, minus 2 from avoidance x
			if(Physics.Raycast(RaySource1.position, velocity, out hit, rayDist, mask))
			{
				float dist = Vector3.Distance(hit.point, RaySource1.position);
				Avoidance.x += 1 * dist / rayDist;
			}
			else
			{
				Avoidance.x = Mathf.Clamp01(Avoidance.x - 2);
			}

			//if there's a hit, add 1 * dist/rayDist to avoidance x
			//if there's no hit, minus 2 from avoidance x
			if(Physics.Raycast(RaySource2.position, velocity, out hit, rayDist, mask))
			{
				float dist = Vector3.Distance(hit.point, RaySource2.position);
				Avoidance.y += 1 * dist / rayDist;
			}
			else
			{
				Avoidance.y = Mathf.Clamp01(Avoidance.y - 2);
			}

			//if there's a hit, add 1 * dist/rayDist to avoidance x
			//if there's no hit, minus 2 from avoidance x
			if(Physics.Raycast(RaySource3.position, velocity, out hit, rayDist, mask))
			{
				float dist = Vector3.Distance(hit.point, RaySource3.position);
				Avoidance.z += 1 * dist / rayDist;
			}
			else
			{
				Avoidance.z = Mathf.Clamp01(Avoidance.z - 2);
			}


		}
	}
}

public enum AvoidanceState
{
	Travel,
	Combat,
}