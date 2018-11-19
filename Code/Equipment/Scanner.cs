using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : EquipmentBase
{
	public List<ScannerCoverage> Coverage;
	public float Range;
	public float ScanDelay;

	public void Initialize(InvItemData itemData)
	{
		if(itemData == null)
		{
			Range = 100;
			Coverage = new List<ScannerCoverage>();
			Coverage.Add(ScannerCoverage.Ship);
			ScanDelay = 0;
		}
		else
		{
			Range = itemData.Item.GetFloatAttribute("Scan Range");
			ScanDelay = itemData.Item.GetFloatAttribute("Scan Delay");
			Coverage = new List<ScannerCoverage>();
			int [] coverages = itemData.Item.GetIntArrayAttribute("Coverage Data");
			foreach(int coverage in coverages)
			{
				Coverage.Add((ScannerCoverage)coverage);
			}
			this.PowerRequired = itemData.Item.GetFloatAttribute("Power Required");
		}
	}
}

public enum ScannerCoverage
{
	Ship = 0,
	Weapon = 1,
	Equipment = 2,
	Cargo = 3,
	SecretCargo = 4,
}