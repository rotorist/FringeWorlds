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

		Item ammo = ParentShip.Storage.TakeAmmo(AmmoID, 1);
		if(ammo != null)
		{
			GameObject cm = GameObject.Instantiate(Resources.Load("CountermeasureEffect")) as GameObject;
			cm.transform.parent = ParentShip.transform;
			cm.transform.localPosition = Vector3.zero;
			cm.transform.localEulerAngles = Vector3.zero;
			cm.transform.localScale = new Vector3(1, 1, 1);
			cm.transform.parent = null;
			CounterMeasureFlares flares = cm.GetComponentInChildren<CounterMeasureFlares>();

			flares.DeviationRadius = ammo.GetFloatAttribute("DeviationRadius");
			flares.EffectiveTime = ammo.GetFloatAttribute("EffectiveDuration");
			flares.InitialVelocity = ParentShip.RB.velocity;
			ParentShip.CurrentCountermeasure = cm;

			flares.StartEffect();
		}
	}

}
