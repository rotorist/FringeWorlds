using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InvItemData
{

	public Item Item;
	public int Quantity;
	public string RelatedItemID;

	public InvItemData()
	{

	}

	public InvItemData(InvItemData template)
	{
		Item = new Item(template.Item);
		Quantity = template.Quantity;
		RelatedItemID = template.RelatedItemID;
	}
}
