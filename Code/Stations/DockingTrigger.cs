using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingTrigger : MonoBehaviour 
{
	public string ID;
	public StationBase ParentStation;

	void OnTriggerEnter(Collider other)
	{
		ShipReference shipRef = other.GetComponent<ShipReference>();
		if(shipRef != null)
		{
			ParentStation.OnDetectDocking(ID, shipRef.ParentShip);
		}
	}
}
