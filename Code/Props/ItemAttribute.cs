using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemAttribute 
{
	public string Name;
	public object Value;
	public string SerValue;

	public ItemAttribute(string name, object value)
	{
		Name = name;
		Value = value;
	}

}

