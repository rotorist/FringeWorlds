using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMDispenser : Defensive
{
	

	public void DropCountermeasure()
	{
		if(AmmoID == "")
		{
			return;
		}

		Item ammo = ParentShip.Storage.TakeAmmo(AmmoID, 1, "Countermeasure", false);
		if(ammo != null)
		{
			string prefabID = ammo.GetStringAttribute("Weapon Prefab ID");
			GameObject cm = GameObject.Instantiate(Resources.Load(prefabID)) as GameObject;
			cm.transform.parent = ParentShip.transform;
			cm.transform.localPosition = Vector3.zero;
			cm.transform.localEulerAngles = Vector3.zero;
			cm.transform.localScale = new Vector3(1, 1, 1);
			cm.transform.parent = null;
			CounterMeasureFlares flares = cm.GetComponentInChildren<CounterMeasureFlares>();

			flares.DeviationRadius = ammo.GetFloatAttribute("Missile Deviation");
			flares.EffectiveTime = ammo.GetFloatAttribute("Effective Duration");
			flares.InitialVelocity = ParentShip.RB.velocity;
			ParentShip.CurrentCountermeasure = cm;

			flares.StartEffect();
		}
	}

}
