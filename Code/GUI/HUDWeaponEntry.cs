using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDWeaponEntry : MonoBehaviour 
{
	public UILabel WeaponGroupLabel;
	public UILabel WeaponNameLabel;

	public void UpdateEntry(int groupNumber, string weaponName)
	{
		WeaponGroupLabel.text = groupNumber.ToString();
		WeaponNameLabel.text = weaponName;
	}

}
