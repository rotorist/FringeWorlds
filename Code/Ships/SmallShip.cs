using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallShip : ShipBase
{
	//for testing only
	/*
	public Transform LeftGunJoint;
	public Transform LeftGun;
	public Transform RightGunJoint;
	public Transform RightGun;
	*/

	public override void EnableColliders ()
	{
		//hull collider
		Collider collider = this.ShipModel.GetComponent<Collider>();
		collider.enabled = true;
		//shield collider
		if(this.Shield != null)
		{
			this.Shield.GetComponent<Collider>().enabled = true;
		}
	}

	public override void DisableColliders ()
	{
		//hull collider
		Collider collider = this.ShipModel.GetComponent<Collider>();
		collider.enabled = false;
		//shield collider
		if(this.Shield != null)
		{
			this.Shield.GetComponent<Collider>().enabled = false;
		}
	}
}
