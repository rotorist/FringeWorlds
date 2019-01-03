using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class StaticUtility
{
	public static float GetShieldDamageMultiplier(ShieldTech shieldTech, DamageType damageType)
	{
		return GameManager.Inst.Constants.ShieldDamageMultiplier[shieldTech][damageType];
	}

	public static float GetMaxElementV3(Vector3 input)
	{
		return Mathf.Max(Mathf.Max(input.x, input.y), input.z);
	}

	public static int FlipCoin()
	{
		if(UnityEngine.Random.value < 0.5f)
		{
			return -1;
		}
		else
		{
			return 1;
		}
	}

	public static Vector3 FindEvadeDirWithTarget(ShipBase self, Vector3 targetPos, float skew)
	{
		Vector3 rawDest = (targetPos - self.transform.position) * -1f;
		//find a random direction perpendicular to los
		Vector3 planeNormal = self.RB.velocity;
		if(planeNormal == Vector3.zero)
		{
			planeNormal = self.transform.forward;
		}

		Vector3 projVec = Vector3.ProjectOnPlane((rawDest - self.transform.position), planeNormal);
		if(projVec == Vector3.zero)
		{
			projVec = self.transform.right;
		}

		return planeNormal.normalized + projVec.normalized * skew;
	}

	public static bool CheckFighterOnTail(Vector3 targetPos, Vector3 myPos, Vector3 targetForward, Vector3 myForward)
	{
		//if the angle between target-me-los and my forward is less than 60 degrees
		//and the angle between target-me-los and target forward is less than 30 then enemy is on my tail
		Vector3 los = myPos - targetPos;
		float angleLosMyForward = Vector3.Angle(los, myForward);
		float angleLosTargetForward = Vector3.Angle(los, targetForward);
		//Debug.Log(angleLosMyForward + ", " +  angleLosTargetForward + ", " + los.magnitude);
		if(los.magnitude <= 5)
		{
			return true;
		}
		if(angleLosMyForward < 60 && angleLosTargetForward < 30 && los.magnitude < 60)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	//first-order intercept using absolute target position
	public static Vector3 FirstOrderIntercept
	(
		Vector3 shooterPosition,
		Vector3 shooterVelocity,
		float shotSpeed,
		Vector3 targetPosition,
		Vector3 targetVelocity
	)  {
		Vector3 targetRelativePosition = targetPosition - shooterPosition;
		Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
		float t = FirstOrderInterceptTime
			(
				shotSpeed,
				targetRelativePosition,
				targetRelativeVelocity
			);
		return targetPosition + t*(targetRelativeVelocity);
	}
	//first-order intercept using relative target position
	public static float FirstOrderInterceptTime
	(
		float shotSpeed,
		Vector3 targetRelativePosition,
		Vector3 targetRelativeVelocity
	) {
		float velocitySquared = targetRelativeVelocity.sqrMagnitude;
		if(velocitySquared < 0.001f)
			return 0f;

		float a = velocitySquared - shotSpeed*shotSpeed;

		//handle similar velocities
		if (Mathf.Abs(a) < 0.001f)
		{
			float t = -targetRelativePosition.sqrMagnitude/
				(
					2f*Vector3.Dot
					(
						targetRelativeVelocity,
						targetRelativePosition
					)
				);
			return Mathf.Max(t, 0f); //don't shoot back in time
		}

		float b = 2f*Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
		float c = targetRelativePosition.sqrMagnitude;
		float determinant = b*b - 4f*a*c;

		if (determinant > 0f) { //determinant > 0; two intercept paths (most common)
			float	t1 = (-b + Mathf.Sqrt(determinant))/(2f*a),
			t2 = (-b - Mathf.Sqrt(determinant))/(2f*a);
			if (t1 > 0f) {
				if (t2 > 0f)
					return Mathf.Min(t1, t2); //both are positive
				else
					return t1; //only t1 is positive
			} else
				return Mathf.Max(t2, 0f); //don't shoot back in time
		} else if (determinant < 0f) //determinant < 0; no intercept path
			return 0f;
		else //determinant = 0; one intercept path, pretty much never happens
			return Mathf.Max(-b/(2f*a), 0f); //don't shoot back in time
	}

	public static bool IsInArea(Vector3 targetPos, Vector3 center, Vector3 bounds)
	{
		if(targetPos.x > center.x - bounds.x && targetPos.x < center.x + bounds.x &&
			targetPos.y > center.y - bounds.y && targetPos.y < center.y + bounds.y &&
			targetPos.z > center.z - bounds.z && targetPos.z < center.z + bounds.z)
		{
			return true;
		}

		return false;
	}

	public static void ShuffleMe<T>(this IList<T> list)  
	{  
		System.Random random = new System.Random();
		int n = list.Count;  

		for(int i= list.Count - 1; i > 1; i--)
		{
			int rnd = random.Next(i + 1);  

			T value = list[rnd];  
			list[rnd] = list[i];  
			list[i] = value;
		}
	}
}
