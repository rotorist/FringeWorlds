using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDWeaponEntry : MonoBehaviour 
{
	public UILabel WeaponGroupLabel;
	public UILabel WeaponNameLabel;
	public UILabel AmmoLabel;
	public string AmmoID;



	public void UpdateEntry(int groupNumber, string weaponName, int ammoCount)
	{
		if(groupNumber < 0)
		{
			WeaponGroupLabel.text = "";
		}
		else
		{
			WeaponGroupLabel.text = groupNumber.ToString();
		}
		WeaponNameLabel.text = weaponName;

		UpdateAmmoCount(ammoCount);
	}

	public void UpdateAmmoCount(int ammoCount)
	{
		if(ammoCount < 0)
		{
			AmmoLabel.text = "";
		}
		else
		{
			AmmoLabel.text = ammoCount.ToString();
		}
	}

}
