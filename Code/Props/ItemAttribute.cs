using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemAttribute 
{
	public string Name;
	public object Value;
	public string SerValue;
	public bool IsHidden;
	public string Unit;

	public ItemAttribute(string name, object value, bool isHidden, string unit)
	{
		Name = name;
		Value = value;
		IsHidden = isHidden;
		Unit = unit;
	}

}

