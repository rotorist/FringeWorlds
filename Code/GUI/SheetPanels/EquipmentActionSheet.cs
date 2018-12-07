using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentActionSheet : PanelBase
{
	public UILabel ItemTitle;
	public UILabel ItemDesc;
	public UILabel ItemAttributes;
	public UILabel ItemAttributeValues;
	public UILabel ItemSubnoteTitle;
	public UILabel ItemSubnote;

	public override void Initialize ()
	{
		base.Initialize();
	}

	public override void Show ()
	{
		base.Show ();
	}

	public override void Hide ()
	{
		base.Hide ();
	}

	public void SetItemTitle(string title)
	{
		ItemTitle.text = title;
		if(title == "")
		{
			ItemTitle.text = "Pending Item Selection...";
		}
	}

	public void SetItemDesc(string description)
	{
		ItemDesc.text = description;
		Debug.Log("Description height " + ItemDesc.height);
	}

	public void SetItemSubnote(string title, string subnote)
	{
		ItemSubnoteTitle.text = title;
		ItemSubnote.text = subnote;
		ItemSubnoteTitle.transform.localPosition = new Vector3(0, ItemDesc.transform.localPosition.y - ItemDesc.height - 12, 0);
		ItemSubnote.transform.localPosition = ItemSubnoteTitle.transform.localPosition;
	}

	public void ListItemAttributes(Item item)
	{
		string attributeNames = "";
		string attributeValues = "";
		foreach(ItemAttribute attribute in item.Attributes)
		{
			if(!attribute.IsHidden)
			{
				attributeNames += attribute.Name + '\n';
				attributeValues += attribute.Value.ToString();
				if(!string.IsNullOrEmpty(attribute.Unit))
				{
					attributeValues += " " + attribute.Unit;
				}
				attributeValues += '\n';
			}
		}
		//add cargo units
		attributeNames += "Cargo Space";
		attributeValues += item.CargoUnits.ToString();

		ItemAttributes.text = attributeNames;
		ItemAttributeValues.text = attributeValues;
	}

	public void Clear()
	{
		SetItemTitle("");
		SetItemDesc("");
		ItemAttributes.text = "";
		ItemAttributeValues.text = "";
	}

}
