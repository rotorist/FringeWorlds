﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponJoint : MonoBehaviour 
{
	public string JointID;
	public Weapon MountedWeapon;
	public Transform Target;
	public Vector3 TargetPos;
	public float GimballLimitPercent;
	public TurretControlMode ControlMode;
	public ShipBase ParentShip;

	void Update()
	{
		if((ControlMode == TurretControlMode.Automatic || ControlMode == TurretControlMode.Selected) && MountedWeapon != null 
			&& MountedWeapon.RotationType == WeaponRotationType.Turret && ParentShip.DockedStationID == "" && !ParentShip.IsInPortal)
		{
			UpdateTarget();
			if(Target != null && !ParentShip.Engine.IsCruising)
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
						50, Target.position, targetVelocity);
				}
				else if(MountedWeapon.TurretBase != null)
				{
					//TargetPos = transform.position + transform.forward * 100;
				}
			}

			Vector3 lookDir = TargetPos - MountedWeapon.transform.position;

			if(MountedWeapon.RotationType == WeaponRotationType.Gimball)
			{
				Vector3 verticalLos = lookDir - (transform.forward * 100);
				float angle = Vector3.Angle(lookDir, transform.forward);
				if(angle > MountedWeapon.GimballMax * GimballLimitPercent)
				{
					
					verticalLos = verticalLos.normalized * (Mathf.Tan(Mathf.Deg2Rad * MountedWeapon.GimballMax * GimballLimitPercent) * 100);
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
				if(angle > MountedWeapon.GimballMax)
				{
					//verticalLos = verticalLos.normalized * (Mathf.Tan(Mathf.Deg2Rad * GimballMax * GimballLimitPercent) * 100);
					//Vector3 newTarget = transform.position + transform.up * 100 + verticalLos;
					//barrelLookDir = newTarget - MountedWeapon.transform.position;
					Vector3 flatDir = Vector3.ProjectOnPlane(lookDir, MountedWeapon.TurretBase.transform.up);
					barrelLookDir = Vector3.ProjectOnPlane(flatDir, MountedWeapon.TurretBase.transform.right);

					
				}
				else if(angle < MountedWeapon.GimballMin)
				{
					Vector3 flatDir = Vector3.ProjectOnPlane(lookDir, MountedWeapon.TurretBase.transform.up);
					barrelLookDir = Vector3.ProjectOnPlane(flatDir, MountedWeapon.TurretBase.transform.right);
					barrelLookDir = barrelLookDir / Mathf.Cos(Mathf.Deg2Rad * (MountedWeapon.GimballMax - MountedWeapon.GimballMin));
				}

				Quaternion barrelRotation = Quaternion.LookRotation(barrelLookDir, Vector3.Cross(barrelLookDir, MountedWeapon.TurretBase.transform.right) * -1);

				MountedWeapon.TurretBase.transform.rotation = Quaternion.Lerp(MountedWeapon.TurretBase.transform.rotation, baseRotation, Time.deltaTime * 6);
				MountedWeapon.Barrel.transform.rotation = Quaternion.Lerp(MountedWeapon.Barrel.transform.rotation, barrelRotation, Time.deltaTime * 6);
			}
			else
			{
				
			}
		}
	}

	public void LoadWeapon(InvItemData weaponItem)
	{
		if(MountedWeapon != null)
		{
			GameObject.Destroy(MountedWeapon.gameObject);
			MountedWeapon = null;
		}
		Debug.Log("Loading weapon " + weaponItem.Item.ID);
		GameObject o = GameObject.Instantiate(Resources.Load(weaponItem.Item.GetStringAttribute("Weapon Prefab ID"))) as GameObject;
		Weapon weapon = o.GetComponent<Weapon>();
		MountedWeapon = weapon;
		MountedWeapon.transform.parent = transform;
		MountedWeapon.transform.localPosition = Vector3.zero;
		MountedWeapon.transform.localEulerAngles = Vector3.zero;
		//MountedWeapon.transform.LookAt(transform.forward, transform.up);
		MountedWeapon.ParentShip = ParentShip;
		MountedWeapon.DisplayName = weaponItem.Item.DisplayName;
		MountedWeapon.FiringSound = weaponItem.Item.GetStringAttribute("Firing Sound");
		MountedWeapon.Initialize(weaponItem, weaponItem.RelatedItemID);

		MountedWeapon.Rebuild();
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
			float angle = Vector3.Angle(MountedWeapon.TurretBase.transform.up, los);
			if(angle > MountedWeapon.GimballMax * GimballLimitPercent || angle < MountedWeapon.GimballMin || los.magnitude > MountedWeapon.Range)
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
				float angle = Vector3.Angle(MountedWeapon.TurretBase.transform.up, los);
				if(relationship < 0.4f && angle <= MountedWeapon.GimballMax * GimballLimitPercent && angle >= MountedWeapon.GimballMin && los.magnitude < MountedWeapon.Range)
				{
					Target = ship.transform;
					return;
				}
			}
		}
	}
}

[System.Serializable]
public class WeaponJointData
{
	public string JointID;
	public int Class;
	public WeaponRotationType RotationType;
}

public enum TurretControlMode
{
	Manual,
	Selected,
	Automatic,
}

public enum WeaponRotationType
{
	Gimball,
	Turret,
	Fixed,
}