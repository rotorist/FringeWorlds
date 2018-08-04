using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponJoint : MonoBehaviour 
{
	public string JointID;
	public Weapon MountedWeapon;
	public Transform Target;
	public Vector3 TargetPos;
	public float GimballLimitPercent;
	public float GimballMax;
	public TurretControlMode ControlMode;
	public ShipBase ParentShip;

	void Update()
	{
		if((ControlMode == TurretControlMode.Automatic || ControlMode == TurretControlMode.Selected) && MountedWeapon != null 
			&& MountedWeapon.RotationType == WeaponRotationType.Turret && ParentShip.DockedStationID == "" && !ParentShip.IsInPortal)
		{
			UpdateTarget();
			if(Target != null)
			{
				
				MountedWeapon.Fire();
			}
		}

		if(MountedWeapon != null)
		{
			if(MountedWeapon.RotationType == WeaponRotationType.Turret && (ControlMode == TurretControlMode.Automatic || ControlMode == TurretControlMode.Selected))
			{
				if(Target != null)
				{
					Vector3 targetVelocity = Vector3.zero;
					if(transform.GetComponent<Rigidbody>() != null)
					{
						targetVelocity = transform.GetComponent<Rigidbody>().velocity;
					}
					TargetPos = StaticUtility.FirstOrderIntercept(ParentShip.transform.position, ParentShip.RB.velocity,
						30, Target.position, targetVelocity);
				}
				else if(MountedWeapon.TurretBase != null)
				{
					TargetPos = transform.position + transform.forward * 100;
				}
			}

			Vector3 lookDir = TargetPos - MountedWeapon.transform.position;

			if(MountedWeapon.RotationType == WeaponRotationType.Gimball)
			{
				Vector3 verticalLos = lookDir - (transform.forward * 100);
				float angle = Vector3.Angle(lookDir, transform.forward);

				if(angle > GimballMax * GimballLimitPercent)
				{
					verticalLos = verticalLos.normalized * (Mathf.Tan(Mathf.Deg2Rad * GimballMax * GimballLimitPercent) * 100);
					Vector3 newTarget = transform.position + transform.forward * 100 + verticalLos;
					lookDir = newTarget - MountedWeapon.transform.position;

				}

				Quaternion rotation = Quaternion.LookRotation(lookDir, transform.up);
				MountedWeapon.Barrel.transform.rotation = Quaternion.Lerp(MountedWeapon.Barrel.transform.rotation, rotation, Time.deltaTime * 9);
			}
			else if(MountedWeapon.RotationType == WeaponRotationType.Turret)
			{
				Vector3 baseLookDir = Vector3.ProjectOnPlane(lookDir, transform.up);
				Quaternion baseRotation = Quaternion.LookRotation(baseLookDir, transform.up);

				Vector3 barrelLookDir = Vector3.ProjectOnPlane(lookDir, MountedWeapon.TurretBase.transform.right);
				Vector3 verticalLos = lookDir - (transform.up * 100);
				float angle = Vector3.Angle(lookDir, transform.up);
				if(angle > GimballMax * GimballLimitPercent)
				{
					//verticalLos = verticalLos.normalized * (Mathf.Tan(Mathf.Deg2Rad * GimballMax * GimballLimitPercent) * 100);
					//Vector3 newTarget = transform.position + transform.up * 100 + verticalLos;
					//barrelLookDir = newTarget - MountedWeapon.transform.position;
					Vector3 flatDir = Vector3.ProjectOnPlane(lookDir, MountedWeapon.TurretBase.transform.up);
					barrelLookDir = Vector3.ProjectOnPlane(flatDir, MountedWeapon.TurretBase.transform.right);
				}

				Quaternion barrelRotation = Quaternion.LookRotation(barrelLookDir, Vector3.Cross(barrelLookDir, MountedWeapon.TurretBase.transform.right) * -1);

				MountedWeapon.TurretBase.transform.rotation = Quaternion.Lerp(MountedWeapon.TurretBase.transform.rotation, baseRotation, Time.deltaTime * 9);
				MountedWeapon.Barrel.transform.rotation = Quaternion.Lerp(MountedWeapon.Barrel.transform.rotation, barrelRotation, Time.deltaTime * 9);
			}
			else
			{
				
			}
		}
	}

	public void LoadWeapon(string weaponID)
	{
		if(MountedWeapon != null)
		{
			GameObject.Destroy(MountedWeapon);
			MountedWeapon = null;
		}

		GameObject o = GameObject.Instantiate(Resources.Load(weaponID)) as GameObject;
		Weapon weapon = o.GetComponent<Weapon>();
		MountedWeapon = weapon;
		MountedWeapon.transform.parent = transform;
		MountedWeapon.transform.localPosition = Vector3.zero;
		MountedWeapon.transform.LookAt(transform.forward);
		MountedWeapon.ParentShip = ParentShip;
	}


	private void UpdateTarget()
	{
		if(ControlMode == TurretControlMode.Selected && ParentShip == GameManager.Inst.PlayerControl.PlayerShip)
		{
			Target = null;

			if(GameManager.Inst.PlayerControl.SelectedObjectType == SelectedObjectType.Ship)
			{
				ShipBase ship = (ShipBase)GameManager.Inst.PlayerControl.SelectedObject;
				float relationship = GameManager.Inst.NPCManager.GetFactionRelationship(ParentShip.MyAI.MyFaction, ship.MyAI.MyFaction);
				if(relationship < 0.4f && ship.DockedStationID == "")
				{
					Target = ship.transform;
				}
			}
			else if(GameManager.Inst.PlayerControl.SelectedObjectType == SelectedObjectType.Turret)
			{

			}

			return;
		}

		if(Target != null)
		{
			//check if target is still within range and fov
			Vector3 los = Target.transform.position - transform.position;
			if(Vector3.Angle(MountedWeapon.TurretBase.transform.up, los) > GimballMax * GimballLimitPercent || los.magnitude > MountedWeapon.Range)
			{
				Target = null;
				return;
			}
		}
		else
		{
			foreach(ShipBase ship in GameManager.Inst.NPCManager.AllShips)
			{
				if(ship == ParentShip || ship.DockedStationID != "")
				{
					continue;
				}
				float relationship = GameManager.Inst.NPCManager.GetFactionRelationship(ParentShip.MyAI.MyFaction, ship.MyAI.MyFaction);
				Vector3 los = ship.transform.position - transform.position;

				if(relationship < 0.4f && Vector3.Angle(MountedWeapon.TurretBase.transform.up, los) <= GimballMax * GimballLimitPercent && los.magnitude < MountedWeapon.Range)
				{
					Target = ship.transform;
					return;
				}
			}
		}
	}
}

public enum TurretControlMode
{
	Manual,
	Selected,
	Automatic,
}
