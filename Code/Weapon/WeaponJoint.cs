using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponJoint : MonoBehaviour 
{
	public string JointID;
	public Weapon MountedWeapon;
	public Vector3 TargetPos;
	public float GimballLimitPercent;
	public float GimballMax;
	public ShipBase ParentShip;

	void Update()
	{
		if(GimballMax > 0 && MountedWeapon != null)
		{
			Vector3 lookDir = TargetPos - MountedWeapon.transform.position;
			Vector3 verticalLos = lookDir - (transform.forward * 100);
			float angle = Vector3.Angle(lookDir, transform.forward);

			if(angle > GimballMax * GimballLimitPercent)
			{
				verticalLos = verticalLos.normalized * (Mathf.Tan(Mathf.Deg2Rad * GimballMax * GimballLimitPercent) * 100);
				Vector3 newTarget = transform.position + transform.forward * 100 + verticalLos;
				lookDir = newTarget - MountedWeapon.transform.position;

			}

			Quaternion rotation = Quaternion.LookRotation(lookDir, transform.up);
			MountedWeapon.transform.rotation = Quaternion.Lerp(MountedWeapon.transform.rotation, rotation, Time.deltaTime * 9);
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
}
