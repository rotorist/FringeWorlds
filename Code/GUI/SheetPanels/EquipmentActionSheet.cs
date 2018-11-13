using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentActionSheet : PanelBase
{
	public UILabel ItemTitle;
	public UILabel ItemDesc;
	public UILabel ItemAttributes;
	public UILabel ItemAttributeValues;

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
	}

	public void ListItemAttributes(List<ItemAttribute> attributes)
	{
		string attributeNames = "";
		string attributeValues = "";
		foreach(ItemAttribute attribute in attributes)
		{
			if(!attribute.IsHidden)
			{
				attributeNames += attribute.Name + '\n';
				attributeValues += attribute.Value.ToString() + '\n';
			}
		}

		ItemAttributes.text = attributeNames;
		ItemAttributeValues.text = attributeValues;
	}

}
