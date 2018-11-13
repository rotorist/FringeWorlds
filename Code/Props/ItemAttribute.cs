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

	public ItemAttribute(string name, object value, bool isHidden)
	{
		Name = name;
		Value = value;
		IsHidden = isHidden;
	}

}

