using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : EquipmentBase
{
	public ScannerType Type;
	public float Range;

}

public enum ScannerType
{
	LongRange,
	MediumRange,
	ShortRange,
}